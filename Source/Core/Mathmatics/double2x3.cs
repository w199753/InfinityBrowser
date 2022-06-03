//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Runtime.CompilerServices;

#pragma warning disable 0660, 0661

namespace Infinity.Mathmatics
{
    [System.Serializable]
    public partial struct double2x3 : System.IEquatable<double2x3>, IFormattable
    {
        public double2 c0;
        public double2 c1;
        public double2 c2;

        /// <summary>double2x3 zero value.</summary>
        public static readonly double2x3 zero;

        /// <summary>Constructs a double2x3 matrix from three double2 vectors.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(double2 c0, double2 c1, double2 c2)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
        }

        /// <summary>Constructs a double2x3 matrix from 6 double values given in row-major order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(double m00, double m01, double m02,
                         double m10, double m11, double m12)
        {
            this.c0 = new double2(m00, m10);
            this.c1 = new double2(m01, m11);
            this.c2 = new double2(m02, m12);
        }

        /// <summary>Constructs a double2x3 matrix from a single double value by assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(double v)
        {
            this.c0 = v;
            this.c1 = v;
            this.c2 = v;
        }

        /// <summary>Constructs a double2x3 matrix from a single bool value by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(bool v)
        {
            this.c0 = math.select(new double2(0.0), new double2(1.0), v);
            this.c1 = math.select(new double2(0.0), new double2(1.0), v);
            this.c2 = math.select(new double2(0.0), new double2(1.0), v);
        }

        /// <summary>Constructs a double2x3 matrix from a bool2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(bool2x3 v)
        {
            this.c0 = math.select(new double2(0.0), new double2(1.0), v.c0);
            this.c1 = math.select(new double2(0.0), new double2(1.0), v.c1);
            this.c2 = math.select(new double2(0.0), new double2(1.0), v.c2);
        }

        /// <summary>Constructs a double2x3 matrix from a single int value by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(int v)
        {
            this.c0 = v;
            this.c1 = v;
            this.c2 = v;
        }

        /// <summary>Constructs a double2x3 matrix from a int2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(int2x3 v)
        {
            this.c0 = v.c0;
            this.c1 = v.c1;
            this.c2 = v.c2;
        }

        /// <summary>Constructs a double2x3 matrix from a single uint value by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(uint v)
        {
            this.c0 = v;
            this.c1 = v;
            this.c2 = v;
        }

        /// <summary>Constructs a double2x3 matrix from a uint2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(uint2x3 v)
        {
            this.c0 = v.c0;
            this.c1 = v.c1;
            this.c2 = v.c2;
        }

        /// <summary>Constructs a double2x3 matrix from a single float value by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(float v)
        {
            this.c0 = v;
            this.c1 = v;
            this.c2 = v;
        }

        /// <summary>Constructs a double2x3 matrix from a float2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x3(float2x3 v)
        {
            this.c0 = v.c0;
            this.c1 = v.c1;
            this.c2 = v.c2;
        }


        /// <summary>Implicitly converts a single double value to a double2x3 matrix by assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x3(double v) { return new double2x3(v); }

        /// <summary>Explicitly converts a single bool value to a double2x3 matrix by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double2x3(bool v) { return new double2x3(v); }

        /// <summary>Explicitly converts a bool2x3 matrix to a double2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double2x3(bool2x3 v) { return new double2x3(v); }

        /// <summary>Implicitly converts a single int value to a double2x3 matrix by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x3(int v) { return new double2x3(v); }

        /// <summary>Implicitly converts a int2x3 matrix to a double2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x3(int2x3 v) { return new double2x3(v); }

        /// <summary>Implicitly converts a single uint value to a double2x3 matrix by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x3(uint v) { return new double2x3(v); }

        /// <summary>Implicitly converts a uint2x3 matrix to a double2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x3(uint2x3 v) { return new double2x3(v); }

        /// <summary>Implicitly converts a single float value to a double2x3 matrix by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x3(float v) { return new double2x3(v); }

        /// <summary>Implicitly converts a float2x3 matrix to a double2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x3(float2x3 v) { return new double2x3(v); }


        /// <summary>Returns the result of a componentwise multiplication operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator * (double2x3 lhs, double2x3 rhs) { return new double2x3 (lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2); }

        /// <summary>Returns the result of a componentwise multiplication operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator * (double2x3 lhs, double rhs) { return new double2x3 (lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs); }

        /// <summary>Returns the result of a componentwise multiplication operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator * (double lhs, double2x3 rhs) { return new double2x3 (lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2); }


        /// <summary>Returns the result of a componentwise addition operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator + (double2x3 lhs, double2x3 rhs) { return new double2x3 (lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2); }

        /// <summary>Returns the result of a componentwise addition operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator + (double2x3 lhs, double rhs) { return new double2x3 (lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs); }

        /// <summary>Returns the result of a componentwise addition operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator + (double lhs, double2x3 rhs) { return new double2x3 (lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2); }


        /// <summary>Returns the result of a componentwise subtraction operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator - (double2x3 lhs, double2x3 rhs) { return new double2x3 (lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2); }

        /// <summary>Returns the result of a componentwise subtraction operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator - (double2x3 lhs, double rhs) { return new double2x3 (lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs); }

        /// <summary>Returns the result of a componentwise subtraction operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator - (double lhs, double2x3 rhs) { return new double2x3 (lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2); }


        /// <summary>Returns the result of a componentwise division operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator / (double2x3 lhs, double2x3 rhs) { return new double2x3 (lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2); }

        /// <summary>Returns the result of a componentwise division operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator / (double2x3 lhs, double rhs) { return new double2x3 (lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs); }

        /// <summary>Returns the result of a componentwise division operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator / (double lhs, double2x3 rhs) { return new double2x3 (lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2); }


        /// <summary>Returns the result of a componentwise modulus operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator % (double2x3 lhs, double2x3 rhs) { return new double2x3 (lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2); }

        /// <summary>Returns the result of a componentwise modulus operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator % (double2x3 lhs, double rhs) { return new double2x3 (lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs); }

        /// <summary>Returns the result of a componentwise modulus operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator % (double lhs, double2x3 rhs) { return new double2x3 (lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2); }


        /// <summary>Returns the result of a componentwise increment operation on a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator ++ (double2x3 val) { return new double2x3 (++val.c0, ++val.c1, ++val.c2); }


        /// <summary>Returns the result of a componentwise decrement operation on a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator -- (double2x3 val) { return new double2x3 (--val.c0, --val.c1, --val.c2); }


        /// <summary>Returns the result of a componentwise less than operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator < (double2x3 lhs, double2x3 rhs) { return new bool2x3 (lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2); }

        /// <summary>Returns the result of a componentwise less than operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator < (double2x3 lhs, double rhs) { return new bool2x3 (lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs); }

        /// <summary>Returns the result of a componentwise less than operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator < (double lhs, double2x3 rhs) { return new bool2x3 (lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2); }


        /// <summary>Returns the result of a componentwise less or equal operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <= (double2x3 lhs, double2x3 rhs) { return new bool2x3 (lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2); }

        /// <summary>Returns the result of a componentwise less or equal operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <= (double2x3 lhs, double rhs) { return new bool2x3 (lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs); }

        /// <summary>Returns the result of a componentwise less or equal operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <= (double lhs, double2x3 rhs) { return new bool2x3 (lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2); }


        /// <summary>Returns the result of a componentwise greater than operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator > (double2x3 lhs, double2x3 rhs) { return new bool2x3 (lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2); }

        /// <summary>Returns the result of a componentwise greater than operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator > (double2x3 lhs, double rhs) { return new bool2x3 (lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs); }

        /// <summary>Returns the result of a componentwise greater than operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator > (double lhs, double2x3 rhs) { return new bool2x3 (lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2); }


        /// <summary>Returns the result of a componentwise greater or equal operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >= (double2x3 lhs, double2x3 rhs) { return new bool2x3 (lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2); }

        /// <summary>Returns the result of a componentwise greater or equal operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >= (double2x3 lhs, double rhs) { return new bool2x3 (lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs); }

        /// <summary>Returns the result of a componentwise greater or equal operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >= (double lhs, double2x3 rhs) { return new bool2x3 (lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2); }


        /// <summary>Returns the result of a componentwise unary minus operation on a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator - (double2x3 val) { return new double2x3 (-val.c0, -val.c1, -val.c2); }


        /// <summary>Returns the result of a componentwise unary plus operation on a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 operator + (double2x3 val) { return new double2x3 (+val.c0, +val.c1, +val.c2); }


        /// <summary>Returns the result of a componentwise equality operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator == (double2x3 lhs, double2x3 rhs) { return new bool2x3 (lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2); }

        /// <summary>Returns the result of a componentwise equality operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator == (double2x3 lhs, double rhs) { return new bool2x3 (lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs); }

        /// <summary>Returns the result of a componentwise equality operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator == (double lhs, double2x3 rhs) { return new bool2x3 (lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2); }


        /// <summary>Returns the result of a componentwise not equal operation on two double2x3 matrices.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator != (double2x3 lhs, double2x3 rhs) { return new bool2x3 (lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2); }

        /// <summary>Returns the result of a componentwise not equal operation on a double2x3 matrix and a double value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator != (double2x3 lhs, double rhs) { return new bool2x3 (lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs); }

        /// <summary>Returns the result of a componentwise not equal operation on a double value and a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator != (double lhs, double2x3 rhs) { return new bool2x3 (lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2); }



        /// <summary>Returns the double2 element at a specified index.</summary>
        unsafe public ref double2 this[int index]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if ((uint)index >= 3)
                    throw new System.ArgumentException("index must be between[0...2]");
