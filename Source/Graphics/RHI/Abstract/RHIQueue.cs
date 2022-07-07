using System;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHIQueueDescriptor : IEquatable<RHIQueueDescriptor>
    {
        public uint count;
        public EQueueType type;

        public bool Equals(RHIQueueDescriptor other) => (type == other.type) && (count == other.count);

        public override bool Equals(object? obj)
        {
            return (obj != null) ? Equals((RHIQueueDescriptor)obj) : false;
        }

        public override int GetHashCode() => new uint2(count, (uint)type).GetHashCode();

        public static bool operator ==(in RHIQueueDescriptor value1, in RHIQueueDescriptor value2) => value1.Equals(value2);

        public static bool operator !=(in RHIQueueDescriptor value1, in RHIQueueDescriptor value2) => !value1.Equals(value2);
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
