namespace Infinity.Graphics
{
    public abstract class RHICommandPool : Disposal
    {
        public abstract void Reset();
        public abstract RHICommandBuffer CreateCommandBuffer();
    }
}
