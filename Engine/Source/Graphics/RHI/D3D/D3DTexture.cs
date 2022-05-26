using System.Diagnostics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
    internal unsafe class D3DTexture : RHITexture
    {
        public D3DDevice D3DDevice
        {
            get
            {
                return m_D3DDevice;
            }
        }
        public ETextureUsageFlags Usages
        {
            get
            {
                return m_Usages;
            }
        }
        public ID3D12Resource* NativeResource
        {
            get
            {
                return m_NativeResource;
            }
        }

        private D3DDevice m_D3DDevice;
        private ETextureUsageFlags m_Usages;
        private ID3D12Resource* m_NativeResource;

        public D3DTexture(D3DDevice device, in RHITextureCreateInfo createInfo)
        {
            m_D3DDevice = device;
            m_Usages = createInfo.usages;

            D3D12_HEAP_PROPERTIES heapProperties = new D3D12_HEAP_PROPERTIES(/*D3DUtility.GetDX12HeapType(createInfo.usages)*/D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT);
            D3D12_RESOURCE_DESC textureDesc = new D3D12_RESOURCE_DESC();
            textureDesc.MipLevels = (ushort)createInfo.mipLevels;
            textureDesc.Format = /*D3DUtility.GetNativeFormat(createInfo->format)*/DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_TYPELESS;
            textureDesc.Width = (ulong)createInfo.extent.x;
            textureDesc.Height = (uint)createInfo.extent.y;
            textureDesc.Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE;
            textureDesc.DepthOrArraySize = (ushort)createInfo.extent.z;
            textureDesc.SampleDesc.Count = (uint)createInfo.samples;
            textureDesc.SampleDesc.Quality = 0;
            textureDesc.Dimension = D3DUtility.ConvertToDX12ResourceDimension(createInfo.dimension);

            ID3D12Resource* dx12Resource;
            bool success = SUCCEEDED(m_D3DDevice.NativeDevice->CreateCommittedResource(&heapProperties, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &textureDesc, D3DUtility.GetDX12ResourceStateByUsage(createInfo.usages), null, Windows.__uuidof<ID3D12Resource>(), (void**)&dx12Resource));
            Debug.Assert(success);
            m_NativeResource = dx12Resource;
        }

        public D3DTexture(D3DDevice device, in ID3D12Resource* resource)
        {
            m_D3DDevice = device;
            m_NativeResource = resource;
            m_Usages = ETextureUsageFlags.ColorAttachment;
        }

        public override RHITextureView CreateTextureView(in RHITextureViewCreateInfo createInfo)
        {
            return new D3DTextureView(this, createInfo);
        }

        protected override void Release()
        {
            m_NativeResource->Release();
        }
    }
}
