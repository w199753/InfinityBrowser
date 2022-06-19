using System;

namespace Infinity.Graphics
{
    public struct RHIBufferCreateInfo
    {
        public int size;
        public EBufferState state;
        public EBufferUsage usages;
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

        protected uint m_SizeInBytes;

        public abstract IntPtr Map(in EMapMode mapMode, in int offset, in int length);
        public abstract void UnMap();
        public abstract RHIBufferView CreateBufferView(in RHIBufferViewCreateInfo createInfo);
    }
}
