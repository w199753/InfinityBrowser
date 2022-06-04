using System.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602, CS8604, CS8618, CA1416
    internal unsafe class Dx12CommandPool : RHICommandPool
    {
        public Dx12Device Dx12Device
        {
            get
            {
                return m_Dx12Device;
            }
        }
        public ID3D12CommandAllocator* NativeCommandAllocator
        {
            get
            {
                return m_NativeCommandAllocator;
            }
        }

        private Dx12Device m_Dx12Device;
        private ID3D12CommandAllocator* m_NativeCommandAllocator;

        public Dx12CommandPool(Dx12Device device, in EQueueType type)
        {
            m_Type = type;
            m_Dx12Device = device;

            ID3D12CommandAllocator* commandAllocator;
            bool success = SUCCEEDED(m_Dx12Device.NativeDevice->CreateCommandAllocator(Dx12Utility.ConvertToNativeQueueType(type), __uuidof<ID3D12CommandAllocator>(), (void**)&commandAllocator));
            Debug.Assert(success);
            m_NativeCommandAllocator = commandAllocator;
        }

        public override RHICommandBuffer CreateCommandBuffer()
        {
            return new Dx12CommandBuffer(m_Dx12Device, this);
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
