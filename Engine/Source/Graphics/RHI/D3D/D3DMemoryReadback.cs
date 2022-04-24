using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI.D3D
{
    public class FD3DMemoryReadback : FRHIMemoryReadback
    {
        internal FD3DMemoryReadback(FRHIContext context, string requestName, bool bProfiler) : base(context, requestName, bProfiler)
        {
            name = requestName;
            gpuTime = -1;
            m_IsReady = true;
            m_IsProfile = bProfiler;
            m_Fence = context.GetFence(requestName);
            m_Query = bProfiler == true ? context.CreateQuery(EQueryType.CopyTimestamp, requestName) : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void EnqueueCopy(FRHIContext context, FRHIBuffer buffer)
        {
            if (m_IsReady && context.copyQueueState)
            {
                FRHICommandBuffer cmdBuffer = context.GetCommandBuffer(EContextType.Copy, name);
                cmdBuffer.Clear();

                if (m_IsProfile) 
                {
                    cmdBuffer.BeginEvent(name);
                    cmdBuffer.BeginQuery(m_Query);
                    buffer.Readback(cmdBuffer);
                    cmdBuffer.EndQuery(m_Query);
                    cmdBuffer.EndEvent();
                } else {
                    buffer.Readback(cmdBuffer);
                }

                context.ExecuteCommandBuffer(cmdBuffer);
                context.ReleaseCommandBuffer(cmdBuffer);
                context.WriteToFence(EContextType.Copy, m_Fence);
                //context.WaitForFence(EContextType.Graphics, m_Fence);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void GetData<T>(FRHIContext context, FRHIBuffer buffer, T[] data) where T : struct
        {
            if (m_IsReady = m_Fence.IsCompleted)
            {
                buffer.GetData(data);
                gpuTime = m_IsProfile ? m_Query.GetResult(context.copyFrequency) : -1;
            }
        }

        protected override void Release()
        {
            m_Fence?.Dispose();
            m_Query?.Dispose();
        }
    }
}
