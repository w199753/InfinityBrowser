using System;
using System.Diagnostics;
using Infinity.Mathmatics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CA1416, CS8602, CS8604
    internal unsafe class Dx12SwapChain : RHISwapChain
    {
        public override int BackBufferIndex
        {
            get
            {
                return (int)m_NativeSwapChain->GetCurrentBackBufferIndex();
            }
        }

        private int m_Count;
        private Dx12Device m_Dx12Device;
        private EPresentMode m_PresentMode;
        private Dx12Texture[] m_Textures;
        private IDXGISwapChain4* m_NativeSwapChain;

        public Dx12SwapChain(Dx12Device device, in RHISwapChainCreateInfo createInfo)
        {
            m_Dx12Device = device;
            m_Count = createInfo.count;
            m_PresentMode = createInfo.presentMode;

            m_Textures = new Dx12Texture[m_Count];
            CreateDX12SwapChain(createInfo);
            FetchTextures(createInfo);
        }

        public override RHITexture GetTexture(in int index)
        {
            return m_Textures[index];
        }

        public override void Resize(in int2 extent)
        {
            throw new NotImplementedException();
        }

        public override void Present()
        {
            m_NativeSwapChain->Present(Dx12Utility.ConvertToDx12SyncInterval(m_PresentMode), 0);
        }

        private void CreateDX12SwapChain(in RHISwapChainCreateInfo createInfo) 
        {
            Dx12Queue dx12Queue = (Dx12Queue)createInfo.presentQueue;
            Dx12Instance dx12Instance = m_Dx12Device.Dx12Gpu.Dx12Instance;

            DXGI_SWAP_CHAIN_DESC1 desc = new DXGI_SWAP_CHAIN_DESC1();
            desc.BufferCount = (uint)createInfo.count;
            desc.Width = (uint)createInfo.extent.x;
            desc.Height = (uint)createInfo.extent.y;
            desc.Format = /*Dx12Utility.ConvertToDx12Format(createInfo.format)*/DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
            desc.BufferUsage = createInfo.frameBufferOnly ? DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT : (DXGI.DXGI_USAGE_SHADER_INPUT | DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT);
            desc.SwapEffect = /*Dx12Utility.GetDx12SwapEffect(createInfo.presentMode)*/ DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD;
            desc.SampleDesc = new DXGI_SAMPLE_DESC(1, 0);

            IDXGISwapChain1* dx12SwapChain1;
            bool success = SUCCEEDED(dx12Instance.DXGIFactory->CreateSwapChainForHwnd((IUnknown*)dx12Queue.NativeCommandQueue, new HWND(createInfo.window.ToPointer()), &desc, null, null, &dx12SwapChain1));
            Debug.Assert(success);
            m_NativeSwapChain = (IDXGISwapChain4*)dx12SwapChain1;
        }

        private void FetchTextures(in RHISwapChainCreateInfo createInfo)
        {
            RHITextureCreateInfo textureCreateInfo;
            {
                textureCreateInfo.extent = new int3(createInfo.extent.xy, 1);
                textureCreateInfo.samples = 1;
                textureCreateInfo.mipLevels = 1;
                textureCreateInfo.format = createInfo.format;
                textureCreateInfo.dimension = ETextureDimension.Tex2D;
                textureCreateInfo.state = ETextureState.Present;
                textureCreateInfo.usage = ETextureUsage.RenderTarget;
                textureCreateInfo.storageMode = EStorageMode.Default;
            }

            for (int i = 0; i < m_Count; ++i)
            {
                ID3D12Resource* dx12Resource = null;
                bool success = SUCCEEDED(m_NativeSwapChain->GetBuffer((uint)i, __uuidof<ID3D12Resource>(), (void**)&dx12Resource));
                Debug.Assert(success);
                m_Textures[i] = new Dx12Texture(m_Dx12Device, textureCreateInfo, dx12Resource);
            }
        }

        protected override void Release()
        {
            m_NativeSwapChain->Release();
        }
    }
#pragma warning restore CS8600, CS8602, CA1416, CS8602, CS8604
}
