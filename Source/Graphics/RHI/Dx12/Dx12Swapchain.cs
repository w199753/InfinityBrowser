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
        private Dx12Texture[] m_Textures;
        private IDXGISwapChain4* m_NativeSwapChain;

        public Dx12SwapChain(Dx12Device device, in RHISwapChainDescriptor descriptor)
        {
            m_Dx12Device = device;
            m_Count = descriptor.count;

            m_Textures = new Dx12Texture[m_Count];
            CreateDX12SwapChain(descriptor);
            FetchTextures(descriptor);
        }

        public override RHITexture GetTexture(in int index)
        {
            return m_Textures[index];
        }

        public override void Resize(in int2 extent)
        {
            throw new NotImplementedException();
        }

        public override void Present(EPresentMode presentMode)
        {
            m_NativeSwapChain->Present(Dx12Utility.ConvertToDx12SyncInterval(presentMode), 0);
        }

        private void CreateDX12SwapChain(in RHISwapChainDescriptor descriptor) 
        {
            Dx12Queue dx12Queue = (Dx12Queue)descriptor.presentQueue;
            Dx12Instance dx12Instance = m_Dx12Device.Dx12Gpu.Dx12Instance;

            DXGI_SWAP_CHAIN_DESC1 desc = new DXGI_SWAP_CHAIN_DESC1();
            desc.BufferCount = (uint)descriptor.count;
            desc.Width = (uint)descriptor.extent.x;
            desc.Height = (uint)descriptor.extent.y;
            desc.Format = /*Dx12Utility.ConvertToDx12Format(descriptor.format)*/DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
            desc.BufferUsage = descriptor.frameBufferOnly ? DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT : (DXGI.DXGI_USAGE_SHADER_INPUT | DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT);
            desc.SwapEffect = /*Dx12Utility.ConvertToDx12SwapEffect(descriptor.presentMode)*/ DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD;
            desc.SampleDesc = new DXGI_SAMPLE_DESC(1, 0);

            IDXGISwapChain1* dx12SwapChain1;
            bool success = SUCCEEDED(dx12Instance.DXGIFactory->CreateSwapChainForHwnd((IUnknown*)dx12Queue.NativeCommandQueue, new HWND(descriptor.window.ToPointer()), &desc, null, null, &dx12SwapChain1));
            Debug.Assert(success);
            m_NativeSwapChain = (IDXGISwapChain4*)dx12SwapChain1;
        }

        private void FetchTextures(in RHISwapChainDescriptor descriptor)
        {
            RHITextureDescriptor textureDescriptor;
            {
                textureDescriptor.extent = new int3(descriptor.extent.xy, 1);
                textureDescriptor.samples = 1;
                textureDescriptor.mipCount = 1;
                textureDescriptor.format = descriptor.format;
                textureDescriptor.state = ETextureState.Present;
                textureDescriptor.usage = ETextureUsage.RenderTarget;
                textureDescriptor.dimension = ETextureDimension.Texture2D;
                textureDescriptor.storageMode = EStorageMode.Default;
            }

            for (int i = 0; i < m_Count; ++i)
            {
                ID3D12Resource* dx12Resource = null;
                bool success = SUCCEEDED(m_NativeSwapChain->GetBuffer((uint)i, __uuidof<ID3D12Resource>(), (void**)&dx12Resource));
                Debug.Assert(success);
                m_Textures[i] = new Dx12Texture(m_Dx12Device, textureDescriptor, dx12Resource);
            }
        }

        protected override void Release()
        {
            m_NativeSwapChain->Release();
        }
    }
#pragma warning restore CS8600, CS8602, CA1416, CS8602, CS8604
}
