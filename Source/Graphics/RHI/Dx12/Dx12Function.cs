using System;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class Dx12Function : RHIFunction
    {
        public D3D12_SHADER_BYTECODE NativeShaderBytecode
        {
            get
            {
                return m_NativeShaderBytecode;
            }
        }

        private D3D12_SHADER_BYTECODE m_NativeShaderBytecode;

        public Dx12Function(in RHIFunctionDescriptor descriptor)
        {
            m_Descriptor = descriptor;
            m_NativeShaderBytecode = new D3D12_SHADER_BYTECODE(descriptor.ByteCode.ToPointer(), new UIntPtr(descriptor.ByteSize));
        }

        protected override void Release()
        {

        }
    }

    internal unsafe class Dx12FunctionTable : RHIFunctionTable
    {
        public Dx12FunctionTable(Dx12RaytracingPipeline rayTracingPipeline)
        {

        }

        public override void SetRayGenerationShader(string exportName, RHIBindGroup bindGroup)
        {

        }

        public override void AddMissShader(string exportName, RHIBindGroup bindGroup)
        {

        }

        public override void AddHitGroup(string exportName, RHIBindGroup bindGroup)
        {

        }

        public override void AddCallableShader(string exportName, RHIBindGroup bindGroup)
        {

        }

        public override void ClearMissShaders()
        {

        }

        public override void ClearHitShaders()
        {

        }

        public override void ClearCallableShaders()
        {

        }

    }
}
