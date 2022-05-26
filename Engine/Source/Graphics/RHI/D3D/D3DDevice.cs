using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
    internal unsafe class D3DDevice : RHIDevice
    {
        internal ID3D12Device6* nativeDevice;
        //internal IDXGIAdapter1* nativeAdapter;
        internal IDXGIFactory7* nativeFactory;

        public D3DDevice()
        {
            IDXGIFactory7* factory;
            DirectX.CreateDXGIFactory2(0, Windows.__uuidof<IDXGIFactory7>(), (void**)&factory);
            nativeFactory = factory;

            //IDXGIAdapter1* adapter;
            //factory->EnumAdapters1(0, &adapter);
            //nativeAdapter = adapter;

            ID3D12Device6* device;
            DirectX.D3D12CreateDevice(null/*(IUnknown*)adapter*/, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_1, Windows.__uuidof<ID3D12Device6>(), (void**)&device);
            nativeDevice = device;
        }

        protected override void Release()
        {
            nativeDevice->Release();
            //nativeAdapter->Release();
            nativeFactory->Release();
        }

        public static implicit operator ID3D12Device6*(D3DDevice device) { return device.nativeDevice; }
    }
}
