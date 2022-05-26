namespace Infinity.Graphics
{
    public abstract class RHIQueue : Disposal
    {
        public abstract RHICommandPool CreateCommandPool();
        public abstract void Submit(RHICommandBuffer cmdBuffer, RHIFence fence);
    }
}
