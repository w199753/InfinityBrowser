namespace Infinity.Graphics
{
    public struct RHIQueueDescriptor
    {
        public uint count;
        public EQueueType type;
    }

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
