using System;

namespace Infinity.Graphics
{
    public struct RHIScopedCommandBufferRef : IDisposable
    {
        RHICommandBuffer m_CommandBuffer;

        internal RHIScopedCommandBufferRef(RHICommandBuffer commandBuffer)
        {
            m_CommandBuffer = commandBuffer;
        }

        public void Dispose()
        {
            m_CommandBuffer.End();
        }
    }

    public abstract class RHICommandBuffer : Disposal
    {
        public EQueueType QueueType => m_QueueType;

        protected EQueueType m_QueueType;

        public RHIScopedCommandBufferRef BeginScoped()
        {
            Begin();
            return new RHIScopedCommandBufferRef(this);
        }

        public abstract void Begin();
        public abstract RHIBlitEncoder GetBlitEncoder();
        public abstract RHIComputeEncoder GetComputeEncoder();
        public abstract RHIGraphicsEncoder GetGraphicsEncoder();
        public abstract void End();
        //public abstract void Commit();
        //public abstract void WaitUntilCompleted();
    }
}
