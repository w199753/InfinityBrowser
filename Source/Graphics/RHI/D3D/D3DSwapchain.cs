using System.Diagnostics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CA1416, CS8602, CS8604
    internal unsafe class D3DSwapChain : RHISwapChain
    {
        public override int BackBufferIndex
        {
            get
            {
                return (int)m_NativeSwapChain->GetCurrentBackBufferIndex();
            }
        }

        private int m_Count;
        private D3DDevice m_D3DDevice;
        private EPresentMode m_PresentMode;
        private D3DTexture[] m_Textures;
        private IDXGISwapChain4* m_NativeSwapChain;

        public D3DSwapChain(D3DDevice device, in RHISwapChainCreateInfo createInfo)
        {
            m_D3DDevice = device;
            m_Count = createInfo.count;
            m_PresentMode = createInfo.presentMode;

            m_Textures = new D3DTexture[m_Count];
            CreateDX12SwapChain(createInfo);
            FetchTextures();
        }

        public override RHITexture GetTexture(in int index)
        {
            return m_Textures[index];
        }

        public override void Present()
        {
            m_NativeSwapChain->Present(D3DUtility.ConvertToNativeSyncInterval(m_PresentMode), 0);
        }

        private void FetchTextures()
        {
            for (int i = 0; i < m_Count; ++i)
            {
                ID3D12Resource* dx12Resource = null;
                bool success = SUCCEEDED(m_NativeSwapChain->GetBuffer((uint)i, __uuidof<ID3D12Resource>(), (void**)&dx12Resource));
                Debug.Assert(success);
                m_Textures[i] = new D3DTexture(m_D3DDevice, dx12Resource);
            }
        }

        private void CreateDX12SwapChain(in RHISwapChainCreateInfo createInfo) 
        {
            D3DQueue d3dQueue = (D3DQueue)createInfo.presentQueue;
            D3DInstance d3dInstance = m_D3DDevice.D3DGpu.D3DInstance;

            DXGI_SWAP_CHAIN_DESC1 desc = new DXGI_SWAP_CHAIN_DESC1();
            desc.BufferCount = (uint)createInfo.count;
            desc.Width = (uint)createInfo.extent.x;
            desc.Height = (uint)createInfo.extent.y;
            desc.Format = /*D3DUtility.GetNativeFormat(createInfo.format)*/DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
            desc.BufferUsage = DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT;
            desc.SwapEffect = /*D3DUtility.GetNativeSwapEffect(createInfo.presentMode)*/ DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD;
            desc.SampleDesc.Count = 1;

            IDXGISwapChain1* dx12SwapChain1;
            bool success = SUCCEEDED(d3dInstance.DXGIFactory->CreateSwapChainForHwnd((IUnknown*)d3dQueue.NativeCommandQueue, new HWND(createInfo.window.ToPointer()), &desc, null, null, &dx12SwapChain1));
            Debug.Assert(success);
            m_NativeSwapChain = (IDXGISwapChain4*)dx12SwapChain1;
        }

        protected override void Release()
        {
            m_NativeSwapChain->Release();

            for (int i = 0; i < m_Textures.Length; ++i)
            {
                m_Textures[i].Dispose();
            }
        }
    }
#pragma warning restore CS8600, CS8602, CA1416, CS8602, CS8604
}
