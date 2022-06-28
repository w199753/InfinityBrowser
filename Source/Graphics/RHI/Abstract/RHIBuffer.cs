using System;

namespace Infinity.Graphics
{
    public struct RHIBufferDescriptor
    {
        public int size;
        public EBufferState state;
        public EBufferUsage usage;
        public EStorageMode storageMode;
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

        public RHIBufferDescriptor Descriptor
        {
            get
            {
                return m_Descriptor;
            }
        }

        protected uint m_SizeInBytes;
        protected RHIBufferDescriptor m_Descriptor;

        public abstract IntPtr Map(in int length, in int offset);
        public abstract void UnMap();
        public abstract RHIBufferView CreateBufferView(in RHIBufferViewDescriptor descriptor);
    }
}
