using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Infinity.Shaderlib
{
    public static class CrossCompiler
    {
        public static string DisassemblySPIRV(byte[] bytecode)
        {
            ShaderConductorWrapper.ResultDesc desc2;
            IntPtr destination = Marshal.AllocHGlobal((int) bytecode.Length);
            Marshal.Copy(bytecode, 0, destination, bytecode.Length);
            ShaderConductorWrapper.DisassembleDesc source = new ShaderConductorWrapper.DisassembleDesc {
                language = ShaderConductorWrapper.EShaderLanguage.SpirV,
                binary = destination,
                binarySize = bytecode.Length
            };
            ShaderConductorWrapper.Disassemble(ref source, out desc2);
            Marshal.FreeHGlobal(destination);
            ShaderConductorWrapper.DestroyShaderConductorBlob(desc2.target);
            ShaderConductorWrapper.DestroyShaderConductorBlob(desc2.errorWarningMsg);
            return Marshal.PtrToStringAnsi(ShaderConductorWrapper.GetShaderConductorBlobData(desc2.target), ShaderConductorWrapper.GetShaderConductorBlobSize(desc2.target));
        }

        public static string HLSLTo(string hlslSource, string entryPoint, in Graphics.EShaderStage stage, in ShaderConductorWrapper.EShaderLanguage language, in bool keepDebugInfo = false, in bool disableOptimization = false)
        {
            int version = 450;

            ShaderConductorWrapper.ResultDesc desc4;
            string str2;
            ShaderConductorWrapper.SourceDesc source = new ShaderConductorWrapper.SourceDesc {
                source = hlslSource,
                stage = stage.ToShaderConductorStage(),
                entryPoint = entryPoint
            };
            ShaderConductorWrapper.OptionsDesc options = ShaderConductorWrapper.OptionsDesc.Default;
            options.shaderModel = new ShaderConductorWrapper.ShaderModel(6, 3);
            options.enable16bitTypes = true;
            options.enableDebugInfo = keepDebugInfo;
            options.disableOptimizations = disableOptimization;
            options.optimizationLevel = 3;
            options.packMatricesInRowMajor = true;
            if (language == ShaderConductorWrapper.EShaderLanguage.SpirV)
            {
                options.shiftAllUABuffersBindings = 20;
                options.shiftAllSamplersBindings = 40;
                options.shiftAllTexturesBindings = 60;
            }
            ShaderConductorWrapper.TargetDesc target = new ShaderConductorWrapper.TargetDesc {
                language = language,
                version = (version).ToString()
            };
            ShaderConductorWrapper.Compile(ref source, ref options, ref target, out desc4);
            if (desc4.hasError)
            {
                string str = Marshal.PtrToStringAnsi(ShaderConductorWrapper.GetShaderConductorBlobData(desc4.errorWarningMsg), ShaderConductorWrapper.GetShaderConductorBlobSize(desc4.errorWarningMsg));
                throw new Exception("Translation error: " + str);
            }
            if (desc4.isText)
            {
                str2 = Marshal.PtrToStringAnsi(ShaderConductorWrapper.GetShaderConductorBlobData(desc4.target), ShaderConductorWrapper.GetShaderConductorBlobSize(desc4.target));
                //str2 = TranslationFixes(language, stage, version, str2);
            }
            else
            {
                ShaderConductorWrapper.ResultDesc desc8;
                ShaderConductorWrapper.DisassembleDesc desc9 = new ShaderConductorWrapper.DisassembleDesc {
                    language = language,
                    binarySize = ShaderConductorWrapper.GetShaderConductorBlobSize(desc4.target),
                    binary = ShaderConductorWrapper.GetShaderConductorBlobData(desc4.target)
                };
                ShaderConductorWrapper.Disassemble(ref desc9, out desc8);
                str2 = Marshal.PtrToStringAnsi(ShaderConductorWrapper.GetShaderConductorBlobData(desc8.target), ShaderConductorWrapper.GetShaderConductorBlobSize(desc8.target));
                ShaderConductorWrapper.DestroyShaderConductorBlob(desc8.target);
                ShaderConductorWrapper.DestroyShaderConductorBlob(desc8.errorWarningMsg);
            }
            ShaderConductorWrapper.DestroyShaderConductorBlob(desc4.target);
            ShaderConductorWrapper.DestroyShaderConductorBlob(desc4.errorWarningMsg);
            return str2;
        }

        public static byte[] HLSLToBinarySpirV(string hlslSource, string entryPoint, in Graphics.EShaderStage stage, in bool keepDebugInfo = false, in bool disableOptimization = false)
        {
            ShaderConductorWrapper.ResultDesc desc4;
            ShaderConductorWrapper.SourceDesc source = new ShaderConductorWrapper.SourceDesc {
                source = hlslSource,
                stage = stage.ToShaderConductorStage(),
                entryPoint = entryPoint
            };
            ShaderConductorWrapper.OptionsDesc options = ShaderConductorWrapper.OptionsDesc.Default;
            options.shaderModel = new ShaderConductorWrapper.ShaderModel(6, 3);
            options.enable16bitTypes = true;
            options.enableDebugInfo = keepDebugInfo;
            options.disableOptimizations = disableOptimization;
            options.optimizationLevel = 3;
            options.packMatricesInRowMajor = true;
            options.shiftAllUABuffersBindings = 20;
            options.shiftAllSamplersBindings = 40;
            options.shiftAllTexturesBindings = 60;
            ShaderConductorWrapper.TargetDesc target = new ShaderConductorWrapper.TargetDesc {
                language = ShaderConductorWrapper.EShaderLanguage.SpirV,
                version = string.Empty
            };
            ShaderConductorWrapper.Compile(ref source, ref options, ref target, out desc4);
            if (desc4.hasError)
            {
                string str = Marshal.PtrToStringAnsi(ShaderConductorWrapper.GetShaderConductorBlobData(desc4.errorWarningMsg), ShaderConductorWrapper.GetShaderConductorBlobSize(desc4.errorWarningMsg));
                throw new Exception("Translation error: " + str);
            }
            int shaderConductorBlobSize = ShaderConductorWrapper.GetShaderConductorBlobSize(desc4.target);
            int num2 = shaderConductorBlobSize + (shaderConductorBlobSize % 4);
            byte[] destination = new byte[num2];
            Marshal.Copy(ShaderConductorWrapper.GetShaderConductorBlobData(desc4.target), destination, 0, shaderConductorBlobSize);
            for (int i = shaderConductorBlobSize; i < num2; i++)
            {
                destination[i] = 0;
            }
            ShaderConductorWrapper.DestroyShaderConductorBlob(desc4.target);
            ShaderConductorWrapper.DestroyShaderConductorBlob(desc4.errorWarningMsg);
            return destination;
        }

        private static ShaderConductorWrapper.EShaderStage ToShaderConductorStage(this Graphics.EShaderStage stage)
        {
            switch (stage)
            {
                case Graphics.EShaderStage.Vertex:
                    return ShaderConductorWrapper.EShaderStage.Vertex;

                case Graphics.EShaderStage.Fragment:
                    return ShaderConductorWrapper.EShaderStage.Pixel;

                case Graphics.EShaderStage.Compute:
                    return ShaderConductorWrapper.EShaderStage.Compute;

                default:
                    return ShaderConductorWrapper.EShaderStage.Pixel;
            }
            throw new Exception($"Stage:{stage} not supported");
        }

        /*private static string TranslationFixes(ShaderConductorWrapper.ShadingLanguage language, ShaderConductorWrapper.ShaderStage stage, int version, string translation)
        {
            string input = translation;
            if (language == ShaderConductorWrapper.ShadingLanguage.Essl)
            {
                if (version < 310)
                {
                    if (stage == ShaderConductorWrapper.ShaderStage.Vertex)
                    {
                        input = input.Replace("out_var_", "var_");
                    }
                    if (stage == ShaderConductorWrapper.ShaderStage.Pixel)
                    {
                        input = input.Replace("in_var_", "var_");
                    }
                }
                input = Regex.Replace(input, @"(findLSB\(.*\))", "uint($1)");
            }
            return input;
        }*/
    }
}

