using System;

namespace Infinity.Mathmatics
{
    public struct Viewport : IEquatable<Viewport>
    {
        public uint TopLeftX;

        public uint TopLeftY;

        public uint Width;

        public uint Height;

        public float MinDepth;

        public float MaxDepth;

        public static bool operator ==(in Viewport l, in Viewport r)
        {
            if (l.TopLeftX == r.TopLeftX && l.TopLeftY == r.TopLeftY && l.Width == r.Width && l.Height == r.Height && l.MinDepth == r.MinDepth)
            {
                return l.MaxDepth == r.MaxDepth;
            }

            return false;
        }

        public static bool operator !=(in Viewport l, in Viewport r)
        {
            return !(l == r);
        }

        public Viewport(in uint topLeftX, in uint topLeftY, in uint width, in uint height, in float minDepth = 0f, in float maxDepth = 1f)
        {
            TopLeftX = topLeftX;
            TopLeftY = topLeftY;
            Width = width;
            Height = height;
            MinDepth = minDepth;
            MaxDepth = maxDepth;
        }

        public override bool Equals(object obj)
        {
            if (obj is Viewport)
            {
                Viewport other = (Viewport)obj;
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Viewport other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TopLeftX, TopLeftY, Width, Height, MinDepth, MaxDepth);
        }
    }
}
