using System;

namespace Maths
{
    public struct Vector3 : IEquatable<Vector3>
    {
        public double x;
        public double y;
        public double z;

        public Vector3(double x, double y) : this(x, y, 0.0) { }

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;

                    case 1:
                        return y;

                    case 2:
                        return z;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;

                    case 1:
                        y = value;
                        break;

                    case 2:
                        z = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static Vector3 zero
        {
            get
            {
                return new Vector3(0.0, 0.0, 0.0);
            }
        }

        public double magnitude
        {
            get
            {
                return Math.Sqrt(sqrMagnitude);
            }
        }

        public Vector3 normalized
        {
            get
            {
                double m = magnitude;
                if (magnitude == 0.0)
                {
                    return zero;
                }
                else
                {
                    return this / m;
                }
            }
        }

        public double sqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            double x = a.y * b.z - a.z * b.y;
            double y = a.z * b.x - a.x * b.z;
            double z = a.x * b.y - a.y * b.x;

            return new Vector3(x, y, z);
        }

        public static double Distance(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude;
        }

        public static double Dot(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, double t)
        {
            return LerpUnclamped(a, b, Extensions.Clamp01(t));
        }

        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, double t)
        {
            return a + t * (b - a);
        }

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            double x = Math.Max(a.x, b.x);
            double y = Math.Max(a.y, b.y);
            double z = Math.Max(a.z, b.z);

            return new Vector3(x, y, z);
        }

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            double x = Math.Min(a.x, b.x);
            double y = Math.Min(a.y, b.y);
            double z = Math.Min(a.z, b.z);

            return new Vector3(x, y, z);
        }

        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                return Equals((Vector3)obj);
            }
            return false;
        }

        public bool Equals(Vector3 v)
        {
            return x == v.x && y == v.y && z == v.z;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(double d, Vector3 a)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator *(Vector3 a, double d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator /(Vector3 a, double d)
        {
            if (d == 0.0)
            {
                throw new DivideByZeroException();
            }
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !a.Equals(b);
        }
    }
}
