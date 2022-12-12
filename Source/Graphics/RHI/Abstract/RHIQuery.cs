using System;
using Infinity.Core;
using Infinity.Mathmatics;

namespace Infinity.Graphics
{
    public struct RHIQueryDescription : IEquatable<RHIQueryDescription>
    {
        public uint Count;
        public EQueryType Type;

        public bool Equals(RHIQueryDescription other) => (Type == other.Type) && (Count == other.Count);

        public override bool Equals(object? obj)
        {
            return (obj != null) ? Equals((RHIQueryDescription)obj) : false;
        }

        public override int GetHashCode() => new uint2(Count, (uint)Type).GetHashCode();

        public static bool operator == (in RHIQueryDescription value1, in RHIQueryDescription value2) => value1.Equals(value2);

        public static bool operator != (in RHIQueryDescription value1, in RHIQueryDescription value2) => !value1.Equals(value2);
    }

    public abstract class RHIQuery : Disposal
    {
        public abstract bool ResolveData(RHIBlitEncoder blotEncoder);
        public abstract bool ReadData(in uint startIndex, in uint count, in Span<ulong> results);
    }
}