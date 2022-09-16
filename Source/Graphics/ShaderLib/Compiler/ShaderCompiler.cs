﻿using System;
using Infinity.Memory;
using Infinity.Container;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static Infinity.Shaderlib.ShaderConductorWrapper;

namespace Infinity.Shaderlib
{
    internal unsafe class ShaderConductorDebugger
    {
        ShaderConductorBlob m_Target;

        public ShaderConductorDebugger(ShaderConductorBlob target)
        {
            m_Target = target;
        }

        public int Size
        {
            get
            {
                return m_Target.Size;
            }
        }

        public List<byte> Data
        {
            get
            {
                byte* dataPtr = (byte*)m_Target.Data.ToPointer();

                var result = new List<byte>();
                for (int i = 0; i < m_Target.Size; ++i)
                {
                    result.Add(dataPtr[i]);
                }
                return result;
            }
        }
    }

    [DebuggerTypeProxy(typeof(ShaderConductorDebugger))]
    public unsafe struct ShaderConductorBlob : IDisposable
    {
        public int Size => m_Size;
        public IntPtr Data => m_Data;

        private int m_Size;
        private IntPtr m_Data;
        private ResultDesc? m_Result;

        internal ShaderConductorBlob(in ResultDesc result)
        {
            m_Result = result;
            m_Size = GetShaderConductorBlobSize(result.target);
            m_Data = GetShaderConductorBlobData(result.target);
        }

        internal ShaderConductorBlob(in int size, in IntPtr data)
        {
            m_Size = size;
            m_Data = data;
            m_Result = default;
        }

        /*internal ShaderConductorBlob(in ResultDesc result)
        {
            m_Size = GetShaderConductorBlobSize(result.target);
            m_Data = new IntPtr(MemoryUtility.Malloc(sizeof(byte), m_Size));
            MemoryUtility.CopyTo(GetShaderConductorBlobData(result.target), m_Data, m_Size);
        }*/

        /*internal ShaderConductorBlob(in ResultDesc result)
        {
            int blobSize = GetShaderConductorBlobSize(result.target);
            m_Size = blobSize + (blobSize % 4);

            byte* dataPtr = (byte*)MemoryUtility.Malloc(sizeof(byte), m_Size);
            m_Data = new IntPtr(dataPtr);

            MemoryUtility.CopyTo(GetShaderConductorBlobData(result.target), m_Data, m_Size);
            for (int i = blobSize; i < m_Size; ++i)
            {
                dataPtr[i] = 0;
            }
        }*/

        public void Dispose()
        {
            //MemoryUtility.Free(m_Data.ToPointer());
            if(m_Result.HasValue)
            {
                DestroyShaderConductorBlob(m_Result.Value.target);
                DestroyShaderConductorBlob(m_Result.Value.errorWarningMsg);
            }
        }
    }

    public class ShaderCompilerLibraryLoader : IDisposable
    {
        string m_ShaderCode;
        //Vortice.Dxc.IDxcBlob m_CodeBlob;
        //Vortice.Dxc.IDxcResult m_DxcResult;

        public ShaderCompilerLibraryLoader()
        {
            m_ShaderCode = new string(@"
            [[vk::binding(0, 0)]]
            RWTexture2D<float4> _ResultTexture[1];

            [numthreads(8, 8, 1)]
            void Main (uint3 id : SV_DispatchThreadID)
            {
                float2 UV = (id.xy + 0.5) / float2(1600, 900);
                float IDMod7 = saturate(((id.x & 7) / 7) + ((id.y & 7) / 7));
                _ResultTexture[0][id.xy] = float4(id.x & id.y, IDMod7, UV);
            }");

            /*Vortice.Dxc.DxcCompilerOptions dxcOption = new Vortice.Dxc.DxcCompilerOptions();
            dxcOption.Enable16bitTypes = true;
            dxcOption.ShaderModel = Vortice.Dxc.DxcShaderModel.Model6_2;
            m_DxcResult = Vortice.Dxc.DxcCompiler.Compile(Vortice.Dxc.DxcShaderStage.Compute, m_ShaderCode, "Main", dxcOption);
            m_CodeBlob = m_DxcResult.GetOutput(Vortice.Dxc.DxcOutKind.Object);*/
        }

        public void Dispose()
        {
            //m_CodeBlob.Dispose();
            //m_DxcResult.Dispose();
        }
    }

