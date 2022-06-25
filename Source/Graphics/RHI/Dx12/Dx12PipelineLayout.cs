using System;
using System.Diagnostics;
using Infinity.Container;
using Infinity.Mathmatics;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS0169, CS0649, CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12BindTypeAndParameterSlot
    {
        public int index;
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

        public Dx12PipelineLayout(Dx12Device device, in RHIPipelineLayoutCreateInfo createInfo)
        {
            m_VertexParameterMap = new Dictionary<int, Dx12BindTypeAndParameterSlot>(8);
            m_FragmentParameterMap = new Dictionary<int, Dx12BindTypeAndParameterSlot>(8);
            m_ComputeParameterMap = new Dictionary<int, Dx12BindTypeAndParameterSlot>(8);

            int parameterCount = 0;
            for (int i = 0; i < createInfo.bindGroupCount; ++i)
            {
                Dx12BindGroupLayout bindGroupLayout = createInfo.bindGroupLayouts[i] as Dx12BindGroupLayout;
                parameterCount += bindGroupLayout.RootParameterKeyInfos.Length;
            }

            D3D12_DESCRIPTOR_RANGE1* rootDescriptorRangePtr = stackalloc D3D12_DESCRIPTOR_RANGE1[parameterCount];
            Span<D3D12_DESCRIPTOR_RANGE1> rootDescriptorRangeViews = new Span<D3D12_DESCRIPTOR_RANGE1>(rootDescriptorRangePtr, parameterCount);

            D3D12_ROOT_PARAMETER1* rootParameterPtr = stackalloc D3D12_ROOT_PARAMETER1[parameterCount];
            Span<D3D12_ROOT_PARAMETER1> rootParameterViews = new Span<D3D12_ROOT_PARAMETER1>(rootParameterPtr, parameterCount);

            for (int i = 0; i < createInfo.bindGroupCount; ++i)
            {
                Dx12BindGroupLayout bindGroupLayout = createInfo.bindGroupLayouts[i] as Dx12BindGroupLayout;

                for (int j = 0; j < bindGroupLayout.RootParameterKeyInfos.Length; ++j)
                {
                    ref Dx12RootParameterKeyInfo keyInfo = ref bindGroupLayout.RootParameterKeyInfos[j];

                    ref D3D12_DESCRIPTOR_RANGE1 rootDescriptorRange = ref rootDescriptorRangeViews[i + j];
                    rootDescriptorRange.Init(Dx12Utility.ConvertToDx12BindType(keyInfo.bindType), (uint)keyInfo.count, (uint)keyInfo.slot, (uint)keyInfo.layoutIndex, Dx12Utility.GetDx12DescriptorRangeFalag(keyInfo.bindType));

                    ref D3D12_ROOT_PARAMETER1 rootParameterView = ref rootParameterViews[i + j];
                    rootParameterView.InitAsDescriptorTable(1, rootDescriptorRangePtr + (i + j), Dx12Utility.ConvertToDx12ShaderStage(keyInfo.shaderStage));

                    Dx12BindTypeAndParameterSlot parameter;
                    parameter.index = i + j;
                    parameter.bindType = keyInfo.bindType;

                    if ((keyInfo.shaderStage & EShaderStageFlags.Vertex) == EShaderStageFlags.Vertex)
                    {
                        m_VertexParameterMap.TryAdd(new int3(keyInfo.layoutIndex << 8, keyInfo.slot, Dx12Utility.GetDx12BindKey(keyInfo.bindType)).GetHashCode(), parameter);
                    }

                    if ((keyInfo.shaderStage & EShaderStageFlags.Fragment) == EShaderStageFlags.Fragment)
                    {
                        m_FragmentParameterMap.TryAdd(new int3(keyInfo.layoutIndex << 8, keyInfo.slot, Dx12Utility.GetDx12BindKey(keyInfo.bindType)).GetHashCode(), parameter);
                    }

                    if ((keyInfo.shaderStage & EShaderStageFlags.Compute) == EShaderStageFlags.Compute)
                    {
                        m_ComputeParameterMap.TryAdd(new int3(keyInfo.layoutIndex << 8, keyInfo.slot, Dx12Utility.GetDx12BindKey(keyInfo.bindType)).GetHashCode(), parameter);
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

        public Dx12BindTypeAndParameterSlot? QueryRootDescriptorParameterIndex(in EShaderStageFlags shaderStage, in int layoutIndex, in int slot, in EBindType bindType)
        {
            bool hasValue = false;
            Dx12BindTypeAndParameterSlot? outParameter = null;

            if ((shaderStage & EShaderStageFlags.Vertex) == EShaderStageFlags.Vertex)
            {
                hasValue = m_VertexParameterMap.TryGetValue(new int3(layoutIndex << 8, slot, Dx12Utility.GetDx12BindKey(bindType)).GetHashCode(), out Dx12BindTypeAndParameterSlot parameter);
                outParameter = parameter;
            }

            if ((shaderStage & EShaderStageFlags.Fragment) == EShaderStageFlags.Fragment)
            {
                hasValue = m_FragmentParameterMap.TryGetValue(new int3(layoutIndex << 8, slot, Dx12Utility.GetDx12BindKey(bindType)).GetHashCode(), out Dx12BindTypeAndParameterSlot parameter);
                outParameter = parameter;
            }

            if ((shaderStage & EShaderStageFlags.Compute) == EShaderStageFlags.Compute)
            {
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
#pragma warning restore CS0169, CS0649, CS8600, CS8602, CS8604, CS8618, CA1416
}
