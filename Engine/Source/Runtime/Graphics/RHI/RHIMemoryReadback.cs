﻿using InfinityEngine.Core.Object;

namespace InfinityEngine.Graphics.RHI
{
    public class FRHIMemoryReadback : FDisposable
    {
        public bool IsReady { get { return m_Fence.IsCompleted; } }

        private FRHIFence m_Fence;

        internal FRHIMemoryReadback(FRHIDevice device) : base()
        {
            m_Fence = new FRHIFence(device);
        }

        protected virtual void RequestReadback(FRHIGraphicsContext graphicsContext, FRHICommandBuffer cmdBuffer, FRHIBuffer buffer) { }

        protected virtual void RequestReadback() { }

        protected override void Release()
        {
            m_Fence?.Dispose();
        }
    }
}
