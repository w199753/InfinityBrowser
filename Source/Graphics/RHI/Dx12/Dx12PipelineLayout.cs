using System;
using System.Diagnostics;
using Infinity.Container;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS0169, CS0649, CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12BindingTypeAndRootParameterIndex
    {
        public int index;
        public EBindingType bindType;
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
        private Dictionary<int, Dx12BindingTypeAndRootParameterIndex> m_VertexRootParameterIndexMap;
        private Dictionary<int, Dx12BindingTypeAndRootParameterIndex> m_FragmentRootParameterIndexMap;
        private Dictionary<int, Dx12BindingTypeAndRootParameterIndex> m_ComputeRootParameterIndexMap;

        public Dx12PipelineLayout(Dx12Device device, in RHIPipelineLayoutCreateInfo createInfo)
        {
            m_VertexRootParameterIndexMap = new Dictionary<int, Dx12BindingTypeAndRootParameterIndex>(8);
            m_FragmentRootParameterIndexMap = new Dictionary<int, Dx12BindingTypeAndRootParameterIndex>(8);
            m_ComputeRootParameterIndexMap = new Dictionary<int, Dx12BindingTypeAndRootParameterIndex>(8);

            /*int allocSize = 0;
            for (int i = 0; i < createInfo.bindGroupCount; ++i)
            {
                Dx12BindGroupLayout bindGroupLayout = createInfo.bindGroupLayouts[i] as Dx12BindGroupLayout;
                allocSize += bindGroupLayout.NativeRootParameters.Length;
            }*/

            //D3D12_ROOT_PARAMETER1* rootParameters = stackalloc D3D12_ROOT_PARAMETER1[allocSize];
            TValueArray<D3D12_ROOT_PARAMETER1> rootParameters = new TValueArray<D3D12_ROOT_PARAMETER1>(128);

            //int baseSlot = -1;

            for (int i = 0; i < createInfo.bindGroupCount; ++i)
            {
                //baseSlot += 1;
                int baseSlot = rootParameters.length;
                Dx12BindGroupLayout bindGroupLayout = createInfo.bindGroupLayouts[i] as Dx12BindGroupLayout;

                for (int j = 0; j < bindGroupLayout.NativeRootParameters.Length; ++j)
                {
                    int slot = baseSlot + j;
                    //rootParameters[i + j] = bindGroupLayout.NativeRootParameters[slot];
                    rootParameters.Add(bindGroupLayout.NativeRootParameters[slot]);

                    ref Dx12RootParameterKeyInfo keyInfo = ref bindGroupLayout.RootParameterKeyInfos[slot];

                    Dx12BindingTypeAndRootParameterIndex parameter;
                    parameter.index = slot;
                    parameter.bindType = keyInfo.bindType;

                    if ((keyInfo.shaderStage & EShaderStageFlags.Vertex) == EShaderStageFlags.Vertex)
                    {
                        m_VertexRootParameterIndexMap.TryAdd((keyInfo.layoutIndex << 8) + keyInfo.slot, parameter);
                    }

                    if ((keyInfo.shaderStage & EShaderStageFlags.Fragment) == EShaderStageFlags.Fragment)
                    {
                        m_FragmentRootParameterIndexMap.TryAdd((keyInfo.layoutIndex << 8) + keyInfo.slot, parameter);
                    }

                    if ((keyInfo.shaderStage & EShaderStageFlags.Compute) == EShaderStageFlags.Compute)
                    {
                        m_ComputeRootParameterIndexMap.TryAdd((keyInfo.layoutIndex << 8) + keyInfo.slot, parameter);
                    }
                }
            }

            D3D12_DESCRIPTOR_RANGE1 descriptorRange = new D3D12_DESCRIPTOR_RANGE1();
            descriptorRange.Init(D3D12_DESCRIPTOR_RANGE_TYPE.D3D12_DESCRIPTOR_RANGE_TYPE_UAV, 1, 0, 0, D3D12_DESCRIPTOR_RANGE_FLAGS.D3D12_DESCRIPTOR_RANGE_FLAG_DATA_STATIC);
            rootParameters[0].InitAsDescriptorTable(1, &descriptorRange, D3D12_SHADER_VISIBILITY.D3D12_SHADER_VISIBILITY_ALL);

            D3D12_VERSIONED_ROOT_SIGNATURE_DESC rootSignatureDesc = new D3D12_VERSIONED_ROOT_SIGNATURE_DESC();
            rootSignatureDesc.Init_1_1((uint)rootParameters.length, rootParameters.NativePtr, 0, null, D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT);

            ID3DBlob* signature;
            Dx12Utility.CHECK_HR(DirectX.D3D12SerializeVersionedRootSignature(&rootSignatureDesc, D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_1, &signature, null));

            ID3D12RootSignature* rootSignature;
            Dx12Utility.CHECK_HR(device.NativeDevice->CreateRootSignature(0, signature->GetBufferPointer(), signature->GetBufferSize(), __uuidof<ID3D12RootSignature>(), (void**)&rootSignature));
            m_NativeRootSignature = rootSignature;

            rootParameters.Dispose();
        }

        public Dx12BindingTypeAndRootParameterIndex? QueryRootDescriptorParameterIndex(in EShaderStageFlags shaderStage, in int layoutIndex, in int slot)
        {
            bool hasValue = false;
            Dx12BindingTypeAndRootParameterIndex? outParameter = null;

            if ((shaderStage & EShaderStageFlags.Vertex) == EShaderStageFlags.Vertex)
            {
                hasValue = m_VertexRootParameterIndexMap.TryGetValue((layoutIndex << 8) + slot, out Dx12BindingTypeAndRootParameterIndex parameter);
                outParameter = parameter;
            }

            if ((shaderStage & EShaderStageFlags.Fragment) == EShaderStageFlags.Fragment)
            {
                hasValue = m_FragmentRootParameterIndexMap.TryGetValue((layoutIndex << 8) + slot, out Dx12BindingTypeAndRootParameterIndex parameter);
                outParameter = parameter;
            }

            if ((shaderStage & EShaderStageFlags.Compute) == EShaderStageFlags.Compute)
            {
                hasValue = m_ComputeRootParameterIndexMap.TryGetValue((layoutIndex << 8) + slot, out Dx12BindingTypeAndRootParameterIndex parameter);
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
