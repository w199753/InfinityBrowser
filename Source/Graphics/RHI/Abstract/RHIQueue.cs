namespace Infinity.Graphics
{
    public abstract class RHIQueue : Disposal
    {
        public EQueueType Type
        {
            get
            {
                return m_Type;
            }
        }

        protected EQueueType m_Type;
        public abstract RHICommandPool CreateCommandPool();
    }
}
