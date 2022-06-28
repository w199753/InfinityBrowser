using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602
    internal unsafe struct Dx12CommandQueueDescriptor
    {
        public EQueueType type;
        public ID3D12CommandQueue* queue;
    }

    internal unsafe class Dx12Queue : RHIQueue
    {
        public Dx12Device Dx12Device
        {
            get
            {
                return m_Dx12Device;
            }
        }
        public ID3D12CommandQueue* NativeCommandQueue
        {
            get
            {
                return m_NativeCommandQueue;
            }
        }

        private Dx12Device m_Dx12Device;
        private ID3D12CommandQueue* m_NativeCommandQueue;

        public Dx12Queue(Dx12Device device, in Dx12CommandQueueDescriptor descriptor)
        {
            m_Type = descriptor.type;
            m_Dx12Device = device;
            m_NativeCommandQueue = descriptor.queue;
        }

        public override RHICommandPool CreateCommandPool()
        {
            return new Dx12CommandPool(this);
        }

        protected override void Release()
        {
            m_NativeCommandQueue->Release();
        }
    }
#pragma warning restore CS8600, CS8602
}
