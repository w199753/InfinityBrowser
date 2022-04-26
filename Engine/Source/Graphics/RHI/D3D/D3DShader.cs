using System;
using Vortice.Dxc;
using Vortice.Direct3D12.Shader;

namespace InfinityEngine.Graphics.RHI.D3D
{
    internal class D3DShaderCompiler
    {
        private static Span<byte> CompileBytecode(DxcShaderStage stage, string shaderSource, string entryPoint)
        {
            IDxcResult results = DxcCompiler.Compile(stage, shaderSource, entryPoint, null);
            return results.GetObjectBytecode();
        }

        private static Span<byte> CompileBytecodeWithReflection(DxcShaderStage stage, string shaderSource, string entryPoint, out ID3D12ShaderReflection reflection)
        {
            IDxcResult results = DxcCompiler.Compile(stage, shaderSource, entryPoint, null, null, null, null);
            using (IDxcBlob reflectionData = results.GetOutput(DxcOutKind.Reflection))
            {
                reflection = DxcCompiler.Utils.CreateReflection<ID3D12ShaderReflection>(reflectionData);
            }

            return results.GetObjectBytecode();
        }
    }

    public class D3DShader : RHIShader
    {
        public D3DShader() : base()
        {

        }

        protected override void Release()
        {

        }
    }

    public class D3DComputeShader : RHIComputeShader
    {
        public D3DComputeShader() : base()
        {

        }

        protected override void Release()
        {
            base.Release();
        }
    }

    public class D3DGraphicsShader : RHIGraphicsShader
    {
        public D3DGraphicsShader() : base()
        {

        }

        protected override void Release()
        {
            base.Release();
        }
    }

    public class D3DRayTraceShader : RHIRayTraceShader
    {
        //Intersection, AnyHit, ClosestHit, Miss, RayGeneration
        public D3DRayTraceShader() : base()
        {

        }

        protected override void Release()
        {
            base.Release();
        }
    }
}
