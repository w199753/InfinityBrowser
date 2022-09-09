using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static Infinity.Shaderlib.ShaderConductorWrapper;

namespace Infinity.Shaderlib
{
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

        public static string HLSLTo(string hlslSource, string entryPoint, in EShaderStage stage, in EShadingLanguage language, int version = 470, in bool keepDebugInfo = false, in bool disableOptimization = false)
        {
            ResultDesc desc4;
            string str2;
            SourceDesc source = new SourceDesc {
                source = hlslSource,
                stage = stage,
                entryPoint = entryPoint
            };
            OptionsDesc options = OptionsDesc.Default;
            options.shaderModel = new ShaderModel(6, 6);
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
                language = language,
                version = (version).ToString()
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
                str2 = TranslationFixes(language, stage, version, str2);
            }
            else
            {
                ResultDesc desc8;
                DisassembleDesc desc9 = new DisassembleDesc {
                    language = language,
                    binarySize = GetShaderConductorBlobSize(desc4.target),
                    binary = GetShaderConductorBlobData(desc4.target)
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

        public static byte[] HLSLToBinarySpirV(string hlslSource, string entryPoint, in EShaderStage stage, in bool keepDebugInfo = false, in bool disableOptimization = false)
        {
            ResultDesc desc4;
            SourceDesc source = new SourceDesc {
                source = hlslSource,
                stage = stage,
                entryPoint = entryPoint
            };
            OptionsDesc options = OptionsDesc.Default;
            options.shaderModel = new ShaderModel(6, 3);
            options.enable16bitTypes = true;
            options.enableDebugInfo = keepDebugInfo;
            options.disableOptimizations = disableOptimization;
            options.optimizationLevel = 3;
            options.packMatricesInRowMajor = true;
            options.shiftAllUABuffersBindings = 20;
            options.shiftAllSamplersBindings = 40;
            options.shiftAllTexturesBindings = 60;
            TargetDesc target = new TargetDesc {
                language = EShadingLanguage.SpirV,
                version = string.Empty
            };
            Compile(ref source, ref options, ref target, out desc4);
            if (desc4.hasError)
            {
                string str = Marshal.PtrToStringAnsi(GetShaderConductorBlobData(desc4.errorWarningMsg), GetShaderConductorBlobSize(desc4.errorWarningMsg));
                throw new Exception("Translation error: " + str);
            }
            int shaderConductorBlobSize = GetShaderConductorBlobSize(desc4.target);
            int num2 = shaderConductorBlobSize + (shaderConductorBlobSize % 4);
            byte[] destination = new byte[num2];
            Marshal.Copy(GetShaderConductorBlobData(desc4.target), destination, 0, shaderConductorBlobSize);
            for (int i = shaderConductorBlobSize; i < num2; i++)
            {
                destination[i] = 0;
            }
            DestroyShaderConductorBlob(desc4.target);
            DestroyShaderConductorBlob(desc4.errorWarningMsg);
            return destination;
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

