using System.Diagnostics;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS0169, CS0649, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
    internal unsafe class D3DComputePipeline : RHIComputePipeline
    {
        public D3DPipelineLayout PipelineLayout
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

        private D3DPipelineLayout m_PipelineLayout;
        private ID3D12PipelineState* m_NativePipelineState;

        public D3DComputePipeline(D3DDevice device, in RHIComputePipelineCreateInfo createInfo)
        {
            m_PipelineLayout = createInfo.layout as D3DPipelineLayout;

            D3DShader computeShader = createInfo.computeShader as D3DShader;

            D3D12_COMPUTE_PIPELINE_STATE_DESC desc = new D3D12_COMPUTE_PIPELINE_STATE_DESC();
            desc.CS = computeShader.NativeShaderBytecode;
            desc.pRootSignature = m_PipelineLayout.NativeRootSignature;

            ID3D12PipelineState* pipelineState;
            bool success = SUCCEEDED(device.NativeDevice->CreateComputePipelineState(&desc, __uuidof<ID3D12RootSignature>(), (void**)&pipelineState));
            Debug.Assert(success);
            m_NativePipelineState = pipelineState;
        }

        protected override void Release()
        {
            m_NativePipelineState->Release();
        }
    }

    internal unsafe class D3DGraphicsPipeline : RHIGraphicsPipeline
    {
        public D3DPipelineLayout PipelineLayout
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

        private D3DPipelineLayout m_PipelineLayout;
        private ID3D12PipelineState* m_NativePipelineState;

        public D3DGraphicsPipeline(D3DDevice device, in RHIGraphicsPipelineCreateInfo createInfo)
        {
            //m_PipelineLayout = createInfo.layout as D3DPipelineLayout;
        }

        protected override void Release()
        {
            //m_NativePipelineState->Release();
        }
    }
#pragma warning restore CS0169, CS0649, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
}
