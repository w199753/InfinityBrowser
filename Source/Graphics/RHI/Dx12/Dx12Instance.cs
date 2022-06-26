using System.Diagnostics;
using TerraFX.Interop.DirectX;
using System.Collections.Generic;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CA1416
    internal unsafe class Dx12Instance : RHIInstance
    {
        public IDXGIFactory7* DXGIFactory
        {
            get
            {
                return m_DXGIFactory;
            }
        }
        public override int GpuCount => m_GPUs.Count;
        public override ERHIBackend RHIType => ERHIBackend.DirectX12;

        private List<Dx12GPU> m_GPUs;
        private IDXGIFactory7* m_DXGIFactory;

        public Dx12Instance()
        {
            m_GPUs = new List<Dx12GPU>(4);
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

            ID3D12Debug1* debugController2;
            SUCCEEDED(debugController->QueryInterface(__uuidof<ID3D12Debug1>(), (void**)&debugController2));
            debugController2->SetEnableGPUBasedValidation(true);
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
                m_GPUs.Add(new Dx12GPU(this, adapter));
                adapter = null;
            }
        }

        public override RHIGPU GetGpu(in int index)
        {
            return m_GPUs[index];
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