    public static class ShaderCompiler
    {
        public static string DisassemblySPIRV(byte[] bytecode)
        {
            ResultDesc desc2;
            IntPtr destination = Marshal.AllocHGlobal((int) bytecode.Length);
            Marshal.Copy(bytecode, 0, destination, bytecode.Length);
            DisassembleDesc source = new DisassembleDesc {
                language = EShadingLanguage.SpirV,
                binary = destination,
                binarySize = bytecode.Length
            };
            Disassemble(ref source, out desc2);
            Marshal.FreeHGlobal(destination);
            DestroyShaderConductorBlob(desc2.target);
            DestroyShaderConductorBlob(desc2.errorWarningMsg);
            return Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc2.target), GetShaderConductorBlobSize(desc2.target));
        }

        public static string HLSLTo(string hlslSource, string entryPoint, in EShaderStage stage, in EShadingLanguage language, in bool keepDebugInfo = false, in bool disableOptimization = false)
        {
            string str2;
            ResultDesc desc4;
            SourceDesc source = new SourceDesc {
                stage = stage,
                source = hlslSource,
                entryPoint = entryPoint
            };
            OptionsDesc options = OptionsDesc.Default;
            options.shaderModel = new ShaderModel(6, 2);
            options.enable16bitTypes = true;
            options.enableDebugInfo = keepDebugInfo;
            options.optimizationLevel = 3;
            options.disableOptimizations = disableOptimization;
            options.packMatricesInRowMajor = true;
            if (language == EShadingLanguage.SpirV)
            {
                options.shiftAllUABuffersBindings = 20;
                options.shiftAllSamplersBindings = 40;
                options.shiftAllTexturesBindings = 60;
            }
            TargetDesc target = new TargetDesc {
                version = "450",
                language = language
            };
            Compile(ref source, ref options, ref target, out desc4);
            if (desc4.hasError)
            {
                string str = Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc4.errorWarningMsg), GetShaderConductorBlobSize(desc4.errorWarningMsg));
                throw new Exception("Translation error: " + str);
            }
            if (desc4.isText)
            {
                str2 = Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc4.target), GetShaderConductorBlobSize(desc4.target));
                str2 = TranslationFixes(language, stage, 450, str2);
            }
            else
            {
                ResultDesc desc8;
                DisassembleDesc desc9 = new DisassembleDesc {
                    language = language,
                    binary = GetShaderConductorBlobData(desc4.target),
                    binarySize = GetShaderConductorBlobSize(desc4.target)
                };
                Disassemble(ref desc9, out desc8);
                str2 = Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc8.target), GetShaderConductorBlobSize(desc8.target));
                DestroyShaderConductorBlob(desc8.target);
                DestroyShaderConductorBlob(desc8.errorWarningMsg);
            }
            DestroyShaderConductorBlob(desc4.target);
            DestroyShaderConductorBlob(desc4.errorWarningMsg);
            return str2;
        }

        public static byte[] HLSLToBinaryDxil(string hlslSource, string entryPoint, in EShaderStage stage, in bool keepDebugInfo = false, in bool disableOptimization = false)
        {
            ResultDesc desc4;
            SourceDesc source = new SourceDesc
            {
                stage = stage,
                source = hlslSource,
                entryPoint = entryPoint
            };
            OptionsDesc options = OptionsDesc.Default;
            options.shaderModel = new ShaderModel(6, 2);
            options.enable16bitTypes = true;
            options.enableDebugInfo = keepDebugInfo;
            options.optimizationLevel = 3;
            options.disableOptimizations = disableOptimization;
            options.packMatricesInRowMajor = true;
            TargetDesc target = new TargetDesc
            {
                version = "450",
                language = EShadingLanguage.Dxil
            };
            Compile(ref source, ref options, ref target, out desc4);
            if (desc4.hasError)
            {
                string str = Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc4.errorWarningMsg), GetShaderConductorBlobSize(desc4.errorWarningMsg));
                throw new Exception("Translation error: " + str);
            }
            int blobSize = GetShaderConductorBlobSize(desc4.target);
            byte[] destination = new byte[blobSize];
            Marshal.Copy(GetShaderConductorBlobData(desc4.target), destination, 0, blobSize);
            DestroyShaderConductorBlob(desc4.target);
            DestroyShaderConductorBlob(desc4.errorWarningMsg);
            return destination;
        }

        public static ShaderConductorBlob HLSLToNativeDxil(string hlslSource, string entryPoint, in EShaderStage stage, in bool keepDebugInfo = false, in bool disableOptimization = true)
        {
            ResultDesc desc4;
            SourceDesc source = new SourceDesc
            {
                stage = stage,
                source = hlslSource,
                entryPoint = entryPoint
            };
            OptionsDesc options = OptionsDesc.Default;
            options.shaderModel = new ShaderModel(6, 2);
            options.enable16bitTypes = true;
            options.enableDebugInfo = keepDebugInfo;
            options.optimizationLevel = 3;
            options.disableOptimizations = disableOptimization;
            options.packMatricesInRowMajor = true;
            TargetDesc target = new TargetDesc
            {
                version = "450",
                language = EShadingLanguage.Dxil
            };
            Compile(ref source, ref options, ref target, out desc4);
            if (desc4.hasError)
            {
                string str = Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc4.errorWarningMsg), GetShaderConductorBlobSize(desc4.errorWarningMsg));
                throw new Exception("Translation error: " + str);
            }
            //ShaderConductorBlob shaderBlob = new ShaderConductorBlob(desc4);
            //DestroyShaderConductorBlob(desc4.target);
            //DestroyShaderConductorBlob(desc4.errorWarningMsg);
            //return shaderBlob;
            return new ShaderConductorBlob(desc4);

            /*Vortice.Dxc.DxcCompilerOptions dxcOption = new Vortice.Dxc.DxcCompilerOptions();
            dxcOption.Enable16bitTypes = true;
            dxcOption.EnableDebugInfo = keepDebugInfo;
            dxcOption.OptimizationLevel = disableOptimization ? 0 : 3;
            dxcOption.PackMatrixRowMajor = true;
            dxcOption.ShaderModel = Vortice.Dxc.DxcShaderModel.Model6_2;
            Vortice.Dxc.IDxcBlob dxcBlob = Vortice.Dxc.DxcCompiler.Compile(stage.ToSDxcShaderStage(), hlslSource, entryPoint, dxcOption).GetOutput(Vortice.Dxc.DxcOutKind.Object);
            return new ShaderConductorBlob(dxcBlob.BufferSize, dxcBlob.BufferPointer);*/
        }

        public static byte[] HLSLToBinarySpirV(string hlslSource, string entryPoint, in EShaderStage stage, in bool keepDebugInfo = false, in bool disableOptimization = false)
        {
            ResultDesc desc4;
            SourceDesc source = new SourceDesc {
                stage = stage,
                source = hlslSource,
                entryPoint = entryPoint
            };
            OptionsDesc options = OptionsDesc.Default;
            options.shaderModel = new ShaderModel(6, 2);
            options.enable16bitTypes = true;
            options.enableDebugInfo = keepDebugInfo;
            options.optimizationLevel = 3;
            options.disableOptimizations = disableOptimization;
            options.packMatricesInRowMajor = true;
            options.shiftAllUABuffersBindings = 20;
            options.shiftAllSamplersBindings = 40;
            options.shiftAllTexturesBindings = 60;
            TargetDesc target = new TargetDesc {
                version = "450",
                language = EShadingLanguage.SpirV
            };
            Compile(ref source, ref options, ref target, out desc4);
            if (desc4.hasError)
            {
                string str = Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc4.errorWarningMsg), GetShaderConductorBlobSize(desc4.errorWarningMsg));
                throw new Exception("Translation error: " + str);
            }
            int blobSize = GetShaderConductorBlobSize(desc4.target);
            //int num2 = blobSize + (blobSize % 4);
            byte[] destination = new byte[blobSize];
            Marshal.Copy(GetShaderConductorBlobData(desc4.target), destination, 0, blobSize);
            /*for (int i = shaderConductorBlobSize; i < num2; ++i)
            {
                destination[i] = 0;
            }*/
            DestroyShaderConductorBlob(desc4.target);
            DestroyShaderConductorBlob(desc4.errorWarningMsg);
            return destination;
        }

        public static ShaderConductorBlob HLSLToNativeSpirV(string hlslSource, string entryPoint, in EShaderStage stage, in bool keepDebugInfo = false, in bool disableOptimization = false)
        {
            ResultDesc desc4;
            SourceDesc source = new SourceDesc
            {
                stage = stage,
                source = hlslSource,
                entryPoint = entryPoint
            };
            OptionsDesc options = OptionsDesc.Default;
            options.shaderModel = new ShaderModel(6, 2);
            options.enable16bitTypes = true;
            options.enableDebugInfo = keepDebugInfo;
            options.optimizationLevel = 3;
            options.disableOptimizations = disableOptimization;
            options.packMatricesInRowMajor = true;
            options.shiftAllUABuffersBindings = 20;
            options.shiftAllSamplersBindings = 40;
            options.shiftAllTexturesBindings = 60;
            TargetDesc target = new TargetDesc
            {
                version = "450",
                language = EShadingLanguage.SpirV
            };
            Compile(ref source, ref options, ref target, out desc4);
            if (desc4.hasError)
            {
                string str = Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc4.errorWarningMsg), GetShaderConductorBlobSize(desc4.errorWarningMsg));
                throw new Exception("Translation error: " + str);
            }
            /*ShaderConductorBlob shaderBlob = new ShaderConductorBlob(desc4);
            DestroyShaderConductorBlob(desc4.target);
            DestroyShaderConductorBlob(desc4.errorWarningMsg);
            return shaderBlob;*/
            return new ShaderConductorBlob(desc4);
        }

        public static EShaderStage ToShaderConductorStage(this Graphics.EShaderStage stage)
        {
            switch (stage)
            {
                case Graphics.EShaderStage.Vertex:
                    return EShaderStage.Vertex;

                case Graphics.EShaderStage.Fragment:
                    return EShaderStage.Fragment;

                case Graphics.EShaderStage.Compute:
                    return EShaderStage.Compute;

                default:
                    return EShaderStage.Fragment;
            }
            throw new Exception($"Stage:{stage} not supported");
        }

        /*public static Vortice.Dxc.DxcShaderStage ToSDxcShaderStage(this EShaderStage stage)
        {
            switch (stage)
            {
                case EShaderStage.Vertex:
                    return Vortice.Dxc.DxcShaderStage.Vertex;

                case EShaderStage.Fragment:
                    return Vortice.Dxc.DxcShaderStage.Pixel;

                case EShaderStage.Compute:
                    return Vortice.Dxc.DxcShaderStage.Compute;

                default:
                    return Vortice.Dxc.DxcShaderStage.Pixel;
            }
            throw new Exception($"Stage:{stage} not supported");
        }*/

        private static string TranslationFixes(EShadingLanguage language, EShaderStage stage, int version, string translation)
        {
            string input = translation;
            if (language == EShadingLanguage.Essl)
            {
                if (version < 310)
                {
                    if (stage == EShaderStage.Vertex)
                    {
                        input = input.Replace("out_var_", "var_");
                    }
                    if (stage == EShaderStage.Fragment)
                    {
                        input = input.Replace("in_var_", "var_");
                    }
                }
                input = Regex.Replace(input, @"(findLSB\(.*\))", "uint($1)");
            }
            return input;
        }
    }
}

