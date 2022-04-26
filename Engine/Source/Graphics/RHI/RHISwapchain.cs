using System.Runtime.CompilerServices;

namespace InfinityEngine.Graphics.RHI
{
    public unsafe abstract class RHISwapChain : Disposal
    {
        public string name;
        public virtual int swapIndex => 0;
        public RHITexture backBuffer => backBuffers[swapIndex];
        public RHIRenderTargetView backBufferView => backBufferViews[swapIndex];

        protected RHITexture[] backBuffers;
        protected RHIRenderTargetView[] backBufferViews;

        internal RHISwapChain(RHIDevice device, RHICommandContext cmdContext, in void* windowPtr, in uint width, in uint height, string name)
        {
            this.name = name;
            this.backBuffers = new RHITexture[2];
            this.backBufferViews = new RHIRenderTargetView[2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Present();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void InitResourceView(RHIContext context);
    }
}