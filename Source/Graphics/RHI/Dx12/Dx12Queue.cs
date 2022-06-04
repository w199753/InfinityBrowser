using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602
    internal unsafe struct Dx12CommandQueueCreateInfo
    {
        public EQueueType queueType;
        public ID3D12CommandQueue* cmdQueue;
    }

    internal unsafe class Dx12Queue : RHIQueue
    {
        public ID3D12CommandQueue* NativeCommandQueue
        {
            get
            {
                return m_NativeCommandQueue;
            }
        }

        private Dx12Device m_Dx12Device;
        private EQueueType m_QueueType;
        private ID3D12CommandQueue* m_NativeCommandQueue;

        public Dx12Queue(Dx12Device device, in Dx12CommandQueueCreateInfo createInfo)
        {
            m_Dx12Device = device;
            m_QueueType = createInfo.queueType;
            m_NativeCommandQueue = createInfo.cmdQueue;
        }

        public override RHICommandPool CreateCommandPool()
        {
            return new Dx12CommandPool(m_Dx12Device, m_QueueType);
        }

        public override void Submit(RHICommandBuffer cmdBuffer, RHIFence fence)
        {
            if (cmdBuffer != null)
            {
                Dx12CommandBuffer dx12CommandBuffer = cmdBuffer as Dx12CommandBuffer;
                ID3D12CommandList** ppCommandLists = stackalloc ID3D12CommandList*[1] { (ID3D12CommandList*)dx12CommandBuffer.NativeCommandList };
                m_NativeCommandQueue->ExecuteCommandLists(1, ppCommandLists);
            }

            if (fence != null)
            {
                Dx12Fence dx12Fence = fence as Dx12Fence;
                dx12Fence.Reset();
                m_NativeCommandQueue->Signal(dx12Fence.NativeFence, 1);
            }
        }

        protected override void Release()
        {
            m_NativeCommandQueue->Release();
        }
    }
#pragma warning restore CS8600, CS8602
}
