using System;
using System.Diagnostics;
using Infinity.Mathmatics;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS0169, CS0649, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12BindTypeAndParameterSlot
    {
        public int slot;
        public EBindType bindType;
    }

    internal unsafe class Dx12PipelineLayout : RHIPipelineLayout
    {
        public ID3D12RootSignature* NativeRootSignature
        {
            get
            {
                return m_NativeRootSignature;
            }
        }

        private ID3D12RootSignature* m_NativeRootSignature;
        private Dictionary<int, Dx12BindTypeAndParameterSlot> m_VertexParameterMap;
        private Dictionary<int, Dx12BindTypeAndParameterSlot> m_FragmentParameterMap;
        private Dictionary<int, Dx12BindTypeAndParameterSlot> m_ComputeParameterMap;

        public Dx12PipelineLayout(Dx12Device device, in RHIPipelineLayoutDescriptor descriptor)
        {
            m_VertexParameterMap = new Dictionary<int, Dx12BindTypeAndParameterSlot>(8);
            m_FragmentParameterMap = new Dictionary<int, Dx12BindTypeAndParameterSlot>(8);
            m_ComputeParameterMap = new Dictionary<int, Dx12BindTypeAndParameterSlot>(8);

            int parameterCount = 0;
            for (int i = 0; i < descriptor.bindGroupLayouts.Length; ++i)
            {
                Dx12BindGroupLayout bindGroupLayout = descriptor.bindGroupLayouts[i] as Dx12BindGroupLayout;
                parameterCount += bindGroupLayout.BindInfos.Length;
            }

            D3D12_DESCRIPTOR_RANGE1* rootDescriptorRangePtr = stackalloc D3D12_DESCRIPTOR_RANGE1[parameterCount];
            Span<D3D12_DESCRIPTOR_RANGE1> rootDescriptorRangeViews = new Span<D3D12_DESCRIPTOR_RANGE1>(rootDescriptorRangePtr, parameterCount);

            D3D12_ROOT_PARAMETER1* rootParameterPtr = stackalloc D3D12_ROOT_PARAMETER1[parameterCount];
            Span<D3D12_ROOT_PARAMETER1> rootParameterViews = new Span<D3D12_ROOT_PARAMETER1>(rootParameterPtr, parameterCount);

            for (int i = 0; i < descriptor.bindGroupLayouts.Length; ++i)
            {
                Dx12BindGroupLayout bindGroupLayout = descriptor.bindGroupLayouts[i] as Dx12BindGroupLayout;

                for (int j = 0; j < bindGroupLayout.BindInfos.Length; ++j)
                {
                    ref Dx12BindInfo bindInfo = ref bindGroupLayout.BindInfos[j];

                    ref D3D12_DESCRIPTOR_RANGE1 rootDescriptorRange = ref rootDescriptorRangeViews[i + j];
                    rootDescriptorRange.Init(Dx12Utility.ConvertToDx12BindType(bindInfo.bindType), bindInfo.Bindless ? (uint)bindInfo.count : 1, (uint)bindInfo.slot, (uint)bindInfo.index, Dx12Utility.GetDx12DescriptorRangeFalag(bindInfo.bindType));

                    ref D3D12_ROOT_PARAMETER1 rootParameterView = ref rootParameterViews[i + j];
                    rootParameterView.InitAsDescriptorTable(1, rootDescriptorRangePtr + (i + j), Dx12Utility.ConvertToDx12ShaderStage(bindInfo.shaderStage));

                    Dx12BindTypeAndParameterSlot parameter;
                    parameter.slot = i + j;
                    parameter.bindType = bindInfo.bindType;

                    if ((bindInfo.shaderStage & EShaderStage.Vertex) == EShaderStage.Vertex)
                    {
                        m_VertexParameterMap.TryAdd(new int3(bindInfo.index << 8, bindInfo.slot, Dx12Utility.GetDx12BindKey(bindInfo.bindType)).GetHashCode(), parameter);
                    }

                    if ((bindInfo.shaderStage & EShaderStage.Fragment) == EShaderStage.Fragment)
                    {
                        m_FragmentParameterMap.TryAdd(new int3(bindInfo.index << 8, bindInfo.slot, Dx12Utility.GetDx12BindKey(bindInfo.bindType)).GetHashCode(), parameter);
                    }

                    if ((bindInfo.shaderStage & EShaderStage.Compute) == EShaderStage.Compute)
                    {
                        m_ComputeParameterMap.TryAdd(new int3(bindInfo.index << 8, bindInfo.slot, Dx12Utility.GetDx12BindKey(bindInfo.bindType)).GetHashCode(), parameter);
                    }
                }
            }

            D3D12_VERSIONED_ROOT_SIGNATURE_DESC rootSignatureDesc = new D3D12_VERSIONED_ROOT_SIGNATURE_DESC();
            rootSignatureDesc.Init_1_1((uint)parameterCount, rootParameterPtr, 0, null, D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT);

            ID3DBlob* signature;
            Dx12Utility.CHECK_HR(DirectX.D3D12SerializeVersionedRootSignature(&rootSignatureDesc, D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_1, &signature, null));

            ID3D12RootSignature* rootSignature;
            Dx12Utility.CHECK_HR(device.NativeDevice->CreateRootSignature(0, signature->GetBufferPointer(), signature->GetBufferSize(), __uuidof<ID3D12RootSignature>(), (void**)&rootSignature));
            m_NativeRootSignature = rootSignature;
        }

        public Dx12BindTypeAndParameterSlot? QueryRootDescriptorParameterIndex(in EShaderStage shaderStage, in int layoutIndex, in int slot, in EBindType bindType)
        {
            bool hasValue = false;
            Dx12BindTypeAndParameterSlot? outParameter = null;

            if ((shaderStage & EShaderStage.Vertex) == EShaderStage.Vertex)
            {
                //hasValue = m_VertexParameterMap.TryGetValue(new int2(slot, Dx12Utility.GetDx12BindKey(bindType)).GetHashCode(), out Dx12BindTypeAndParameterSlot parameter);
                hasValue = m_VertexParameterMap.TryGetValue(new int3(layoutIndex << 8, slot, Dx12Utility.GetDx12BindKey(bindType)).GetHashCode(), out Dx12BindTypeAndParameterSlot parameter);
                outParameter = parameter;
            }

            if ((shaderStage & EShaderStage.Fragment) == EShaderStage.Fragment)
            {
                //hasValue = m_FragmentParameterMap.TryGetValue(new int2(slot, Dx12Utility.GetDx12BindKey(bindType)).GetHashCode(), out Dx12BindTypeAndParameterSlot parameter);
                hasValue = m_FragmentParameterMap.TryGetValue(new int3(layoutIndex << 8, slot, Dx12Utility.GetDx12BindKey(bindType)).GetHashCode(), out Dx12BindTypeAndParameterSlot parameter);
                outParameter = parameter;
            }

            if ((shaderStage & EShaderStage.Compute) == EShaderStage.Compute)
            {
                //hasValue = m_ComputeParameterMap.TryGetValue(new int2(slot, Dx12Utility.GetDx12BindKey(bindType)).GetHashCode(), out Dx12BindTypeAndParameterSlot parameter);
                hasValue = m_ComputeParameterMap.TryGetValue(new int3(layoutIndex << 8, slot, Dx12Utility.GetDx12BindKey(bindType)).GetHashCode(), out Dx12BindTypeAndParameterSlot parameter);
                outParameter = parameter;
            }

            return hasValue ? outParameter : null;
        }

        protected override void Release()
        {
            m_NativeRootSignature->Release();
        }
    }

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
            RHIPipelineLayoutDescriptor pipelienLayoutDescriptor;
            {
                pipelienLayoutDescriptor.bindGroupLayouts = descriptor.bindGroupLayouts;
            }
            m_PipelineLayout = device.CreatePipelineLayout(pipelienLayoutDescriptor) as Dx12PipelineLayout;

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
            m_PipelineLayout.Dispose();
            m_NativePipelineState->Release();
        }
    }

    internal unsafe class Dx12RaytracingPipeline : RHIRaytracingPipeline
    {
        public Dx12PipelineLayout GlobalPipelineLayout
        {
            get
            {
                return m_GlobalPipelineLayout;
            }
        }
        public ID3D12PipelineState* NativePipelineState
        {
            get
            {
                return m_NativePipelineState;
            }
        }

        private Dx12PipelineLayout m_GlobalPipelineLayout;
        private ID3D12PipelineState* m_NativePipelineState;
        private Dictionary<RHIBindGroupLayout, Dx12PipelineLayout> m_LocalPipelineLayoutMap;

        public Dx12RaytracingPipeline(Dx12Device device, in RHIRaytracingPipelineDescriptor descriptor)
        {

        }

        protected override void Release()
        {
            m_GlobalPipelineLayout.Dispose();
            m_NativePipelineState->Release();

            foreach (KeyValuePair<RHIBindGroupLayout, Dx12PipelineLayout> localPipelineLayout in m_LocalPipelineLayoutMap)
            {
                localPipelineLayout.Value.Dispose();
            }
        }
    }

    internal unsafe class Dx12MeshletPipeline : RHIMeshletPipeline
    {
        public int StencilRef
        {
            get
            {
                return m_StencilRef;
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
        private Dx12PipelineLayout m_PipelineLayout;
        private ID3D12PipelineState* m_NativePipelineState;
        private D3D_PRIMITIVE_TOPOLOGY m_PrimitiveTopology;

        public Dx12MeshletPipeline(Dx12Device device, in RHIMeshletPipelineDescriptor descriptor)
        {

        }

        protected override void Release()
        {
            m_PipelineLayout.Dispose();
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
        private int[] m_VertexStrides;
        private Dx12PipelineLayout m_PipelineLayout;
        private ID3D12PipelineState* m_NativePipelineState;
        private D3D_PRIMITIVE_TOPOLOGY m_PrimitiveTopology;

        public Dx12GraphicsPipeline(Dx12Device device, in RHIGraphicsPipelineDescriptor descriptor)
        {
            RHIPipelineLayoutDescriptor pipelienLayoutDescriptor;
            {
                pipelienLayoutDescriptor.bindGroupLayouts = descriptor.bindGroupLayouts;
            }
            m_PipelineLayout = device.CreatePipelineLayout(pipelienLayoutDescriptor) as Dx12PipelineLayout;

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

            m_StencilRef = descriptor.renderStateDescriptor.depthStencilStateDescriptor.stencilReference;
            m_PrimitiveTopology = Dx12Utility.ConvertToDx12PrimitiveTopology(descriptor.vertexStateDescriptor.primitiveTopology);

            D3D12_PRIMITIVE_TOPOLOGY_TYPE primitiveTopologyType = Dx12Utility.ConvertToDx12PrimitiveTopologyType(descriptor.vertexStateDescriptor.primitiveTopology);

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
                PrimitiveTopologyType = primitiveTopologyType,

                SampleMask = descriptor.renderStateDescriptor.sampleMask.HasValue ? ((uint)descriptor.renderStateDescriptor.sampleMask.Value) : uint.MaxValue,
                BlendState = Dx12Utility.CreateDx12BlendState(descriptor.renderStateDescriptor.blendStateDescriptor),
                RasterizerState = Dx12Utility.CreateDx12RasterizerState(descriptor.renderStateDescriptor.rasterizerStateDescriptor, descriptor.outputStateDescriptor.sampleCount != ESampleCount.None),
                DepthStencilState = Dx12Utility.CreateDx12DepthStencilState(descriptor.renderStateDescriptor.depthStencilStateDescriptor)
            };

            if (descriptor.outputStateDescriptor.outputDepthAttachmentDescriptor.HasValue)
            {
                description.DSVFormat = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT;
                //description.DSVFormat = Dx12Utility.ConvertToDx12Format(descriptor.outputState.depthAttachment.Value.format);
            }

            for (int i = 0; i < descriptor.outputStateDescriptor.outputColorAttachmentDescriptors.Length; ++i)
            {
                description.RTVFormats[i] = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                //description.RTVFormats[i] = Dx12Utility.ConvertToDx12Format(descriptor.outputState.colorAttachments.Span[i].format);
            }

            description.Flags = D3D12_PIPELINE_STATE_FLAGS.D3D12_PIPELINE_STATE_FLAG_NONE;
            description.NumRenderTargets = (uint)descriptor.outputStateDescriptor.outputColorAttachmentDescriptors.Length;
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
            m_PipelineLayout.Dispose();
            m_NativePipelineState->Release();
        }
    }
#pragma warning restore CS0169, CS0649, CS8600, CS8601, CS8602, CS8604, CS8618, CA1416
}
