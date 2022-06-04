﻿using System;
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
        private Dictionary<EShaderStageFlags, Dictionary<int, Dx12BindingTypeAndRootParameterIndex>> m_RootDescriptorParameterIndexMap;

        public Dx12PipelineLayout(Dx12Device device, in RHIPipelineLayoutCreateInfo createInfo)
        {
            m_RootDescriptorParameterIndexMap = new Dictionary<EShaderStageFlags, Dictionary<int, Dx12BindingTypeAndRootParameterIndex>>(6);
            m_RootDescriptorParameterIndexMap.TryAdd(EShaderStageFlags.Vertex, new Dictionary<int, Dx12BindingTypeAndRootParameterIndex>(8));
            m_RootDescriptorParameterIndexMap.TryAdd(EShaderStageFlags.Fragment, new Dictionary<int, Dx12BindingTypeAndRootParameterIndex>(8));
            m_RootDescriptorParameterIndexMap.TryAdd(EShaderStageFlags.Compute, new Dictionary<int, Dx12BindingTypeAndRootParameterIndex>(8));

            TValueArray<D3D12_ROOT_PARAMETER1> rootParameters = new TValueArray<D3D12_ROOT_PARAMETER1>(16);

            for (int i = 0; i < createInfo.bindGroupCount; ++i)
            {
                int baseSlot = rootParameters.length;
                Dx12BindGroupLayout bindGroupLayout = createInfo.bindGroupLayouts[i] as Dx12BindGroupLayout;

                for (int j = 0; j < bindGroupLayout.NativeRootParameters.Length; ++j)
                {
                    int slot = baseSlot + j;
                    rootParameters.Add(bindGroupLayout.NativeRootParameters[slot]);

                    ref Dx12RootParameterKeyInfo keyInfo = ref bindGroupLayout.RootParameterKeyInfos[slot];

                    Dx12BindingTypeAndRootParameterIndex parameter;
                    parameter.index = slot;
                    parameter.bindType = keyInfo.bindType;
                    m_RootDescriptorParameterIndexMap.TryGetValue(keyInfo.shaderStage, out Dictionary<int, Dx12BindingTypeAndRootParameterIndex> parameterMap);
                    parameterMap.TryAdd((keyInfo.layoutIndex << 8) + keyInfo.slot, parameter);
                }
            }

            D3D12_VERSIONED_ROOT_SIGNATURE_DESC rootSignatureDesc = new D3D12_VERSIONED_ROOT_SIGNATURE_DESC();
            rootSignatureDesc.Init_1_1((uint)rootParameters.length, rootParameters.ArrayPtr, 0, null, D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT);

            ID3DBlob* signature;
            bool success = SUCCEEDED(DirectX.D3D12SerializeVersionedRootSignature(&rootSignatureDesc, D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_1, &signature, null));
            Debug.Assert(success);
            rootParameters.Dispose();

            ID3D12RootSignature* rootSignature;
            success = SUCCEEDED(device.NativeDevice->CreateRootSignature(0, signature->GetBufferPointer(), signature->GetBufferSize(), __uuidof<ID3D12RootSignature>(), (void**)&rootSignature));
            Debug.Assert(success);
            m_NativeRootSignature = rootSignature;
        }

        public Dx12BindingTypeAndRootParameterIndex? QueryRootDescriptorParameterIndex(in EShaderStageFlags shaderStage, in int layoutIndex, in int slot)
        {
            m_RootDescriptorParameterIndexMap.TryGetValue(shaderStage, out Dictionary<int, Dx12BindingTypeAndRootParameterIndex> parameterMap);
            bool hasValue = parameterMap.TryGetValue((layoutIndex << 8) + slot, out Dx12BindingTypeAndRootParameterIndex parameter);
            return hasValue ? parameter : null;
        }

        protected override void Release()
        {
            m_NativeRootSignature->Release();
        }
    }
#pragma warning restore CS0169, CS0649, CS8600, CS8602, CS8604, CS8618, CA1416
}
