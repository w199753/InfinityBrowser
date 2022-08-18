using System;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHISwapChainDescriptor
    {
        public bool frameBufferOnly;
        public int count;
        public int2 extent;
        public IntPtr surface;
        public EPixelFormat format;
        public RHIQueue presentQueue;
    }

    public abstract class RHISwapChain : Disposal
    {
        public abstract int BackBufferIndex
        {
            get;
        }

        public abstract RHITexture GetTexture(in int index);
        public abstract void Resize(in int2 extent);
        public abstract void Present(EPresentMode presentMode);
    }
}
