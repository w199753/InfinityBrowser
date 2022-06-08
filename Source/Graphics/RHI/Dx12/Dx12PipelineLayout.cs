using System;
using System.Diagnostics;
using Infinity.Container;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS0169, CS0649, CS8600, CS8602, CS8604, CS8618, CA1416
    internal struct Dx12BindTypeAndParameterSlot
    {
        public int slot;
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

            int baseSlot = 0;
            D3D12_ROOT_PARAMETER1* rootParameters = stackalloc D3D12_ROOT_PARAMETER1[parameterCount];
            //TValueArray<D3D12_ROOT_PARAMETER1> rootParameters = new TValueArray<D3D12_ROOT_PARAMETER1>(128);

            for (int i = 0; i < createInfo.bindGroupCount; ++i)
            {
                baseSlot += i;
                //int baseSlot = rootParameters.length;
                Dx12BindGroupLayout bindGroupLayout = createInfo.bindGroupLayouts[i] as Dx12BindGroupLayout;

                for (int j = 0; j < bindGroupLayout.RootParameterKeyInfos.Length; ++j)
                {
                    int slot = baseSlot + j;
                    ref Dx12RootParameterKeyInfo keyInfo = ref bindGroupLayout.RootParameterKeyInfos[slot];

                    D3D12_DESCRIPTOR_RANGE1 dx12DescriptorRange = new D3D12_DESCRIPTOR_RANGE1();
                    dx12DescriptorRange.Init(Dx12Utility.ConvertToDX12BindType(keyInfo.bindType), 1, (uint)keyInfo.slot, (uint)keyInfo.layoutIndex, D3D12_DESCRIPTOR_RANGE_FLAGS.D3D12_DESCRIPTOR_RANGE_FLAG_DATA_STATIC);
                    rootParameters[i + j].InitAsDescriptorTable(1, &dx12DescriptorRange, Dx12Utility.ConvertToDX12ShaderStage(keyInfo.shaderStage));
                    //rootParameters.Add(bindGroupLayout.NativeRootParameters[slot]);

                    Dx12BindTypeAndParameterSlot parameter;
                    parameter.slot = slot;
                    parameter.bindType = keyInfo.bindType;

                    if ((keyInfo.shaderStage & EShaderStageFlags.Vertex) == EShaderStageFlags.Vertex)
                    {
                        m_VertexParameterMap.TryAdd((keyInfo.layoutIndex << 8) + keyInfo.slot, parameter);
                    }

                    if ((keyInfo.shaderStage & EShaderStageFlags.Fragment) == EShaderStageFlags.Fragment)
                    {
                        m_FragmentParameterMap.TryAdd((keyInfo.layoutIndex << 8) + keyInfo.slot, parameter);
                    }

                    if ((keyInfo.shaderStage & EShaderStageFlags.Compute) == EShaderStageFlags.Compute)
                    {
                        m_ComputeParameterMap.TryAdd((keyInfo.layoutIndex << 8) + keyInfo.slot, parameter);
                    }
                }
            }

            D3D12_VERSIONED_ROOT_SIGNATURE_DESC rootSignatureDesc = new D3D12_VERSIONED_ROOT_SIGNATURE_DESC();
            rootSignatureDesc.Init_1_1((uint)parameterCount, rootParameters, 0, null, D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT);

            ID3DBlob* signature;
            Dx12Utility.CHECK_HR(DirectX.D3D12SerializeVersionedRootSignature(&rootSignatureDesc, D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_1, &signature, null));

            ID3D12RootSignature* rootSignature;
            Dx12Utility.CHECK_HR(device.NativeDevice->CreateRootSignature(0, signature->GetBufferPointer(), signature->GetBufferSize(), __uuidof<ID3D12RootSignature>(), (void**)&rootSignature));
            m_NativeRootSignature = rootSignature;

            //rootParameters.Dispose();
        }

        public Dx12BindTypeAndParameterSlot? QueryRootDescriptorParameterIndex(in EShaderStageFlags shaderStage, in int layoutIndex, in int slot)
        {
            bool hasValue = false;
            Dx12BindTypeAndParameterSlot? outParameter = null;

            if ((shaderStage & EShaderStageFlags.Vertex) == EShaderStageFlags.Vertex)
            {
                hasValue = m_VertexParameterMap.TryGetValue((layoutIndex << 8) + slot, out Dx12BindTypeAndParameterSlot parameter);
                outParameter = parameter;
            }

            if ((shaderStage & EShaderStageFlags.Fragment) == EShaderStageFlags.Fragment)
            {
                hasValue = m_FragmentParameterMap.TryGetValue((layoutIndex << 8) + slot, out Dx12BindTypeAndParameterSlot parameter);
                outParameter = parameter;
            }

            if ((shaderStage & EShaderStageFlags.Compute) == EShaderStageFlags.Compute)
            {
                hasValue = m_ComputeParameterMap.TryGetValue((layoutIndex << 8) + slot, out Dx12BindTypeAndParameterSlot parameter);
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
