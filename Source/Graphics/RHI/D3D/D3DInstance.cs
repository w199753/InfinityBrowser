using System.Diagnostics;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CA1416
    internal unsafe class D3DInstance : RHIInstance
    {
        public IDXGIFactory7* DXGIFactory
        {
            get
            {
                return m_DXGIFactory;
            }
        }
        public override int? GpuCount => m_GPUs?.Count;
        public override ERHIBackend RHIType => ERHIBackend.DirectX12;

        private List<D3DGPU>? m_GPUs;
        private IDXGIFactory7* m_DXGIFactory;

        public D3DInstance()
        {
            m_GPUs = new List<D3DGPU>(4);
            CreateDX12Factory();
            EnumerateAdapters();
        }

        private void CreateDX12Factory()
        {
            uint factoryFlags = 0;

#if BUILD_CONFIG_DEBUG
            ID3D12Debug* debugController;
            if (SUCCEEDED(DirectX.D3D12GetDebugInterface(__uuidof<ID3D12Debug>(), (void**)&debugController))) 
            {
                debugController->EnableDebugLayer();
                factoryFlags |= DXGI.DXGI_CREATE_FACTORY_DEBUG;
            }
#endif

            IDXGIFactory7* factory;
            bool success =  SUCCEEDED(DirectX.CreateDXGIFactory2(factoryFlags, __uuidof<IDXGIFactory7>(), (void**)&factory));
            Debug.Assert(success);

            m_DXGIFactory = factory;
        }

        private void EnumerateAdapters()
        {
            IDXGIAdapter1* adapter = null;

            for (uint i = 0; SUCCEEDED(m_DXGIFactory->EnumAdapters1(i, &adapter)); ++i)
            {
                m_GPUs?.Add(new D3DGPU(this, adapter));
                adapter = null;
            }
        }

        public override RHIGPU? GetGpu(in int index)
        {
            return m_GPUs?[index];
        }

        protected override void Release()
        {
            DXGIFactory->Release();

            for(int i = 0; i < m_GPUs?.Count; ++i)
            {
                m_GPUs?[i].Dispose();
            }
        }
    }
#pragma warning restore CA1416
}
