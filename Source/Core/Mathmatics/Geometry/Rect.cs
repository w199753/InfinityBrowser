using System;

namespace Infinity.Mathmatics
{
    public struct Rect : IEquatable<Rect>
    {
        public uint left;

        public uint top;

        public uint right;

        public uint bottom;

        public Rect(in uint Left, in uint Top, in uint Right, in uint Bottom)
        {
            left = Left;
            top = Top;
            right = Right;
            bottom = Bottom;
        }

        public static bool operator ==(in Rect l, in Rect r)
        {
            if (l.left == r.left && l.top == r.top && l.right == r.right)
            {
                return l.bottom == r.bottom;
            }

            return false;
        }

        public static bool operator !=(in Rect l, in Rect r)
        {
            return !(l == r);
        }

        public override bool Equals(object obj)
        {
            if (obj is Rect)
            {
                Rect other = (Rect)obj;
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Rect other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(left, top, right, bottom);
        }
    }
}
