namespace Infinity.Graphics
{
    public abstract class RHICommandPool : Disposal
    {
        public EQueueType Type
        {
            get
            {
                return m_Type;
            }
        }

        protected EQueueType m_Type;

        public abstract void Reset();
        public abstract RHICommandBuffer CreateCommandBuffer();
    }
}
