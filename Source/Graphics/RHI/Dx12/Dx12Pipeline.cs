using System.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS0169, CS0649, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
    internal unsafe class Dx12ComputePipeline : RHIComputePipeline
    {
        public Dx12PipelineLayout PipelineLayout
        {
            get
            {
                return m_PipelineLayout;
            }
        }
        public ID3D12PipelineState* NativePipelineState
        {
            get
            {
                return m_NativePipelineState;
            }
        }

        private Dx12PipelineLayout m_PipelineLayout;
        private ID3D12PipelineState* m_NativePipelineState;

        public Dx12ComputePipeline(Dx12Device device, in RHIComputePipelineCreateInfo createInfo)
        {
            m_PipelineLayout = createInfo.pipelineLayout as Dx12PipelineLayout;

            Dx12Shader computeShader = createInfo.computeShader as Dx12Shader;

            D3D12_COMPUTE_PIPELINE_STATE_DESC desc = new D3D12_COMPUTE_PIPELINE_STATE_DESC();
            desc.CS.BytecodeLength = computeShader.NativeShaderBytecode.BytecodeLength;
            desc.CS.pShaderBytecode = computeShader.NativeShaderBytecode.pShaderBytecode;
            desc.pRootSignature = m_PipelineLayout.NativeRootSignature;
            desc.Flags = D3D12_PIPELINE_STATE_FLAGS.D3D12_PIPELINE_STATE_FLAG_NONE;

            ID3D12PipelineState* pipelineState;
            bool success = SUCCEEDED(device.NativeDevice->CreateComputePipelineState(&desc, __uuidof<ID3D12PipelineState>(), (void**)&pipelineState));
            Debug.Assert(success);
            m_NativePipelineState = pipelineState;
        }

        protected override void Release()
        {
            m_NativePipelineState->Release();
        }
    }

    internal unsafe class Dx12GraphicsPipeline : RHIGraphicsPipeline
    {
        public Dx12PipelineLayout PipelineLayout
        {
            get
            {
                return m_PipelineLayout;
            }
        }
        public ID3D12PipelineState* NativePipelineState
        {
            get
            {
                return m_NativePipelineState;
            }
        }

        private Dx12PipelineLayout m_PipelineLayout;
        private ID3D12PipelineState* m_NativePipelineState;

        public Dx12GraphicsPipeline(Dx12Device device, in RHIGraphicsPipelineCreateInfo createInfo)
        {
            //m_PipelineLayout = createInfo.layout as Dx12PipelineLayout;
        }

        protected override void Release()
        {
            //m_NativePipelineState->Release();
        }
    }
#pragma warning restore CS0169, CS0649, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
}
