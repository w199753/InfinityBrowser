using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
#pragma warning disable CA1416
    internal unsafe class D3DGPU : RHIGPU
    {
        public D3DInstance D3DInstance
        {
            get
            {
                return m_D3DInstance;
            }
        }
        public IDXGIAdapter1* DXGIAdapter
        {
            get
            {
                return m_DXGIAdapter;
            }
        }

        private D3DInstance m_D3DInstance;
        private IDXGIAdapter1* m_DXGIAdapter;

        public D3DGPU(D3DInstance instance, in IDXGIAdapter1* adapter)
        {
            m_DXGIAdapter = adapter;
            m_D3DInstance = instance;
        }

        public override RHIGpuProperty GetProperty()
        {
            DXGI_ADAPTER_DESC1 desc;
            m_DXGIAdapter->GetDesc1(&desc);

            RHIGpuProperty property = new RHIGpuProperty();
            property.vendorId = desc.VendorId;
            property.deviceId = desc.DeviceId;
            property.type = (desc.Flags & (uint)DXGI_ADAPTER_FLAG.DXGI_ADAPTER_FLAG_SOFTWARE) == 1 ? EGpuType.Software : EGpuType.Hardware;
            return property;
        }

        public override RHIDevice CreateDevice(in RHIDeviceCreateInfo createInfo)
        {
            return new D3DDevice(this, createInfo);
        }

        protected override void Release()
        {
            m_DXGIAdapter->Release();
        }
    }
#pragma warning restore CA1416
}
