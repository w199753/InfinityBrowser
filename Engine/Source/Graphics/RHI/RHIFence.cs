using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Infinity.Graphics
{
    public abstract class RHIFence : Disposal
    {
        public string name;
        public virtual bool IsCompleted => false;
        public virtual ulong CompletedValue => 0;

        internal RHIFence(RHIDevice device, string name) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void Signal(RHICommandContext cmdContext);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void WaitOnCPU(AutoResetEvent fenceEvent);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void WaitOnGPU(RHICommandContext cmdContext);
    }

    internal class RHIFencePool : Disposal
    {
        RHIContext m_Context;
        Stack<RHIFence> m_Pooled;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Pooled.Count; } }

        public RHIFencePool(RHIContext context)
        {
            m_Pooled = new Stack<RHIFence>();
            m_Context = context;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RHIFence GetTemporary(string name)
        {
            RHIFence gpuFence;
            if (m_Pooled.Count == 0)
            {
                gpuFence = m_Context.CreateFence(name);
                countAll++;
            }
            else
            {
                gpuFence = m_Pooled.Pop();
            }
            gpuFence.name = name;
            return gpuFence;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseTemporary(RHIFence gpuFence)
        {
            m_Pooled.Push(gpuFence);
        }

        protected override void Release()
        {
            m_Context = null;
            foreach (RHIFence gpuFence in m_Pooled)
            {
                gpuFence.Dispose();
            }
        }
    }
}
