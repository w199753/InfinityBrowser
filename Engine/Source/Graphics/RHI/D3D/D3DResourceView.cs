using System;
using InfinityEngine.Memory;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI.D3D
{
    public unsafe class D3DDeptnStencilView : RHIDeptnStencilView
    {
        internal D3D12_CPU_DESCRIPTOR_HANDLE descriptorHandle => m_DescriptorHandle;

        private D3D12_CPU_DESCRIPTOR_HANDLE m_DescriptorHandle;
        private RHIDescriptorHeapFactory m_DescriptorHeapFactory;

        internal D3DDeptnStencilView(RHIDevice device, RHIDescriptorHeapFactory descriptorHeapFactory, RHITexture texture) 
        {
            m_DescriptorHeapFactory = descriptorHeapFactory;
            D3DDescriptorHeapFactory d3dDescriptorHeapFactory = (D3DDescriptorHeapFactory)descriptorHeapFactory;
            descriptorIndex = d3dDescriptorHeapFactory.Allocate();
            m_DescriptorHandle = d3dDescriptorHeapFactory.cpuStartHandle;
            m_DescriptorHandle.Offset(descriptorIndex, d3dDescriptorHeapFactory.descriptorSize);

            D3DDevice d3dDevice = (D3DDevice)device;
            D3DTexture d3DTexture = (D3DTexture)texture;
            D3D12_DEPTH_STENCIL_VIEW_DESC dsvDescriptor;
            dsvDescriptor.Format = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT;
            dsvDescriptor.ViewDimension = D3D12_DSV_DIMENSION.D3D12_DSV_DIMENSION_TEXTURE2D;
            d3dDevice.nativeDevice->CreateDepthStencilView(d3DTexture.defaultResource, &dsvDescriptor, m_DescriptorHandle);
        }

        protected override void Release()
        {
            m_DescriptorHeapFactory.Free(descriptorIndex);
        }
    }

    public unsafe class D3DRenderTargetView : RHIRenderTargetView
    {
        internal D3D12_CPU_DESCRIPTOR_HANDLE descriptorHandle => m_DescriptorHandle;

        private D3D12_CPU_DESCRIPTOR_HANDLE m_DescriptorHandle;
        private RHIDescriptorHeapFactory m_DescriptorHeapFactory;

        internal D3DRenderTargetView(RHIDevice device, RHIDescriptorHeapFactory descriptorHeapFactory, RHITexture texture)
        {
            m_DescriptorHeapFactory = descriptorHeapFactory;
            D3DDescriptorHeapFactory d3dDescriptorHeapFactory = (D3DDescriptorHeapFactory)descriptorHeapFactory;
            descriptorIndex = d3dDescriptorHeapFactory.Allocate();
            m_DescriptorHandle = d3dDescriptorHeapFactory.cpuStartHandle;
            m_DescriptorHandle.Offset(descriptorIndex, d3dDescriptorHeapFactory.descriptorSize);

            D3DDevice d3dDevice = (D3DDevice)device;
            D3DTexture d3DTexture = (D3DTexture)texture;
            D3D12_RENDER_TARGET_VIEW_DESC rtvDescriptor;
            rtvDescriptor.Format = D3DResourceUtility.GetNativeViewFormat(d3DTexture.descriptor.format);
            rtvDescriptor.ViewDimension = D3DResourceUtility.GetRTVDimension(d3DTexture.descriptor.textureType);
            d3dDevice.nativeDevice->CreateRenderTargetView(d3DTexture.defaultResource, &rtvDescriptor, m_DescriptorHandle);
        }

        protected override void Release()
        {
            m_DescriptorHeapFactory.Free(descriptorIndex);
        }
    }
}
