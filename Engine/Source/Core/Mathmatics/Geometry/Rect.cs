using System;

namespace InfinityEngine.Mathmatics.Geometry
{
    public struct Rect : IEquatable<Rect>
    {
        public int left;

        public int top;

        public int right;

        public int bottom;

        public Rect(int Left, int Top, int Right, int Bottom)
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
