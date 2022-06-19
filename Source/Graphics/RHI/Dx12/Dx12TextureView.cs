﻿using Infinity.Mathmatics;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class Dx12TextureView : RHITextureView
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
        private bool4 m_LifeState;
        private Dx12Texture m_Dx12Texture;
        private ID3D12DescriptorHeap* m_NativeDescriptorHeap;
        private D3D12_CPU_DESCRIPTOR_HANDLE m_NativeCpuDescriptorHandle;
        private D3D12_GPU_DESCRIPTOR_HANDLE m_NativeGpuDescriptorHandle;

        public Dx12TextureView(Dx12Texture texture, in RHITextureViewCreateInfo createInfo)
        {
            m_LifeState = false;
            m_Dx12Texture = texture;
            ETextureUsage usages = texture.Usages;

            if (createInfo.type == ETextureViewType.DepthStencil)
            {
                if(Dx12Utility.IsDepthStencilTexture(usages))
                {
                    m_LifeState.x = true;

                    //D3D12_DEPTH_STENCIL_DESC desc = new D3D12_DEPTH_STENCIL_DESC();
                    //desc.Format = /*Dx12Utility.GetNativeFormat(createInfo.format)*/ DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                    //desc.ViewDimension = /*Dx12Utility.GetNativeViewDimension(createInfo.dimension)*/ D3D12_RTV_DIMENSION.D3D12_RTV_DIMENSION_TEXTURE2D;
                    /*Dx12Utility.FillTexture2DDSV(ref desc.Texture2D, createInfo);
                    Dx12Utility.FillTexture3DDSV(ref desc.Texture3D, createInfo);
                    Dx12Utility.FillTexture2DArrayDSV(ref desc.Texture2DArray, createInfo);

                    Dx12DescriptorInfo allocation = m_Dx12Texture.Dx12Device.AllocateRtvDescriptor(1);
                    m_HeapIndex = allocation.index;
                    m_NativeDescriptorHeap = allocation.descriptorHeap;
                    m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                    m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                    m_Dx12Texture.Dx12Device.NativeDevice->CreateRenderTargetView(texture.NativeResource, &desc, m_NativeCpuDescriptorHandle);*/
                }
            }
            else if (createInfo.type == ETextureViewType.RenderTarget)
            {
                if (Dx12Utility.IsRenderTargetTexture(usages))
                {
                    m_LifeState.y = true;

                    D3D12_RENDER_TARGET_VIEW_DESC desc = new D3D12_RENDER_TARGET_VIEW_DESC();
                    desc.Format = /*Dx12Utility.GetNativeFormat(createInfo.format)*/ DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                    desc.ViewDimension = /*Dx12Utility.GetNativeViewDimension(createInfo.dimension)*/ D3D12_RTV_DIMENSION.D3D12_RTV_DIMENSION_TEXTURE2D;
                    Dx12Utility.FillTexture2DRTV(ref desc.Texture2D, createInfo);
                    Dx12Utility.FillTexture3DRTV(ref desc.Texture3D, createInfo);
                    Dx12Utility.FillTexture2DArrayRTV(ref desc.Texture2DArray, createInfo);

                    Dx12DescriptorInfo allocation = m_Dx12Texture.Dx12Device.AllocateRtvDescriptor(1);
                    m_HeapIndex = allocation.index;
                    m_NativeDescriptorHeap = allocation.descriptorHeap;
                    m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                    m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                    m_Dx12Texture.Dx12Device.NativeDevice->CreateRenderTargetView(m_Dx12Texture.NativeResource, &desc, m_NativeCpuDescriptorHandle);
                }
            } 
            else if (createInfo.type == ETextureViewType.ShaderResource)
            {
                if(Dx12Utility.IsShaderResourceTexture(usages))
                {
                    m_LifeState.z = true;

                    D3D12_SHADER_RESOURCE_VIEW_DESC desc = new D3D12_SHADER_RESOURCE_VIEW_DESC();
                    desc.Format = /*Dx12Utility.GetNativeFormat(createInfo.format)*/ DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                    desc.ViewDimension = /*Dx12Utility.GetNativeViewDimension(createInfo.dimension)*/ D3D12_SRV_DIMENSION.D3D12_SRV_DIMENSION_TEXTURE2D;
                    Dx12Utility.FillTexture2DSRV(ref desc.Texture2D, createInfo);
                    Dx12Utility.FillTexture2DArraySRV(ref desc.Texture2DArray, createInfo);
                    Dx12Utility.FillTextureCubeSRV(ref desc.TextureCube, createInfo);
                    Dx12Utility.FillTextureCubeArraySRV(ref desc.TextureCubeArray, createInfo);
                    Dx12Utility.FillTexture3DSRV(ref desc.Texture3D, createInfo);

                    Dx12DescriptorInfo allocation = m_Dx12Texture.Dx12Device.AllocateCbvSrvUavDescriptor(1);
                    m_HeapIndex = allocation.index;
                    m_NativeDescriptorHeap = allocation.descriptorHeap;
                    m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                    m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                    m_Dx12Texture.Dx12Device.NativeDevice->CreateShaderResourceView(m_Dx12Texture.NativeResource, &desc, m_NativeCpuDescriptorHandle);
                }
            }
            else if (createInfo.type == ETextureViewType.UnorderedAccess)
            {
                if(Dx12Utility.IsUnorderedAccessTexture(usages))
                {
                    m_LifeState.w = true;

                    D3D12_UNORDERED_ACCESS_VIEW_DESC desc = new D3D12_UNORDERED_ACCESS_VIEW_DESC();
                    desc.Format = /*Dx12Utility.GetNativeFormat(createInfo.format)*/ DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
                    desc.ViewDimension = /*Dx12Utility.GetNativeViewDimension(createInfo.dimension)*/ D3D12_UAV_DIMENSION.D3D12_UAV_DIMENSION_TEXTURE2D;
                    Dx12Utility.FillTexture2DUAV(ref desc.Texture2D, createInfo);
                    Dx12Utility.FillTexture3DUAV(ref desc.Texture3D, createInfo);
                    Dx12Utility.FillTexture2DArrayUAV(ref desc.Texture2DArray, createInfo);

                    Dx12DescriptorInfo allocation = m_Dx12Texture.Dx12Device.AllocateCbvSrvUavDescriptor(1);
                    m_HeapIndex = allocation.index;
                    m_NativeDescriptorHeap = allocation.descriptorHeap;
                    m_NativeCpuDescriptorHandle = allocation.cpuHandle;
                    m_NativeGpuDescriptorHandle = allocation.gpuHandle;
                    m_Dx12Texture.Dx12Device.NativeDevice->CreateUnorderedAccessView(m_Dx12Texture.NativeResource, null, &desc, m_NativeCpuDescriptorHandle);
                }
            }
        }

        protected override void Release()
        {
            if (m_LifeState.x)
            {
                m_Dx12Texture.Dx12Device.FreeDsvDescriptor(m_HeapIndex);
            }
            if (m_LifeState.y)
            {
                m_Dx12Texture.Dx12Device.FreeRtvDescriptor(m_HeapIndex);
            }
            if (m_LifeState.z || m_LifeState.w)
            {
                m_Dx12Texture.Dx12Device.FreeCbvSrvUavDescriptor(m_HeapIndex);
            }
        }
    }
}
