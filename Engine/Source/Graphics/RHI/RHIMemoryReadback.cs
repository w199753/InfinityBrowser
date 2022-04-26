using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI
{
    public abstract class RHIMemoryReadback : Disposal
    {
        protected bool m_IsReady;
        protected bool m_IsProfile;
        protected RHIFence m_Fence;
        protected RHIQuery m_Query;

        public string name;
        public float gpuTime;
        public bool IsReady => m_IsReady;

        internal RHIMemoryReadback(RHIContext context, string requestName, bool bProfiler) { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void EnqueueCopy(RHIContext context, RHIBuffer buffer);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void GetData<T>(RHIContext context, RHIBuffer buffer, T[] data) where T : struct;
    }

    internal class RHIMemoryReadbackPool : Disposal
    {
        bool m_IsProfile;
        RHIContext m_Context;
        Stack<RHIMemoryReadback> m_Pooled;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Pooled.Count; } }

        public RHIMemoryReadbackPool(RHIContext context, bool bProfile)
        {
            m_Context = context;
            m_IsProfile = bProfile;
            m_Pooled = new Stack<RHIMemoryReadback>();
        }

        public RHIMemoryReadback GetTemporary(string name)
        {
            RHIMemoryReadback gpuReadback;
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

        public void ReleaseTemporary(RHIMemoryReadback gpuReadback)
        {
            m_Pooled.Push(gpuReadback);
        }

        protected override void Release()
        {
            m_Context = null;
            foreach (RHIMemoryReadback gpuReadback in m_Pooled)
            {
                gpuReadback.Dispose();
            }
        }
    }
}
