using System;

namespace Infinity.Graphics
{
    public struct RHIBufferCreateInfo
    {
        public int size;
        public EBufferFlag flag;
        public EBufferState state;
        public EResourceUsage usage;
    }

    public abstract class RHIBuffer : Disposal
    {
        public uint SizeInBytes
        {
            get
            {
                return m_SizeInBytes;
            }
        }

        public RHIBufferCreateInfo CreateInfo
        {
            get
            {
                return m_CreateInfo;
            }
        }

        protected uint m_SizeInBytes;
        protected RHIBufferCreateInfo m_CreateInfo;

        public abstract IntPtr Map(in int length, in int offset);
        public abstract void UnMap();
        public abstract RHIBufferView CreateBufferView(in RHIBufferViewCreateInfo createInfo);
    }
}
