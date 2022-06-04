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
        private Dx12Buffer m_Dx12Buffer;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private D3D12_INDEX_BUFFER_VIEW m_NativeIndexBufferView;
        private D3D12_VERTEX_BUFFER_VIEW m_NativeVertexBufferView;
        private D3D12_CPU_DESCRIPTOR_HANDLE m_NativeCpuDescriptorHandle;
        private D3D12_GPU_DESCRIPTOR_HANDLE m_NativeGpuDescriptorHandle;

        public Dx12BufferView(Dx12Buffer buffer, in RHIBufferViewCreateInfo createInfo)
        {
            m_Dx12Buffer = buffer;

            if (Dx12Utility.IsIndexBuffer(m_Dx12Buffer.Usages))
            {
                m_NativeIndexBufferView.SizeInBytes = (uint)createInfo.size;
                m_NativeIndexBufferView.Format = /*Dx12Utility.GetNativeFormat(createInfo.index.format)*/ DXGI_FORMAT.DXGI_FORMAT_R16_UINT;
                m_NativeIndexBufferView.BufferLocation = m_Dx12Buffer.NativeResource->GetGPUVirtualAddress() + (ulong)createInfo.offset;
            }
            else if (Dx12Utility.IsVertexBuffer(m_Dx12Buffer.Usages))
            {
                m_NativeVertexBufferView.SizeInBytes = (uint)createInfo.size;
                m_NativeVertexBufferView.StrideInBytes = (uint)createInfo.vertex.stride;
                m_NativeVertexBufferView.BufferLocation = m_Dx12Buffer.NativeResource->GetGPUVirtualAddress() + (ulong)createInfo.offset;
            }
            else if (Dx12Utility.IsConstantBuffer(m_Dx12Buffer.Usages))
            {
                D3D12_CONSTANT_BUFFER_VIEW_DESC desc = new D3D12_CONSTANT_BUFFER_VIEW_DESC();
                desc.SizeInBytes = (uint)createInfo.size;
                desc.BufferLocation = m_Dx12Buffer.NativeResource->GetGPUVirtualAddress() + (ulong)createInfo.offset;

                Dx12DescriptorInfo allocation = m_Dx12Buffer.Dx12Device.AllocateCbvSrvUavDescriptor(1);
                m_HeapIndex = allocation.index;
                m_NativeDescriptorHeap = allocation.descriptorHeap;
                m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                m_Dx12Buffer.Dx12Device.NativeDevice->CreateConstantBufferView(&desc, m_NativeCpuDescriptorHandle);
            }
            else if (Dx12Utility.IsUnorderedAccessBuffer(m_Dx12Buffer.Usages))
            {
                D3D12_UNORDERED_ACCESS_VIEW_DESC desc = new D3D12_UNORDERED_ACCESS_VIEW_DESC();
                desc.Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
                desc.Buffer.NumElements = (uint)createInfo.size;
                desc.Buffer.FirstElement = (ulong)createInfo.offset;
                desc.ViewDimension = D3D12_UAV_DIMENSION.D3D12_UAV_DIMENSION_BUFFER;

                Dx12DescriptorInfo allocation = m_Dx12Buffer.Dx12Device.AllocateCbvSrvUavDescriptor(1);
                m_HeapIndex = allocation.index;
                m_NativeDescriptorHeap = allocation.descriptorHeap;
                m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                m_Dx12Buffer.Dx12Device.NativeDevice->CreateUnorderedAccessView(m_Dx12Buffer.NativeResource, null, &desc, m_NativeCpuDescriptorHandle);
            }
        }

        protected override void Release()
        {
            if (Dx12Utility.IsConstantBuffer(m_Dx12Buffer.Usages) || Dx12Utility.IsUnorderedAccessBuffer(m_Dx12Buffer.Usages))
            {
                m_Dx12Buffer.Dx12Device.FreeCbvSrvUavDescriptor(m_HeapIndex);
            }
        }
    }
}
