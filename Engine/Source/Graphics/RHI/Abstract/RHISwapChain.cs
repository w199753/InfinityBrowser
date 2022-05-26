using System;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHISwapChainCreateInfo
    {
        public int count;
        public int2 extent;
        public IntPtr window;
        public EPixelFormat format;
        public EPresentMode presentMode;
        public RHIQueue presentQueue;
    }

    public abstract class RHISwapChain : Disposal
    {
        public abstract RHITexture GetTexture(in int index);
        public abstract int GetBackBufferIndex();
        public abstract void Present();
    }
}
