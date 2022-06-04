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

        public Dx12Shader(in RHIShaderCreateInfo createInfo)
        {
            m_NativeShaderBytecode = new D3D12_SHADER_BYTECODE(createInfo.byteCode.ToPointer(), new UIntPtr((uint)createInfo.size));
        }

        protected override void Release()
        {

        }
    }
}
