using System;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class Dx12Shader : RHIShader
    {
        public D3D12_SHADER_BYTECODE NativeShaderBytecode
        {
            get
            {
                return m_NativeShaderBytecode;
            }
        }

        private D3D12_SHADER_BYTECODE m_NativeShaderBytecode;

        public Dx12Shader(in RHIShaderDescriptor descriptor)
        {
            m_Descriptor = descriptor;
            m_NativeShaderBytecode = new D3D12_SHADER_BYTECODE(descriptor.byteCode.ToPointer(), new UIntPtr((uint)descriptor.size));
        }

        protected override void Release()
        {

        }
    }
}
