using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class D3DBufferView : RHIBufferView
    {
        public ID3D12DescriptorHeap* NativeDescriptorHeap
        {
            get
            {
                return m_NativeDescriptorHeap;
            }
        }
        public D3D12_INDEX_BUFFER_VIEW NativeIndexBufferView
        {
            get
            {
                return m_NativeIndexBufferView;
            }
        }
        public D3D12_VERTEX_BUFFER_VIEW NativeVertexBufferView
        {
            get
            {
                return m_NativeVertexBufferView;
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
        private D3DBuffer m_D3DBuffer;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private D3D12_INDEX_BUFFER_VIEW m_NativeIndexBufferView;
        private D3D12_VERTEX_BUFFER_VIEW m_NativeVertexBufferView;
        private D3D12_CPU_DESCRIPTOR_HANDLE m_NativeCpuDescriptorHandle;
        private D3D12_GPU_DESCRIPTOR_HANDLE m_NativeGpuDescriptorHandle;

        public D3DBufferView(D3DBuffer buffer, in RHIBufferViewCreateInfo createInfo)
        {
            m_D3DBuffer = buffer;

            if (D3DUtility.IsIndexBuffer(m_D3DBuffer.Usages))
            {
                m_NativeIndexBufferView.SizeInBytes = (uint)createInfo.size;
                m_NativeIndexBufferView.Format = /*D3DUtility.GetNativeFormat(createInfo.index.format)*/ DXGI_FORMAT.DXGI_FORMAT_R16_UINT;
                m_NativeIndexBufferView.BufferLocation = m_D3DBuffer.NativeResource->GetGPUVirtualAddress() + (ulong)createInfo.offset;
            }
            else if (D3DUtility.IsVertexBuffer(m_D3DBuffer.Usages))
            {
                m_NativeVertexBufferView.SizeInBytes = (uint)createInfo.size;
                m_NativeVertexBufferView.StrideInBytes = (uint)createInfo.vertex.stride;
                m_NativeVertexBufferView.BufferLocation = m_D3DBuffer.NativeResource->GetGPUVirtualAddress() + (ulong)createInfo.offset;
            }
            else if (D3DUtility.IsConstantBuffer(m_D3DBuffer.Usages))
            {
                D3D12_CONSTANT_BUFFER_VIEW_DESC desc = new D3D12_CONSTANT_BUFFER_VIEW_DESC();
                desc.SizeInBytes = (uint)createInfo.size;
                desc.BufferLocation = m_D3DBuffer.NativeResource->GetGPUVirtualAddress() + (ulong)createInfo.offset;

                D3DDescriptorInfo allocation = m_D3DBuffer.D3DDevice.AllocateCbvSrvUavDescriptor(1);
                m_HeapIndex = allocation.index;
                m_NativeDescriptorHeap = allocation.descriptorHeap;
                m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                m_D3DBuffer.D3DDevice.NativeDevice->CreateConstantBufferView(&desc, m_NativeCpuDescriptorHandle);
            }
            else if (D3DUtility.IsUnorderedAccessBuffer(m_D3DBuffer.Usages))
            {
                D3D12_UNORDERED_ACCESS_VIEW_DESC desc = new D3D12_UNORDERED_ACCESS_VIEW_DESC();
                desc.Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
                desc.Buffer.NumElements = (uint)createInfo.size;
                desc.Buffer.FirstElement = (ulong)createInfo.offset;
                desc.ViewDimension = D3D12_UAV_DIMENSION.D3D12_UAV_DIMENSION_BUFFER;

                D3DDescriptorInfo allocation = m_D3DBuffer.D3DDevice.AllocateCbvSrvUavDescriptor(1);
                m_HeapIndex = allocation.index;
                m_NativeDescriptorHeap = allocation.descriptorHeap;
                m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                m_D3DBuffer.D3DDevice.NativeDevice->CreateUnorderedAccessView(m_D3DBuffer.NativeResource, null, &desc, m_NativeCpuDescriptorHandle);
            }
        }

        protected override void Release()
        {
            if (D3DUtility.IsConstantBuffer(m_D3DBuffer.Usages) || D3DUtility.IsUnorderedAccessBuffer(m_D3DBuffer.Usages))
            {
                m_D3DBuffer.D3DDevice.FreeCbvSrvUavDescriptor(m_HeapIndex);
            }
        }
    }
}
