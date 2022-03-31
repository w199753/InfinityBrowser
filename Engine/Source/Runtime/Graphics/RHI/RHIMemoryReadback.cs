using InfinityEngine.Core.Object;
using System.Collections.Generic;

namespace InfinityEngine.Graphics.RHI
{
    public abstract class FRHIMemoryReadback : FDisposal
    {
        protected bool m_IsReady;
        protected bool m_IsProfile;
        protected FRHIFence m_Fence;
        protected FRHIQuery m_Query;

        public string name;
        public float gpuTime;
        public bool IsReady => m_IsReady;

        internal FRHIMemoryReadback(FRHIContext context, string requestName, bool bProfiler) { }
        public abstract void EnqueueCopy(FRHIContext context, FRHIBuffer buffer);
        public abstract void GetData<T>(FRHIContext context, FRHIBuffer buffer, T[] data) where T : struct;
    }

    internal class FRHIMemoryReadbackPool : FDisposal
    {
        bool m_IsProfile;
        FRHIContext m_Context;
        Stack<FRHIMemoryReadback> m_Pooled;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Pooled.Count; } }

        public FRHIMemoryReadbackPool(FRHIContext context, bool bProfile)
        {
            m_Context = context;
            m_IsProfile = bProfile;
            m_Pooled = new Stack<FRHIMemoryReadback>();
        }

        public FRHIMemoryReadback GetTemporary(string name)
        {
            FRHIMemoryReadback gpuReadback;
            if (m_Pooled.Count == 0)
            {
                gpuReadback = m_Context.CreateMemoryReadback(name, m_IsProfile);
                countAll++;
            } else {
                gpuReadback = m_Pooled.Pop();
            }
            gpuReadback.name = name;
            return gpuReadback;
        }

        public void ReleaseTemporary(FRHIMemoryReadback gpuReadback)
        {
            m_Pooled.Push(gpuReadback);
        }

        protected override void Release()
        {
            m_Context = null;
            foreach (FRHIMemoryReadback gpuReadback in m_Pooled)
            {
                gpuReadback.Dispose();
            }
        }
    }
}
