using System.Diagnostics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
    internal unsafe class Dx12Texture : RHITexture
    {
        public Dx12Device Dx12Device
        {
            get
            {
                return m_Dx12Device;
            }
        }
        public ETextureUsage Usages
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

        private Dx12Device m_Dx12Device;
        private ETextureUsage m_Usages;
        private ID3D12Resource* m_NativeResource;

        public Dx12Texture(Dx12Device device, in RHITextureCreateInfo createInfo)
        {
            m_Dx12Device = device;
            m_Usages = createInfo.usages;

            D3D12_HEAP_PROPERTIES heapProperties = new D3D12_HEAP_PROPERTIES(/*Dx12Utility.GetDX12HeapType(createInfo.usages)*/D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT);
            D3D12_RESOURCE_DESC textureDesc = new D3D12_RESOURCE_DESC();
            textureDesc.MipLevels = (ushort)createInfo.mipLevels;
            textureDesc.Format = /*Dx12Utility.GetNativeFormat(createInfo->format)*/DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS;
            textureDesc.Width = (ulong)createInfo.extent.x;
            textureDesc.Height = (uint)createInfo.extent.y;
            textureDesc.DepthOrArraySize = (ushort)createInfo.extent.z;
            textureDesc.Flags = Dx12Utility.ConvertToDX12ResourceFlagByUsage(createInfo.usages);
            textureDesc.SampleDesc.Count = (uint)createInfo.samples.x;
            textureDesc.SampleDesc.Quality = (uint)createInfo.samples.y;
            textureDesc.Dimension = Dx12Utility.ConvertToDX12TextureDimension(createInfo.dimension);

            ID3D12Resource* dx12Resource;
            bool success = SUCCEEDED(m_Dx12Device.NativeDevice->CreateCommittedResource(&heapProperties, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &textureDesc, Dx12Utility.ConvertToDX12TextureState(createInfo.state)/*Dx12Utility.GetDX12ResourceStateByUsage(createInfo.usages)*/, null, __uuidof<ID3D12Resource>(), (void**)&dx12Resource)); ;
            Debug.Assert(success);
            m_NativeResource = dx12Resource;
        }

        public Dx12Texture(Dx12Device device, in ID3D12Resource* resource)
        {
            m_Dx12Device = device;
            m_NativeResource = resource;
            m_Usages = ETextureUsage.ColorAttachment;
        }

        public override RHITextureView CreateTextureView(in RHITextureViewCreateInfo createInfo)
        {
            return new Dx12TextureView(this, createInfo);
        }

        protected override void Release()
        {
            m_NativeResource->Release();
        }
    }
}
