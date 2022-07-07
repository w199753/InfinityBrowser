using System;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHIQueryDescription : IEquatable<RHIQueryDescription>
    {
        public uint count;
        public EQueryType type;

        public bool Equals(RHIQueryDescription other) => (type == other.type) && (count == other.count);

        public override bool Equals(object? obj)
        {
            return (obj != null) ? Equals((RHIQueryDescription)obj) : false;
        }

        public override int GetHashCode() => new uint2(count, (uint)type).GetHashCode();

        public static bool operator == (in RHIQueryDescription value1, in RHIQueryDescription value2) => value1.Equals(value2);

        public static bool operator != (in RHIQueryDescription value1, in RHIQueryDescription value2) => !value1.Equals(value2);
    }

    public abstract class RHIQuery : Disposal
    {

    }
}