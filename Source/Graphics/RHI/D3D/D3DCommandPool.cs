using System.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal unsafe class D3DCommandPool : RHICommandPool
    {
        public D3DDevice D3DDevice
        {
            get
            {
                return m_D3DDevice;
            }
        }
        public ID3D12CommandAllocator* NativeCommandAllocator
        {
            get
            {
                return m_NativeCommandAllocator;
            }
        }

        private D3DDevice m_D3DDevice;
        private ID3D12CommandAllocator* m_NativeCommandAllocator;

        public D3DCommandPool(D3DDevice device, in EQueueType type)
        {
            m_Type = type;
            m_D3DDevice = device;

            ID3D12CommandAllocator* commandAllocator;
            bool success = SUCCEEDED(m_D3DDevice.NativeDevice->CreateCommandAllocator(D3DUtility.ConvertToNativeQueueType(type), __uuidof<ID3D12CommandAllocator>(), (void**)&commandAllocator));
            Debug.Assert(success);
            m_NativeCommandAllocator = commandAllocator;
        }

        public override RHICommandBuffer CreateCommandBuffer()
        {
            return new D3DCommandBuffer(m_D3DDevice, this);
        }

        public override void Reset()
        {
            m_NativeCommandAllocator->Reset();
        }

        protected override void Release()
        {
            m_NativeCommandAllocator->Release();
        }
    }
#pragma warning restore CS8600, CS8602, CS8604, CS8618, CA1416
}
