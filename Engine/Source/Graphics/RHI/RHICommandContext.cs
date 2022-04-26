using System.Threading;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics
{
    internal abstract class RHICommandContext : Disposal
    {
        EContextType contextType;
        protected AutoResetEvent m_FenceEvent;

        internal RHICommandContext(RHIDevice device, EContextType contextType, string name) 
        { 
            this.contextType = contextType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void SignalQueue(RHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void WaitQueue(RHIFence fence);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void ExecuteQueue(RHICommandBuffer cmdBuffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Flush();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void AsyncFlush();
    }
}
