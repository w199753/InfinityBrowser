using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class D3DTextureView : RHITextureView
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
        private D3DTexture m_D3DTexture;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private D3D12_CPU_DESCRIPTOR_HANDLE m_NativeCpuDescriptorHandle;
        private D3D12_GPU_DESCRIPTOR_HANDLE m_NativeGpuDescriptorHandle;

        public D3DTextureView(D3DTexture texture, in RHITextureViewCreateInfo createInfo)
        {
            m_D3DTexture = texture;
            ETextureUsageFlags usages = texture.Usages;

            if (D3DUtility.IsDepthStencilTexture(usages))
            {
                //D3D12_DEPTH_STENCIL_DESC desc = new D3D12_DEPTH_STENCIL_DESC();
                //desc.Format = /*D3DUtility.GetNativeFormat(createInfo.format)*/ DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                //desc.ViewDimension = /*D3DUtility.GetNativeViewDimension(createInfo.dimension)*/ D3D12_RTV_DIMENSION.D3D12_RTV_DIMENSION_TEXTURE2D;
                /*D3DUtility.FillTexture2DDSV(ref desc.Texture2D, createInfo);
                D3DUtility.FillTexture3DDSV(ref desc.Texture3D, createInfo);
                D3DUtility.FillTexture2DArrayDSV(ref desc.Texture2DArray, createInfo);

                D3DDescriptorInfo allocation = m_D3DTexture.D3DDevice.AllocateRtvDescriptor(1);
                m_HeapIndex = allocation.index;
                m_NativeDescriptorHeap = allocation.descriptorHeap;
                m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                m_D3DTexture.D3DDevice.NativeDevice->CreateRenderTargetView(texture.NativeResource, &desc, m_NativeCpuDescriptorHandle);*/
            }
            else if (D3DUtility.IsRenderTargetTexture(usages))
            {
                D3D12_RENDER_TARGET_VIEW_DESC desc = new D3D12_RENDER_TARGET_VIEW_DESC();
                desc.Format = /*D3DUtility.GetNativeFormat(createInfo.format)*/ DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                desc.ViewDimension = /*D3DUtility.GetNativeViewDimension(createInfo.dimension)*/ D3D12_RTV_DIMENSION.D3D12_RTV_DIMENSION_TEXTURE2D;
                D3DUtility.FillTexture2DRTV(ref desc.Texture2D, createInfo);
                D3DUtility.FillTexture3DRTV(ref desc.Texture3D, createInfo);
                D3DUtility.FillTexture2DArrayRTV(ref desc.Texture2DArray, createInfo);

                D3DDescriptorInfo allocation = m_D3DTexture.D3DDevice.AllocateRtvDescriptor(1);
                m_HeapIndex = allocation.index;
                m_NativeDescriptorHeap = allocation.descriptorHeap;
                m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                m_D3DTexture.D3DDevice.NativeDevice->CreateRenderTargetView(m_D3DTexture.NativeResource, &desc, m_NativeCpuDescriptorHandle);
            } 
            else if (D3DUtility.IsShaderResourceTexture(usages))
            {
                D3D12_SHADER_RESOURCE_VIEW_DESC desc = new D3D12_SHADER_RESOURCE_VIEW_DESC();
                desc.Format = /*D3DUtility.GetNativeFormat(createInfo.format)*/ DXGI_FORMAT.DXGI_FORMAT_R16_UINT;
                desc.ViewDimension = /*D3DUtility.GetNativeViewDimension(createInfo.dimension)*/ D3D12_SRV_DIMENSION.D3D12_SRV_DIMENSION_TEXTURE2D;
                D3DUtility.FillTexture2DSRV(ref desc.Texture2D, createInfo);
                D3DUtility.FillTexture2DArraySRV(ref desc.Texture2DArray, createInfo);
                D3DUtility.FillTextureCubeSRV(ref desc.TextureCube, createInfo);
                D3DUtility.FillTextureCubeArraySRV(ref desc.TextureCubeArray, createInfo);
                D3DUtility.FillTexture3DSRV(ref desc.Texture3D, createInfo);

                D3DDescriptorInfo allocation = m_D3DTexture.D3DDevice.AllocateCbvSrvUavDescriptor(1);
                m_HeapIndex = allocation.index;
                m_NativeDescriptorHeap = allocation.descriptorHeap;
                m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                m_D3DTexture.D3DDevice.NativeDevice->CreateShaderResourceView(m_D3DTexture.NativeResource, &desc, m_NativeCpuDescriptorHandle);
            }
            else if (D3DUtility.IsUnorderedAccessTexture(usages))
            {
                D3D12_UNORDERED_ACCESS_VIEW_DESC desc = new D3D12_UNORDERED_ACCESS_VIEW_DESC();
                desc.Format = /*D3DUtility.GetNativeFormat(createInfo.format)*/ DXGI_FORMAT.DXGI_FORMAT_R16_UINT;
                desc.ViewDimension = /*D3DUtility.GetNativeViewDimension(createInfo.dimension)*/ D3D12_UAV_DIMENSION.D3D12_UAV_DIMENSION_TEXTURE2D;
                D3DUtility.FillTexture2DUAV(ref desc.Texture2D, createInfo);
                D3DUtility.FillTexture3DUAV(ref desc.Texture3D, createInfo);
                D3DUtility.FillTexture2DArrayUAV(ref desc.Texture2DArray, createInfo);

                D3DDescriptorInfo allocation = m_D3DTexture.D3DDevice.AllocateCbvSrvUavDescriptor(1);
                m_HeapIndex = allocation.index;
                m_NativeDescriptorHeap = allocation.descriptorHeap;
                m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                m_D3DTexture.D3DDevice.NativeDevice->CreateUnorderedAccessView(m_D3DTexture.NativeResource, null, &desc, m_NativeCpuDescriptorHandle);
            }
        }

        protected override void Release()
        {
            ETextureUsageFlags usages = m_D3DTexture.Usages;

            if (D3DUtility.IsDepthStencilTexture(usages))
            {
                m_D3DTexture.D3DDevice.FreeDsvDescriptor(m_HeapIndex);
            }
            else if (D3DUtility.IsRenderTargetTexture(usages))
            {
                m_D3DTexture.D3DDevice.FreeRtvDescriptor(m_HeapIndex);
            }
            else if (D3DUtility.IsShaderResourceTexture(usages) || D3DUtility.IsUnorderedAccessTexture(usages))
            {
                m_D3DTexture.D3DDevice.FreeCbvSrvUavDescriptor(m_HeapIndex);
            }
        }
    }
}
