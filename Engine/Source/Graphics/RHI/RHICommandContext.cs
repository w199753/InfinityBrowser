using System.Threading;
using InfinityEngine.Core.Object;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI
{
    internal abstract class FRHICommandContext : FDisposal
    {
        EContextType contextType;
        protected AutoResetEvent m_FenceEvent;

        internal FRHICommandContext(FRHIDevice device, EContextType contextType, string name) 
        { 
            this.contextType = contextType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SignalQueue(FRHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void WaitQueue(FRHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ExecuteQueue(FRHICommandBuffer cmdBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Flush();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void AsyncFlush();
    }
}
