﻿using Infinity.Mathmatics;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class Dx12BufferView : RHIBufferView
    {
        public ID3D12DescriptorHeap* NativeDescriptorHeap
        {
            get
            {
                return m_NativeDescriptorHeap;
            }
        }
        public D3D12_CPU_DESCRIPTOR_HANDLE NativeCpuDescriptorHandle
        {
            get
            {
                return m_NativeCpuDescriptorHandle;
            }
        }
        public D3D12_GPU_DESCRIPTOR_HANDLE NativeGpuDescriptorHandle
        {
            get
            {
                return m_NativeGpuDescriptorHandle;
            }
        }

        private int m_HeapIndex;
        private bool3 m_LifeState;
        private Dx12Buffer m_Dx12Buffer;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private D3D12_CPU_DESCRIPTOR_HANDLE m_NativeCpuDescriptorHandle;
        private D3D12_GPU_DESCRIPTOR_HANDLE m_NativeGpuDescriptorHandle;

        public Dx12BufferView(Dx12Buffer buffer, in RHIBufferViewCreateInfo createInfo)
        {
            m_LifeState = false;
            m_Dx12Buffer = buffer;

            if (createInfo.type == EBufferViewType.UniformBuffer)
            {
                if (Dx12Utility.IsConstantBuffer(buffer.CreateInfo.flag))
                {
                    m_LifeState.x = true;

                    D3D12_CONSTANT_BUFFER_VIEW_DESC desc = new D3D12_CONSTANT_BUFFER_VIEW_DESC();
                    desc.SizeInBytes = (uint)createInfo.stride;
                    desc.BufferLocation = m_Dx12Buffer.NativeResource->GetGPUVirtualAddress() + (ulong)(createInfo.stride * createInfo.offset);

                    Dx12DescriptorInfo allocation = m_Dx12Buffer.Dx12Device.AllocateCbvSrvUavDescriptor(1);
                    m_HeapIndex = allocation.index;
                    m_NativeDescriptorHeap = allocation.descriptorHeap;
                    m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                    m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                    m_Dx12Buffer.Dx12Device.NativeDevice->CreateConstantBufferView(&desc, m_NativeCpuDescriptorHandle);
                }
            }
            else if (createInfo.type == EBufferViewType.ShaderResource)
            {
                if (Dx12Utility.IsShaderResourceBuffer(buffer.CreateInfo.flag))
                {
                    m_LifeState.y = true;

                    D3D12_SHADER_RESOURCE_VIEW_DESC desc = new D3D12_SHADER_RESOURCE_VIEW_DESC();
                    desc.Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
                    desc.Buffer.NumElements = (uint)createInfo.count;
                    desc.Buffer.FirstElement = (ulong)createInfo.offset;
                    desc.Buffer.StructureByteStride = (uint)createInfo.stride;
                    desc.ViewDimension = D3D12_SRV_DIMENSION.D3D12_SRV_DIMENSION_BUFFER;
                    desc.Shader4ComponentMapping = 5768;

                    Dx12DescriptorInfo allocation = m_Dx12Buffer.Dx12Device.AllocateCbvSrvUavDescriptor(1);
                    m_HeapIndex = allocation.index;
                    m_NativeDescriptorHeap = allocation.descriptorHeap;
                    m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                    m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                    m_Dx12Buffer.Dx12Device.NativeDevice->CreateShaderResourceView(m_Dx12Buffer.NativeResource, &desc, m_NativeCpuDescriptorHandle);
                }
            }
            else if (createInfo.type == EBufferViewType.UnorderedAccess)
            {
                if (Dx12Utility.IsUnorderedAccessBuffer(buffer.CreateInfo.flag))
                {
                    m_LifeState.z = true;

                    D3D12_UNORDERED_ACCESS_VIEW_DESC desc = new D3D12_UNORDERED_ACCESS_VIEW_DESC();
                    desc.Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
                    desc.Buffer.NumElements = (uint)createInfo.count;
                    desc.Buffer.FirstElement = (ulong)createInfo.offset;
                    desc.Buffer.StructureByteStride = (uint)createInfo.stride;
                    desc.ViewDimension = D3D12_UAV_DIMENSION.D3D12_UAV_DIMENSION_BUFFER;

                    Dx12DescriptorInfo allocation = m_Dx12Buffer.Dx12Device.AllocateCbvSrvUavDescriptor(1);
                    m_HeapIndex = allocation.index;
                    m_NativeDescriptorHeap = allocation.descriptorHeap;
                    m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                    m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                    m_Dx12Buffer.Dx12Device.NativeDevice->CreateUnorderedAccessView(m_Dx12Buffer.NativeResource, null, &desc, m_NativeCpuDescriptorHandle);
                }
            }
        }

        protected override void Release()
        {
            if (m_LifeState.x || m_LifeState.y || m_LifeState.z)
            {
                m_Dx12Buffer.Dx12Device.FreeCbvSrvUavDescriptor(m_HeapIndex);
            }
        }
    }
}
