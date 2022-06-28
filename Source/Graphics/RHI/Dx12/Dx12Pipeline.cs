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

        public Dx12ComputePipeline(Dx12Device device, in RHIComputePipelineDescriptor descriptor)
        {
            m_PipelineLayout = descriptor.pipelineLayout as Dx12PipelineLayout;

            Dx12Shader computeShader = descriptor.computeShader as Dx12Shader;

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

        public Dx12GraphicsPipeline(Dx12Device device, in RHIGraphicsPipelineDescriptor descriptor)
        {
            m_PipelineLayout = descriptor.pipelineLayout as Dx12PipelineLayout;

            Dx12Shader vertexShader = descriptor.vertexShader as Dx12Shader;
            Dx12Shader fragmentShader = descriptor.fragmentShader as Dx12Shader;
            Span<RHIVertexLayoutDescriptor> vertexLayoutDescriptors = descriptor.vertexStateDescriptor.vertexLayoutDescriptors.Span;

            if ((vertexShader != null))
            {
                m_VertexStrides = new int[vertexLayoutDescriptors.Length];
                for (int j = 0; j < vertexLayoutDescriptors.Length; ++j)
                {
                    m_VertexStrides[j] = vertexLayoutDescriptors[j].stride;
                }
            }

            m_StencilRef = descriptor.renderStateDescriptor.stencilRef;
            m_ScissorEnabled = descriptor.renderStateDescriptor.rasterizerStateDescriptor.scissorEnable;
            m_PrimitiveTopology = Dx12Utility.ConvertToDx12PrimitiveTopology(descriptor.vertexStateDescriptor.primitiveTopology);
            m_PrimitiveTopologyType = Dx12Utility.ConvertToDx12PrimitiveTopologyType(descriptor.vertexStateDescriptor.primitiveTopology);

            int inputElementCount = Dx12Utility.GetDx12VertexLayoutCount(vertexLayoutDescriptors);
            D3D12_INPUT_ELEMENT_DESC* inputElementsPtr = stackalloc D3D12_INPUT_ELEMENT_DESC[inputElementCount];
            Span<D3D12_INPUT_ELEMENT_DESC> inputElementsView = new Span<D3D12_INPUT_ELEMENT_DESC>(inputElementsPtr, inputElementCount);

            Dx12Utility.ConvertToDx12VertexLayout(vertexLayoutDescriptors, inputElementsView);

            D3D12_INPUT_LAYOUT_DESC outputLayout;
            outputLayout.NumElements = (uint)inputElementCount;
            outputLayout.pInputElementDescs = inputElementsPtr;

            D3D12_GRAPHICS_PIPELINE_STATE_DESC description = new D3D12_GRAPHICS_PIPELINE_STATE_DESC
            {
                InputLayout = outputLayout,
                pRootSignature = m_PipelineLayout.NativeRootSignature,
                PrimitiveTopologyType = m_PrimitiveTopologyType,

                SampleMask = descriptor.renderStateDescriptor.sampleMask.HasValue ? ((uint)descriptor.renderStateDescriptor.sampleMask.Value) : uint.MaxValue,
                BlendState = Dx12Utility.CreateDx12BlendState(descriptor.renderStateDescriptor.blendStateDescriptor),
                RasterizerState = Dx12Utility.CreateDx12RasterizerState(descriptor.renderStateDescriptor.rasterizerStateDescriptor, descriptor.outputStateDescriptor.sampleCount != ESampleCount.None),
                DepthStencilState = Dx12Utility.CreateDx12DepthStencilState(descriptor.renderStateDescriptor.depthStencilStateDescriptor)
            };

            if (descriptor.outputStateDescriptor.depthAttachmentDescriptor.HasValue)
            {
                description.DSVFormat = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT;
                //description.DSVFormat = Dx12Utility.ConvertToDx12Format(descriptor.outputState.depthAttachment.Value.format);
            }

            for (int i = 0; i < descriptor.outputStateDescriptor.colorAttachmentDescriptors.Length; ++i)
            {
                description.RTVFormats[i] = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                //description.RTVFormats[i] = Dx12Utility.ConvertToDx12Format(descriptor.outputState.colorAttachments.Span[i].format);
            }

            description.Flags = D3D12_PIPELINE_STATE_FLAGS.D3D12_PIPELINE_STATE_FLAG_NONE;
            description.NumRenderTargets = (uint)descriptor.outputStateDescriptor.colorAttachmentDescriptors.Length;
            description.SampleDesc = Dx12Utility.ConvertToDx12SampleCount(descriptor.outputStateDescriptor.sampleCount);
            //description.StreamOutput = new StreamOutputDescription();

            if (descriptor.vertexShader != null)
            {
                description.VS.BytecodeLength = vertexShader.NativeShaderBytecode.BytecodeLength;
                description.VS.pShaderBytecode = vertexShader.NativeShaderBytecode.pShaderBytecode;
            }

            if (descriptor.fragmentShader != null)
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
