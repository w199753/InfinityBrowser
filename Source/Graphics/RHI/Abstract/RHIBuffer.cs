using System;

namespace Infinity.Graphics
{
    public struct RHIBufferCreateInfo
    {
        public int size;
        public EBufferUsageFlags usages;
    }

    public abstract class RHIBuffer : Disposal
    {
        public abstract IntPtr Map(in EMapMode mapMode, in int offset, in int length);
        public abstract void UnMap();
        public abstract RHIBufferView CreateBufferView(in RHIBufferViewCreateInfo createInfo);
    }
}
