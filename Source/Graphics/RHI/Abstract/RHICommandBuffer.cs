using System;

namespace Infinity.Graphics
{
#pragma warning disable CS8618 
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
        public RHICommandPool CommandPool
        {
            get
            {
                return m_CommandPool;
            }
        }

        protected RHICommandPool m_CommandPool;

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
        public abstract void Commit(RHIFence? fence = null);
        //public abstract void WaitUntilCompleted();
    }
#pragma warning restore CS8618
}
