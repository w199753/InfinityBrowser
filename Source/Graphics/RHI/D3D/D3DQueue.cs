using System.Diagnostics;
using TerraFX.Interop.DirectX;

namespace Infinity.Graphics
{
#pragma warning disable CS8600, CS8602
    internal unsafe struct D3DCommandQueueCreateInfo
    {
        public EQueueType queueType;
        public ID3D12CommandQueue* cmdQueue;
    }

    internal unsafe class D3DQueue : RHIQueue
    {
        public ID3D12CommandQueue* NativeCommandQueue
        {
            get
            {
                return m_NativeCommandQueue;
            }
        }

        private D3DDevice m_D3DDevice;
        private EQueueType m_QueueType;
        private ID3D12CommandQueue* m_NativeCommandQueue;

        public D3DQueue(D3DDevice device, in D3DCommandQueueCreateInfo createInfo)
        {
            m_D3DDevice = device;
            m_QueueType = createInfo.queueType;
            m_NativeCommandQueue = createInfo.cmdQueue;
        }

        public override RHICommandPool CreateCommandPool()
        {
            return new D3DCommandPool(m_D3DDevice, m_QueueType);
        }

        public override void Submit(RHICommandBuffer cmdBuffer, RHIFence fence)
        {
            if (cmdBuffer != null)
            {
                D3DCommandBuffer d3dCommandBuffer = cmdBuffer as D3DCommandBuffer;
                ID3D12CommandList** ppCommandLists = stackalloc ID3D12CommandList*[1] { (ID3D12CommandList*)d3dCommandBuffer.NativeCommandList };
                m_NativeCommandQueue->ExecuteCommandLists(1, ppCommandLists);
            }

            if (fence != null)
            {
                D3DFence d3dFence = fence as D3DFence;
                d3dFence.Reset();
                m_NativeCommandQueue->Signal(d3dFence.NativeFence, 1);
            }
        }

        protected override void Release()
        {
            m_NativeCommandQueue->Release();
        }
    }
#pragma warning restore CS8600, CS8602
}
