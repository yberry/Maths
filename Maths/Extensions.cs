using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Maths
{
    static class Extensions
    {
        static Random random = new Random();

        public static T RandomItem<T>(this IEnumerable<T> source)
        {
            int count = source.Count();
            if (count == 0)
            {
                throw new ArgumentNullException();
            }

            int num = random.Next(count);
            return source.ElementAt(num);
        }

        public static Frac FracSum(this IEnumerable<Frac> source)
        {
            Frac sum = Frac.zero;

            foreach (Frac frac in source)
            {
                sum += frac;
            }

            return sum;
        }

        public static Frac FracSum<TSource>(this IEnumerable<TSource> source, Func<TSource, Frac> selector)
        {
            Frac sum = Frac.zero;

            foreach (TSource elem in source)
            {
                sum += selector(elem);
            }

            return sum;
        }

        public static BigInteger Product(this IEnumerable<int> source)
        {
            if (source.Contains(0))
            {
                return 0;
            }

            int product = 1;
            foreach (int n in source)
            {
                product *= n;
            }
            return product;
        }

        public static BigInteger Product(this IEnumerable<BigInteger> source)
        {
            if (source.Contains(0))
            {
                return 0;
            }

            BigInteger product = 1;
            foreach (BigInteger n in source)
            {
                product *= n;
            }
            return product;
        }

        public static BigInteger Product<TSource>(this IEnumerable<TSource> source, Func<TSource, BigInteger> selector)
        {
            BigInteger product = 1;
            foreach (TSource elem in source)
            {
                BigInteger n = selector(elem);
                if (n.IsZero)
                {
                    return n;
                }
                product *= n;
            }
            return product;
        }

        public static BigInteger BigSum(this IEnumerable<BigInteger> source)
        {
            BigInteger sum = 0;

            foreach (BigInteger bi in source)
            {
                sum += bi;
            }

            return sum;
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> source)
        {
            int c = source.Count();
            if (c == 1)
            {
                yield return source;
            }
            else
            {
                for (int i = 0; i < c; i++)
                {
                    foreach (IEnumerable<T> p in source.Take(i).Concat(source.Skip(i + 1)).GetPermutations())
                    {
                        yield return source.Skip(i).Take(1).Concat(p);
                    }
                }
            }
        }

        public static IEnumerable<int> GetDigits(this IEnumerable<char> number)
        {
            if (number.Any(c => c < '0' || c > '9'))
            {
                throw new FormatException();
            }

            return number.Select(c => c - '0');
        }

        public static int DigitsSum(this IEnumerable<char> number)
        {
            if (number.Any(c => c < '0' || c > '9'))
            {
                throw new FormatException();
            }

            return number.Sum(c => c - '0');
        }

        public static void ReverseSort<T>(this List<T> list) where T : IComparable<T>
        {
            list.Sort((x, y) => y.CompareTo(x));
        }

        public static int Clamp(int val, int min, int max)
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

        public static double Clamp(double val, double min, double max)
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

        public static double Clamp01(double val)
        {
            return Clamp(val, 0.0, 1.0);
        }

        public static int PGCD(int a, int b)
        {
            int tmp = a % b;

            return tmp == 0 ? b : PGCD(b, tmp);
        }

        public static int PPCM(int a, int b)
        {
            return a * b / PGCD(a, b);
        }

        public static double[] SolveSecondDecree(double a, double b, double c)
        {
            if (a == 0.0)
            {
                throw new FormatException();
            }

            List<double> solutions = new List<double>();

            double delta = b * b - 4.0 * a * c;

            if (delta >= 0.0)
            {
                double center = -b / (2.0 * a);

                if (delta == 0.0)
                {
                    solutions.Add(center);
                }
                else
                {
                    double space = Math.Sqrt(delta) / (2.0 * a);
                    solutions.Add(center - space);
                    solutions.Add(center + space);
                }
            }

            return solutions.ToArray();
        }

        public static double Variance(this IEnumerable<double> source)
        {
            double average = source.Average();

            double total = source.Sum(d => (d - average) * (d - average));

            return total / source.Count();
        }

        public static double StandardDeviation(this IEnumerable<double> source)
        {
            return Math.Sqrt(source.Variance());
        }

        public static string ReverseString(this string chain)
        {
            return new string(chain.Reverse().ToArray());
        }

        public static Func<double, double> SinMinMax(double min, double max)
        {
            return x => ((max - min) * Math.Sin(x) + max + min) * 0.5;
        }

        public static Func<double, double> CosMinMax(double min, double max)
        {
            return x => ((max - min) * Math.Cos(x) + max + min) * 0.5;
        }

        #region Double Tableau

        public static bool All<TSource>(this TSource[,] source, Predicate<TSource> predicate)
        {
            if (source == null || predicate == null)
            {
                throw new ArgumentNullException();
            }

            foreach (TSource t in source)
            {
                if (!predicate(t))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Any<TSource>(this TSource[,] source, Predicate<TSource> predicate)
        {
            if (source == null || predicate == null)
            {
                throw new ArgumentNullException();
            }

            foreach (TSource t in source)
            {
                if (predicate(t))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains<TSource>(this TSource[,] source, TSource value)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            foreach (TSource t in source)
            {
                if (t.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static int Count<TSource>(this TSource[,] source, Predicate<TSource> predicate)
        {
            if (source == null || predicate == null)
            {
                throw new ArgumentNullException();
            }

            int nb = 0;

            foreach (TSource t in source)
            {
                if (predicate(t))
                {
                    nb++;
                }
            }

            return nb;
        }

        public static TSource First<TSource>(this TSource[,] source, Predicate<TSource> predicate)
        {
            if (source == null || predicate == null)
            {
                throw new ArgumentNullException();
            }

            foreach (TSource t in source)
            {
                if (predicate(t))
                {
                    return t;
                }
            }

            throw new ArgumentException();
        }

        public static TSource Last<TSource>(this TSource[,] source, Predicate<TSource> predicate)
        {
            if (source == null || predicate == null)
            {
                throw new ArgumentNullException();
            }

            bool found = false;
            TSource elem = default(TSource);

            foreach (TSource t in source)
            {
                if (predicate(t))
                {
                    elem = t;
                    found = true;
                }
            }

            if (found)
            {
                return elem;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static TSource Max<TSource>(this TSource[,] source) where TSource : IComparable<TSource>
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            TSource max = source[0, 0];

            foreach (TSource t in source)
            {
                if (t.CompareTo(max) > 0)
                {
                    max = t;
                }
            }

            return max;
        }

        public static TResult Max<TSource, TResult>(this TSource[,] source, Func<TSource, TResult> selector) where TResult : IComparable<TResult>
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            TResult max = selector(source[0, 0]);

            foreach (TSource t in source)
            {
                TResult elem = selector(t);
                if (elem.CompareTo(max) > 0)
                {
                    max = elem;
                }
            }

            return max;
        }

        public static TSource Min<TSource>(this TSource[,] source) where TSource : IComparable<TSource>
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            TSource min = source[0, 0];

            foreach (TSource t in source)
            {
                if (t.CompareTo(min) < 0)
                {
                    min = t;
                }
            }

            return min;
        }

        public static TResult Min<TSource, TResult>(this TSource[,] source, Func<TSource, TResult> selector) where TResult : IComparable<TResult>
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            TResult min = selector(source[0, 0]);

            foreach (TSource t in source)
            {
                TResult elem = selector(t);
                if (elem.CompareTo(min) < 0)
                {
                    min = elem;
                }
            }

            return min;
        }

        public static TResult[,] Repeat<TResult>(TResult element, int rows, int cols)
        {
            if (rows < 0 || cols < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            TResult[,] tab = new TResult[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tab[i, j] = element;
                }
            }

            return tab;
        }

        public static TResult[,] Select<TSource, TResult>(this TSource[,] source, Func<TSource, TResult> selector)
        {
            if (source == null || selector == null)
            {
                throw new ArgumentNullException();
            }

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            TResult[,] tab = new TResult[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tab[i, j] = selector(source[i, j]);
                }
            }

            return tab;
        }

        public static TResult[,] Select<TSource, TResult>(this TSource[,] source, Func<TSource, int, int, TResult> selector)
        {
            if (source == null || selector == null)
            {
                throw new ArgumentNullException();
            }

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            TResult[,] tab = new TResult[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tab[i, j] = selector(source[i, j], i, j);
                }
            }

            return tab;
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this TSource[,] source, Func<TSource, IEnumerable<TResult>> selector)
        {
            if (source == null || selector == null)
            {
                throw new ArgumentNullException();
            }

            foreach (TSource elem in source)
            {
                foreach (TResult result in selector(elem))
                {
                    yield return result;
                }
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this TSource[,] source, Func<TSource, int, int, IEnumerable<TResult>> selector)
        {
            if (source == null || selector == null)
            {
                throw new ArgumentNullException();
            }

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    foreach (TResult result in selector(source[i, j], i, j))
                    {
                        yield return result;
                    }
                }
            }
        }

        public static IEnumerable<TSource> Skip<TSource>(this TSource[,] source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            int index = 0;
            foreach (TSource elem in source)
            {
                if (++index > count)
                {
                    yield return elem;
                }
            }
        }

        public static IEnumerable<TSource> SkipWhile<TSource>(this TSource[,] source, Predicate<TSource> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            bool skip = true;
            foreach (TSource elem in source)
            {
                if (!skip || !predicate(elem))
                {
                    skip = false;
                    yield return elem;
                }
            }
        }

        public static IEnumerable<TSource> Take<TSource>(this TSource[,] source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            int index = 0;
            foreach (TSource elem in source)
            {
                if (index++ < count)
                {
                    yield return elem;
                }
                else
                {
                    break;
                }
            }
        }

        public static IEnumerable<TSource> TakeWhile<TSource>(this TSource[,] source, Predicate<TSource> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            foreach (TSource elem in source)
            {
                if (predicate(elem))
                {
                    yield return elem;
                }
                else
                {
                    break;
                }
            }
        }

        public static TSource[][] ToMultiArray<TSource>(this TSource[,] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            TSource[][] tab = new TSource[rows][];

            for (int i = 0; i < rows; i++)
            {
                tab[i] = new TSource[cols];
                for (int j = 0; j < cols; j++)
                {
                    tab[i][j] = source[i, j];
                }
            }

            return tab;
        }

        public static TSource[,] ToMultiArray<TSource>(this TSource[][] source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            int rows = source.Length;
            if (rows == 0)
            {
                return new TSource[0, 0];
            }

            int cols = source[0].Length;
            if (source.Any(r => r.Length != cols))
            {
                throw new FormatException();
            }

            TSource[,] tab = new TSource[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tab[i, j] = source[i][j];
                }
            }

            return tab;
        }

        public static IEnumerable<TSource> Where<TSource>(this TSource[,] source, Predicate<TSource> predicate)
        {
            if (source == null || predicate == null)
            {
                throw new ArgumentNullException();
            }

            foreach (TSource elem in source)
            {
                if (predicate(elem))
                {
                    yield return elem;
                }
            }
        }

        #endregion
    }
}