#endif
                fixed (double2x3* array = &this) { return ref ((double2*)array)[index]; }
            }
        }

        /// <summary>Returns true if the double2x3 is equal to a given double2x3, false otherwise.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(double2x3 rhs) { return c0.Equals(rhs.c0) && c1.Equals(rhs.c1) && c2.Equals(rhs.c2); }

        /// <summary>Returns true if the double2x3 is equal to a given double2x3, false otherwise.</summary>
        public override bool Equals(object o) { return Equals((double2x3)o); }


        /// <summary>Returns a hash code for the double2x3.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() { return (int)math.hash(this); }


        /// <summary>Returns a string representation of the double2x3.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return string.Format("double2x3({0}, {1}, {2},  {3}, {4}, {5})", c0.x, c1.x, c2.x, c0.y, c1.y, c2.y);
        }

        /// <summary>Returns a string representation of the double2x3 using a specified format and culture-specific format information.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("double2x3({0}, {1}, {2},  {3}, {4}, {5})", c0.x.ToString(format, formatProvider), c1.x.ToString(format, formatProvider), c2.x.ToString(format, formatProvider), c0.y.ToString(format, formatProvider), c1.y.ToString(format, formatProvider), c2.y.ToString(format, formatProvider));
        }

    }

    public static partial class math
    {
        /// <summary>Returns a double2x3 matrix constructed from three double2 vectors.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(double2 c0, double2 c1, double2 c2) { return new double2x3(c0, c1, c2); }

        /// <summary>Returns a double2x3 matrix constructed from from 6 double values given in row-major order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(double m00, double m01, double m02,
                                          double m10, double m11, double m12)
        {
            return new double2x3(m00, m01, m02,
                                 m10, m11, m12);
        }

        /// <summary>Returns a double2x3 matrix constructed from a single double value by assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(double v) { return new double2x3(v); }

        /// <summary>Returns a double2x3 matrix constructed from a single bool value by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(bool v) { return new double2x3(v); }

        /// <summary>Return a double2x3 matrix constructed from a bool2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(bool2x3 v) { return new double2x3(v); }

        /// <summary>Returns a double2x3 matrix constructed from a single int value by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(int v) { return new double2x3(v); }

        /// <summary>Return a double2x3 matrix constructed from a int2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(int2x3 v) { return new double2x3(v); }

        /// <summary>Returns a double2x3 matrix constructed from a single uint value by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(uint v) { return new double2x3(v); }

        /// <summary>Return a double2x3 matrix constructed from a uint2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(uint2x3 v) { return new double2x3(v); }

        /// <summary>Returns a double2x3 matrix constructed from a single float value by converting it to double and assigning it to every component.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(float v) { return new double2x3(v); }

        /// <summary>Return a double2x3 matrix constructed from a float2x3 matrix by componentwise conversion.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 double2x3(float2x3 v) { return new double2x3(v); }

        /// <summary>Return the double3x2 transpose of a double2x3 matrix.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x2 transpose(double2x3 v)
        {
            return double3x2(
                v.c0.x, v.c0.y,
                v.c1.x, v.c1.y,
                v.c2.x, v.c2.y);
        }

        /// <summary>Returns a uint hash code of a double2x3 vector.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double2x3 v)
        {
            return csum(fold_to_uint(v.c0) * uint2(0xF25BE857u, 0x9BC17CE7u) +
                        fold_to_uint(v.c1) * uint2(0xC8B86851u, 0x64095221u) +
                        fold_to_uint(v.c2) * uint2(0xADF428FFu, 0xA3977109u)) + 0x745ED837u;
        }

        /// <summary>
        /// Returns a uint2 vector hash code of a double2x3 vector.
        /// When multiple elements are to be hashes together, it can more efficient to calculate and combine wide hash
        /// that are only reduced to a narrow uint hash at the very end instead of at every step.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 hashwide(double2x3 v)
        {
            return (fold_to_uint(v.c0) * uint2(0x9CDC88F5u, 0xFA62D721u) +
                    fold_to_uint(v.c1) * uint2(0x7E4DB1CFu, 0x68EEE0F5u) +
                    fold_to_uint(v.c2) * uint2(0xBC3B0A59u, 0x816EFB5Du)) + 0xA24E82B7u;
        }

    }
}