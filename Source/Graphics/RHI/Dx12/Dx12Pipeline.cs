using System;
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

            D3D12_COMPUTE_PIPELINE_STATE_DESC description = new D3D12_COMPUTE_PIPELINE_STATE_DESC();
            description.CS.BytecodeLength = computeShader.NativeShaderBytecode.BytecodeLength;
            description.CS.pShaderBytecode = computeShader.NativeShaderBytecode.pShaderBytecode;
            description.pRootSignature = m_PipelineLayout.NativeRootSignature;
            description.Flags = D3D12_PIPELINE_STATE_FLAGS.D3D12_PIPELINE_STATE_FLAG_NONE;

            ID3D12PipelineState* pipelineState;
            bool success = SUCCEEDED(device.NativeDevice->CreateComputePipelineState(&description, __uuidof<ID3D12PipelineState>(), (void**)&pipelineState));
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
        public int StencilRef
        {
            get
            {
                return m_StencilRef;
            }
        }
        public int[] VertexStrides
        {
            get
            {
                return m_VertexStrides;
            }
        }
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
        public D3D_PRIMITIVE_TOPOLOGY PrimitiveTopology
        {
            get
            {
                return m_PrimitiveTopology;
            }
        }

        private int m_StencilRef;
        private bool m_ScissorEnabled;
        private int[] m_VertexStrides;
        private Dx12PipelineLayout m_PipelineLayout;
        private ID3D12PipelineState* m_NativePipelineState;
        private D3D_PRIMITIVE_TOPOLOGY m_PrimitiveTopology;
        private D3D12_PRIMITIVE_TOPOLOGY_TYPE m_PrimitiveTopologyType;

        public Dx12GraphicsPipeline(Dx12Device device, in RHIGraphicsPipelineCreateInfo createInfo)
        {
            m_PipelineLayout = createInfo.pipelineLayout as Dx12PipelineLayout;

            Dx12Shader vertexShader = createInfo.vertexShader as Dx12Shader;
            Dx12Shader fragmentShader = createInfo.fragmentShader as Dx12Shader;
            Span<RHIVertexLayout> vertexLayouts = createInfo.vertexState.vertexLayouts.Span;

            if ((vertexShader != null))
            {
                m_VertexStrides = new int[vertexLayouts.Length];
                for (int j = 0; j < vertexLayouts.Length; ++j)
                {
                    m_VertexStrides[j] = (int)vertexLayouts[j].stride;
                }
            }

            m_StencilRef = createInfo.fragmentState.stencilRef;
            m_ScissorEnabled = createInfo.fragmentState.rasterizerState.scissorEnable;
            m_PrimitiveTopology = Dx12Utility.ConvertToDX12PrimitiveTopology(createInfo.vertexState.primitiveTopology);
            m_PrimitiveTopologyType = Dx12Utility.ConvertToDX12PrimitiveTopologyType(createInfo.vertexState.primitiveTopology);

            D3D12_GRAPHICS_PIPELINE_STATE_DESC description = new D3D12_GRAPHICS_PIPELINE_STATE_DESC
            {
                InputLayout = Dx12Utility.ConvertToDX12VertexLayout(vertexLayouts),
                pRootSignature = m_PipelineLayout.NativeRootSignature,
                PrimitiveTopologyType = m_PrimitiveTopologyType,

                SampleMask = createInfo.fragmentState.sampleMask.HasValue ? ((uint)createInfo.fragmentState.sampleMask.Value) : uint.MaxValue,
                BlendState = Dx12Utility.CreateDx12BlendState(createInfo.fragmentState.blendState),
                RasterizerState = Dx12Utility.CreateDx12RasterizerState(createInfo.fragmentState.rasterizerState, createInfo.outputState.sampleCount != ESampleCount.None),
                DepthStencilState = Dx12Utility.CreateDx12DepthStencilState(createInfo.fragmentState.depthStencilState)
            };

            if (createInfo.outputState.depthAttachment.HasValue)
            {
                description.DSVFormat = Dx12Utility.ConvertToDx12Format(createInfo.outputState.depthAttachment.Value.format);
            }

            for (int i = 0; i < createInfo.outputState.colorAttachments.Length; ++i)
            {
                description.RTVFormats[i] = Dx12Utility.ConvertToDx12Format(createInfo.outputState.colorAttachments.Span[i].format);
            }

            description.Flags = D3D12_PIPELINE_STATE_FLAGS.D3D12_PIPELINE_STATE_FLAG_NONE;
            description.SampleDesc = Dx12Utility.ConvertToDx12SampleCount(createInfo.outputState.sampleCount);
            //description.StreamOutput = new StreamOutputDescription();

            if (createInfo.vertexShader != null)
            {
                description.VS.BytecodeLength = vertexShader.NativeShaderBytecode.BytecodeLength;
                description.VS.pShaderBytecode = vertexShader.NativeShaderBytecode.pShaderBytecode;
            }

            if (createInfo.fragmentShader != null)
            {
                description.PS.BytecodeLength = fragmentShader.NativeShaderBytecode.BytecodeLength;
                description.PS.pShaderBytecode = fragmentShader.NativeShaderBytecode.pShaderBytecode;
            }

            ID3D12PipelineState* pipelineState;
            bool success = SUCCEEDED(device.NativeDevice->CreateGraphicsPipelineState(&description, __uuidof<ID3D12PipelineState>(), (void**)&pipelineState));
            Debug.Assert(success);
            m_NativePipelineState = pipelineState;
        }

        protected override void Release()
        {
            m_NativePipelineState->Release();
        }
    }
#pragma warning restore CS0169, CS0649, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
}
