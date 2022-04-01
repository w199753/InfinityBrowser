﻿using System.Threading;
using System.Collections.Generic;
using InfinityEngine.Core.Object;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI
{
    public abstract class FRHIFence : FDisposal
    {
        public string name;
        public virtual bool IsCompleted => false;
        public virtual ulong CompletedValue => 0;

        internal FRHIFence(FRHIDevice device, string name) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void Signal(FRHICommandContext cmdContext);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void WaitOnCPU(AutoResetEvent fenceEvent);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void WaitOnGPU(FRHICommandContext cmdContext);
    }

    internal class FRHIFencePool : FDisposal
    {
        FRHIContext m_Context;
        Stack<FRHIFence> m_Pooled;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Pooled.Count; } }

        public FRHIFencePool(FRHIContext context)
        {
            m_Pooled = new Stack<FRHIFence>();
            m_Context = context;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FRHIFence GetTemporary(string name)
        {
            FRHIFence gpuFence;
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
        public void ReleaseTemporary(FRHIFence gpuFence)
        {
            m_Pooled.Push(gpuFence);
        }

        protected override void Release()
        {
            m_Context = null;
            foreach (FRHIFence gpuFence in m_Pooled)
            {
                gpuFence.Dispose();
            }
        }
    }
}
