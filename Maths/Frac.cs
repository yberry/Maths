using System;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Maths
{
    public struct Frac : IComparable, IComparable<Frac>, IEquatable<Frac>
    {
        static Regex regex = new Regex(@"^\d+(\.\d+)? */ *\d+(\.\d+)?$");

        BigInteger num;
        BigInteger den;

        public BigInteger Num
        {
            get
            {
                return num;
            }

            set
            {
                num = value;
                if (value.IsZero)
                {
                    den = 1;
                }
                else
                {
                    Refactor();
                }
            }
        }

        public BigInteger Den
        {
            get
            {
                return den;
            }

            set
            {
                if (value.IsZero)
                {
                    throw new DivideByZeroException();
                }

                if (!num.IsZero)
                {
                    den = value;
                    Refactor();
                }
            }
        }

        public bool IsZero
        {
            get
            {
                return num.IsZero;
            }
        }

        public bool IsOne
        {
            get
            {
                return num.IsOne && den.IsOne;
            }
        }

        public bool IsInteger
        {
            get
            {
                return den.IsOne;
            }
        }

        public int Sign
        {
            get
            {
                return num.Sign;
            }
        }

        public BigInteger EntirePart
        {
            get
            {
                return num / den;
            }
        }

        public BigInteger Rest
        {
            get
            {
                return num % den;
            }
        }

        public bool MoreNumDigits
        {
            get
            {
                return num.ToString().Length > den.ToString().Length;
            }
        }

        public Frac Reverse
        {
            get
            {
                return new Frac(den, num);
            }
        }

        public static Frac zero
        {
            get
            {
                return 0;
            }
        }

        public Frac(double value)
        {
            if (value == 0.0)
            {
                num = 0;
                den = 1;
            }
            else
            {
                string str = value.ToString();
                int index = str.IndexOf('.');

                index = str.Length - index;
                den = 1;
                while (index > 0)
                {
                    value *= 10.0;
                    den *= 10;
                    index--;
                }

                num = new BigInteger(value);

                Refactor();
            }
        }

        public Frac(string str)
        {
            if (regex.IsMatch(str))
            {
                CultureInfo invariant = CultureInfo.InvariantCulture;
                string[] split = str.Split('/');
                Frac n = double.Parse(split[0], invariant);
                Frac d = double.Parse(split[1], invariant);

                Frac res = n / d;

                num = res.num;
                den = res.den;
            }
            else
            {
                throw new FormatException();
            }
        }

        public Frac(BigInteger n)
        {
            num = n;
            den = BigInteger.One;
        }

        public Frac(BigInteger n, BigInteger d)
        {
            if (d.IsZero)
            {
                throw new DivideByZeroException();
            }

            if (n.IsZero || d.IsOne)
            {
                num = n;
                den = BigInteger.One;
            }
            else
            {
                num = n;
                den = d;

                Refactor();
            }
        }

        private void Refactor()
        {
            bool negative = num < 0 ^ den < 0;

            num = BigInteger.Abs(num);
            den = BigInteger.Abs(den);

            BigInteger pgcd = BigInteger.GreatestCommonDivisor(num, den);

            num /= pgcd;
            den /= pgcd;

            if (negative)
            {
                num = -num;
            }
        }

        public static Frac Abs(Frac f)
        {
            return new Frac(BigInteger.Abs(f.num), f.den);
        }

        public static double Log(Frac f)
        {
            return BigInteger.Log(f.num) - BigInteger.Log(f.den);
        }

        public static double Log(Frac f, double baseValue)
        {
            return BigInteger.Log(f.num, baseValue) - BigInteger.Log(f.den, baseValue);
        }

        public static double Log10(Frac f)
        {
            return Log(f, 10.0);
        }

        public static Frac Max(Frac a, Frac b)
        {
            return a > b ? a : b;
        }

        public static Frac Min(Frac a, Frac b)
        {
            return a < b ? a : b;
        }

        public static Frac Clamp(Frac val, Frac min, Frac max)
        {
            if (val < min)
            {
                return min;
            }
            else if (val > max)
            {
                return max;
            }
            else
            {
                return val;
            }
        }

        public static Frac Lerp(Frac a, Frac b, Frac t)
        {
            return Clamp(a + (b - a) * t, a, b);
        }

        public static Frac LerpUnclamped(Frac a, Frac b, Frac t)
        {
            return a + (b - a) * t;
        }

        public static Frac Clamp01(Frac val)
        {
            return Clamp(val, 0, 1);
        }

        public static Frac Pow(Frac f, int exp)
        {
            BigInteger n = BigInteger.Pow(f.num, exp);
            BigInteger d = BigInteger.Pow(f.den, exp);

            return new Frac(n, d);
        }

        public static Frac Add(Frac a, Frac b)
        {
            BigInteger n = a.num * b.den + a.den * b.num;
            BigInteger d = a.den * b.den;

            return new Frac(n, d);
        }

        public static Frac Substract(Frac a, Frac b)
        {
            BigInteger n = a.num * b.den - a.den * b.num;
            BigInteger d = a.den * b.den;

            return new Frac(n, d);
        }

        public static Frac Multiply(Frac a, Frac b)
        {
            return new Frac(a.num * b.num, a.den * b.den);
        }

        public static Frac Divide(Frac a, Frac b)
        {
            return new Frac(a.num * b.den, a.den * b.num);
        }

        public int CompareTo(Frac other)
        {
            return (num * other.den).CompareTo(den * other.num);
        }

        public int CompareTo(object obj)
        {
            if (obj is Frac)
            {
                return CompareTo((Frac)obj);
            }

            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is Frac)
            {
                return Equals((Frac)obj);
            }

            return false;
        }

        public bool Equals(Frac f)
        {
            return num == f.num && den == f.den;
        }

        public override int GetHashCode()
        {
            return num.GetHashCode() ^ den.GetHashCode();
        }

        public override string ToString()
        {
            return num.ToString() + (IsInteger ? "" : "/" + den.ToString());
        }

        public static Frac operator +(Frac a, Frac b)
        {
            return Add(a, b);
        }

        public static Frac operator -(Frac a, Frac b)
        {
            return Substract(a, b);
        }

        public static Frac operator *(Frac a, Frac b)
        {
            return Multiply(a, b);
        }

        public static Frac operator /(Frac a, Frac b)
        {
            return Divide(a, b);
        }

        public static Frac operator -(Frac f)
        {
            return new Frac(-f.num, f.den);
        }

        public static Frac operator ++(Frac f)
        {
            f = Add(f, 1);
            return f;
        }

        public static Frac operator --(Frac f)
        {
            f = Substract(f, 1);
            return f;
        }

        public static bool operator ==(Frac a, Frac b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Frac a, Frac b)
        {
            return !a.Equals(b);
        }

        public static bool operator <(Frac a, Frac b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(Frac a, Frac b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator <=(Frac a, Frac b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >=(Frac a, Frac b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static implicit operator Frac(int value)
        {
            return new Frac(new BigInteger(value));
        }

        public static implicit operator Frac(double value)
        {
            return new Frac(value);
        }

        public static implicit operator Frac(BigInteger value)
        {
            return new Frac(value);
        }

        public static explicit operator double(Frac value)
        {
            return (double)value.num / (double)value.den;
        }
    }
}
