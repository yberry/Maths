using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Maths
{
    class Program
    {
        enum Mult
        {
            Tie,
            Loss
        }

        static void Main(string[] args)
        {
            Frac[,] tab = new Frac[,]
            {
                { 2, -2.5, 7 },
                { -4, 1, -5 },
                { 3, 7, 3 }
            };

            Matrix mat = new Matrix(tab);

            Frac f = new Frac("2.5542/3");

            FunctionCollection fc = new FunctionCollection(x => Math.Exp(x), -5.0, 5.0);

            int[,] clues = new int[,]
            {
                { 0, 0, 0, 0, 0, 0, 0, 2, 7 },
                { 7, 0, 0, 0, 0, 2, 9, 0, 0 },
                { 0, 5, 0, 0, 0, 0, 0, 0, 0 },
                { 9, 0, 0, 6, 0, 5, 1, 0, 0 },
                { 0, 0, 1, 0, 3, 0, 0, 0, 8 },
                { 8, 0, 0, 0, 0, 0, 0, 0, 4 },
                { 0, 2, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 4, 3, 0, 2, 0, 8, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 5, 0 }
            };



            Sudoku sud = new Sudoku(clues);

            sud.Solve();
            Console.WriteLine(sud);
            
            Console.ReadLine();
        }

        static bool MultipleOf3(int i)
        {
            string bin = Convert.ToString(i, 2);

            char state = 'A';

            foreach (char c in bin)
            {
                switch (state)
                {
                    case 'A':
                        if (c == '1')
                        {
                            state = 'B';
                        }
                        break;

                    case 'B':
                        state = c == '0' ? 'C' : 'A';
                        break;

                    case 'C':
                        if (c == '0')
                        {
                            state = 'B';
                        }
                        break;
                }
            }

            return state == 'A';
        }

        public class Calculator
        {

            static int[][] powers = new int[][]
            {
                  new int[] { 0 },
                  new int[] { 1 },
                  new int[] { 2, 4, 8, 6 },
                  new int[] { 3, 9, 7, 1 },
                  new int[] { 4, 6 },
                  new int[] { 5 },
                  new int[] { 6 },
                  new int[] { 7, 9, 3, 1 },
                  new int[] { 8, 4, 2, 6 },
                  new int[] { 9, 1 }
            };

            public static int LastDigit(int[] array)
            {

                if (array.Length == 0)
                {
                    return 1;
                }

                List<int> pow = new List<int>(array.Skip(1));
                int lastNum = array[0] % 10;
                int[] row = powers[lastNum];
                int length = row.Length;

                if (pow.Contains(0))
                {
                    for (int i = pow.Count - 2; i >= 0; i--)
                    {
                        if (pow[i + 1] == 0)
                        {
                            pow[i] = 1;
                            pow.RemoveAt(i + 1);
                        }
                    }

                    if (pow[0] == 0)
                    {
                        return 1;
                    }
                }

                if (length == 1)
                {
                    return row[0];
                }
                else if (length == 2)
                {
                    int index = 1 - array[1] & 1;
                    return row[index];
                }
                else
                {
                    int rest = array[1] % 4;

                    return 1;
                }
            }
        }

        static List<string> Logic(int step)
        {
            if (step == 0)
            {
                return new List<string>() { "1" };
            }

            List<string> list = Logic(step - 1);

            string line = list[list.Count - 1];

            StringBuilder newLine = new StringBuilder();
            int index = 0;
            int count = 0;
            char start = line[0];

            while (index < line.Length)
            {
                while (index < line.Length && line[index] == start)
                {
                    count++;
                    index++;
                }
                newLine.Append(count).Append(start);
                if (index < line.Length)
                {
                    count = 0;
                    start = line[index];
                }
            }

            list.Add(newLine.ToString());

            return list;
        }

        static BigInteger Problem206(string square)
        {
            BigInteger minSquare = BigInteger.Parse(square.Replace('_', '0'));

            double dec = Math.Exp(0.5 * BigInteger.Log(minSquare));

            BigInteger start = new BigInteger(Math.Floor(dec));

            bool found = false;
            while (!found)
            {
                start += 10;
                BigInteger sq = start * start;
                if (sq.ToString().Length == square.Length)
                {
                    found = true;
                    string st = sq.ToString();
                    for (int i = 0; i < square.Length; i++)
                    {
                        if (square[i] != '_' && square[i] != st[i])
                        {
                            found = false;
                            break;
                        }
                    }
                }
            }

            return start;
        }

        static double Problem205()
        {
            Dictionary<int, double> peter = GetProbs(4, 9);
            Dictionary<int, double> colin = GetProbs(6, 6);

            double win = 0.0;

            foreach (var pair in peter)
            {
                double prob = 0.0;
                foreach (var other in colin)
                {
                    if (pair.Key > other.Key)
                    {
                        prob += other.Value;
                    }
                }
                win += pair.Value * prob;
            }

            return win;
        }

        static Dictionary<int, double> GetProbs(int faces, int dices)
        {
            List<int> list = new List<int>() { 0 };

            for (int i = 0; i < dices; i++)
            {
                List<int> tmp = new List<int>();

                foreach (int num in list)
                {
                    for (int j = 1; j <= faces; j++)
                    {
                        tmp.Add(num + j);
                    }
                }

                list = tmp;
            }

            list.Sort();

            Dictionary<int, double> probs = new Dictionary<int, double>();
            double total = list.Count;

            while (list.Count > 0)
            {
                int num = list[0];
                int count = list.Count(n => n == num);
                probs.Add(num, count / total);
                list.RemoveRange(0, count);
            }

            return probs;
        }

        static int Problem204(int type, BigInteger max)
        {
            List<int> primes = GetPrimes(type);

            BigInteger[] mult = primes.Select(p => new BigInteger(p)).ToArray();
            List<BigInteger> numbers = new List<BigInteger>() { 1 };
            int[] indexes = Enumerable.Repeat(0, primes.Count).ToArray();

            BigInteger last = 0;

            while (last <= max)
            {
                last = mult.Min();
                numbers.Add(last);
                for (int i = 0; i < primes.Count; i++)
                {
                    if (last == mult[i])
                    {
                        mult[i] = primes[i] * numbers[++indexes[i]];
                    }
                }
            }

            return numbers.Count - 1;
        }

        static List<int> GetPrimes(int max)
        {
            List<int> primes = new List<int>() { 2 };

            for (int i = 3; i <= max; i += 2)
            {
                if (primes.All(p => i % p > 0))
                {
                    primes.Add(i);
                }
            }

            return primes;
        }

        static BigInteger Problem160(BigInteger x)
        {
            return 0;
        }

        static double Problem151()
        {
            return 0.0;
        }

        static int Problem145(int max)
        {
            int nums = 0;

            for (int i = 12; i < max; i++)
            {
                if (i % 10 == 0)
                {
                    continue;
                }

                int sum = i + Reverse(i);
                if (sum.ToString().All(c => c % 2 == 1))
                {
                    nums++;
                }
            }

            return nums;
        }

        static int Reverse(int num)
        {
            string st = num.ToString();
            string newSt = new string(st.Reverse().ToArray());
            return Convert.ToInt32(newSt);
        }

        static int Problem127(int max)
        {
            int c = 0;

            for (int i = 3; i < max; i++)
            {
                for (int j = 1; j <= i / 2; j += (i % 2 == 0 ? 2 : 1))
                {
                    if (Extensions.PGCD(i, j) == 1)
                    {
                        int b = i - j;
                        if (Extensions.PGCD(i, b) == 1 && Extensions.PGCD(b, j) == 1)
                        {
                            List<BigInteger> fact = DistinctPrimeFactors(i * j * b);
                            if (fact.Product() < i)
                            {
                                c += i;
                            }
                        }
                    }
                }
            }

            return c;
        }

        static int Problem124(int max, int n)
        {
            List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();

            for (int i = 1; i <= max; i++)
            {
                tuples.Add(new Tuple<int, int>(i, Rad(i)));
            }

            tuples.Sort(CompareRad);

            return tuples[n - 1].Item1;
        }

        static int Rad(int n)
        {
            if (n == 1)
            {
                return 1;
            }

            int prod = 1;
            int i = 2;

            while (n > 1)
            {
                if (n % i == 0)
                {
                    prod *= i;
                    while (n % i == 0)
                    {
                        n /= i;
                    }
                }
                i++;
            }

            return prod;
        }

        static int CompareRad(Tuple<int, int> x, Tuple<int, int> y)
        {
            int comp = x.Item2.CompareTo(y.Item2);

            return comp == 0 ? x.Item1.CompareTo(y.Item1) : comp;
        }

        static BigInteger Problem119(int goal)
        {
            int index = 10;
            BigInteger num = 614656;

            while (index < goal)
            {
                num++;
                if (IsDigitPowerSum(num))
                {
                    index++;
                }
            }

            return num;
        }

        static bool IsDigitPowerSum(BigInteger num)
        {
            int sum = num.ToString().DigitsSum();

            if (sum == 1)
            {
                return false;
            }

            while (num % sum == 0)
            {
                num /= sum;
            }

            return num == BigInteger.One;
        }

        static int Problem108(int solutions)
        {
            int n = 1;

            while (GetSolutions(n) <= solutions)
            {
                n++;
            }

            return n;
        }

        static int GetSolutions(int n)
        {
            int product = 1;
            int div = 2;
            
            while (n > 1)
            {
                int val = 1;
                while (n % div == 0)
                {
                    val += 2;
                    n /= div;
                }
                div++;
                product *= val;
            }

            return (product + 1) / 2;
        }

        static int Problem102(string file)
        {
            string[] lines = File.ReadAllLines(file);

            int inside = 0;

            foreach (string line in lines)
            {
                double[] coord = line.Split(',').Select(s => Convert.ToDouble(s)).ToArray();
                Complex a = new Complex(coord[0], coord[1]);
                Complex b = new Complex(coord[2], coord[3]);
                Complex c = new Complex(coord[4], coord[5]);

                
            }

            return inside;
        }

        static BigInteger Problem100(BigInteger minTotal)
        {
            bool trouve = false;
            BigInteger rep = BigInteger.Zero;

            BigInteger delta = 4 + 8 * (minTotal * (minTotal - 1));
            BigInteger root = Sqrt(delta);
            while ((2 + root) % 4 > 0)
            {
                root++;
            }

            while (!trouve)
            {
                BigInteger delta2 = 32 * (root * root - 2);
                while (!IsSquare(delta2))
                {
                    root += 4;
                }

                BigInteger root2 = Sqrt(delta2);

                if ((8 + root2) % 16 > 0)
                {
                    root += 4;
                    continue;
                }

                trouve = true;
            }

            return (2 + root) / 4;
        }

        static bool IsSquare(BigInteger num)
        {
            double dec = Math.Exp(0.5 * BigInteger.Log(num));
            BigInteger root = new BigInteger(Math.Round(dec));

            return num == root * root;
        }

        static BigInteger Sqrt(BigInteger num)
        {
            double dec = Math.Exp(0.5 * BigInteger.Log(num));
            return new BigInteger(Math.Round(dec));
        }

        static int Problem99(string file)
        {
            string[] lines = File.ReadAllLines(file);

            double max = 0.0;
            int indexMax = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                int[] nums = lines[i].Split(',').Select(s => Convert.ToInt32(s)).ToArray();

                double num = nums[1] * Math.Log(nums[0]);
                if (num > max)
                {
                    max = num;
                    indexMax = i;
                }
            }

            return indexMax + 1;
        }

        static string Problem97(int last)
        {
            BigInteger reste = BigInteger.Pow(10, last);

            BigInteger num = 28433;

            for (int i = 0; i < 7830457; i++)
            {
                num *= 2;
                num %= reste;
            }

            num++;

            string st = num.ToString();

            return new string('0', last - st.Length) + st;
        }

        static int Problem96(string file)
        {
            string[] lines = File.ReadAllLines(file);

            int sum = 0;

            for (int i = 0; i < lines.Length / 10; i++)
            {
                int[][] tab = new int[9][];
                for (int j = 0; j < 9; j++)
                {
                    tab[j] = lines[10 * i + j + 1].GetDigits().ToArray();
                }
                Sudoku sudoku = new Sudoku(tab);
                sudoku.Solve();
                sum += sudoku.TopLeftCorner;
            }

            return sum;
        }

        static int Problem93()
        {
            List<double> nums = new List<double>();

            for (int i = 0; i < 10; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    double pow = Math.Pow(i, j);

                    if (!nums.Contains(pow) && j == pow.ToString().Length)
                    {
                        nums.Add(pow);
                    }
                }
            }

            return nums.Count;
        }

        static int Problem92(int max)
        {
            int nums = 0;

            for (int i = 2; i < max; i++)
            {
                int num = i;
                while (num != 1 && num != 89)
                {
                    num = SquareDigits(num);
                }
                if (num == 89)
                {
                    nums++;
                }
            }

            return nums;
        }

        static int SquareDigits(int num)
        {
            return num.ToString().GetDigits().Sum(n => n * n);
        }

        static int Problem89(string file)
        {
            int diff = 0;

            string[] lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                string roman = Roman.ToRoman(Roman.ToDecimal(line));
                diff += line.Length - roman.Length;
            }

            return diff;
        }

        static int Problem81(string file)
        {
            string[] lines = File.ReadAllLines(file);

            int side = lines.Length;

            int[][] matrix = new int[side][];
            for (int j = 0; j < side; j++)
            {
                matrix[j] = lines[j].Split(',').Select(s => Convert.ToInt32(s)).ToArray();
            }

            List<List<int>> losange = new List<List<int>>();

            int i = 0;
            while (i < side)
            {
                int col = 0;
                List<int> diag = new List<int>();
                while (i >= col)
                {
                    diag.Add(matrix[i - col][col]);
                    col++;
                }
                losange.Add(diag);
                i++;
            }

            i = 1;
            while (i < side)
            {
                int row = 0;
                List<int> diag = new List<int>();
                while (i + row < side)
                {
                    diag.Add(matrix[side - 1 - row][i + row]);
                    row++;
                }
                losange.Add(diag);
                i++;
            }

            for (int k = 1; k < losange.Count; k++)
            {
                List<int> previous = losange[k - 1];
                List<int> next = losange[k];

                if (previous.Count < next.Count)
                {
                    for (int l = 0; l < next.Count; l++)
                    {
                        if (l == 0)
                        {
                            next[l] += previous[l];
                        }
                        else if (l == next.Count - 1)
                        {
                            next[l] += previous[l - 1];
                        }
                        else
                        {
                            next[l] += Math.Min(previous[l - 1], previous[l]);
                        }
                    }
                }
                else
                {
                    for (int l = 0; l < next.Count; l++)
                    {
                        next[l] += Math.Min(previous[l], previous[l + 1]);
                    }
                }
            }

            return losange[losange.Count - 1][0];
        }

        static string Problem79(string file)
        {
            string[] lines = File.ReadAllLines(file);

            List<string> keys = new List<string>();
            foreach (string line in lines)
            {
                if (!keys.Contains(line))
                {
                    keys.Add(line);
                }
            }

            List<char> chars = new List<char>();

            foreach (string key in keys)
            {
                foreach (char c in key)
                {
                    if (!chars.Contains(c))
                    {
                        chars.Add(c);
                    }
                }
            }

            foreach (string key in keys)
            {
                int index = 0;
                for (int i = 0; i < 3; i++)
                {
                    int i0 = chars.IndexOf(key[index]);
                    int i1 = chars.IndexOf(key[index + 1]);

                    if (i0 > i1)
                    {
                        char tmp = chars[i1];
                        chars[i1] = chars[i0];
                        chars[i0] = tmp;
                    }

                    index = 1 - index;
                }
            }

            return string.Join("", chars);
        }

        static int Problem76(int max)
        {
            return CountPos(max, max - 1);
        }

        static int CountPos(int total, int nextNum)
        {
            return nextNum == 1 ? 1 : Enumerable.Range(0, 1 + total / nextNum).Select((n, i) => CountPos(total - i * nextNum, nextNum - 1)).Sum();
        }

        static int Problem78(int total)
        {
            int n = 1;

            while (Partition(n) % total > 0)
            {
                n++;
            }

            return n;
        }

        static List<List<BigInteger>> partitions = new List<List<BigInteger>>() { new List<BigInteger>() };

        static BigInteger Partition(int n)
        {
            while (partitions.Count <= n)
            {
                List<BigInteger> line = new List<BigInteger>();

                for (int i = 0; i < partitions.Count; i++)
                {
                    if (i == 0 || i == partitions.Count - 1)
                    {
                        line.Add(1);
                    }
                    else
                    {
                        BigInteger part1 = partitions[partitions.Count - 1][i - 1];
                        BigInteger part2 = partitions.Count - i - 1 < i + 1 ? 0 : partitions[partitions.Count - i - 1][i];

                        line.Add(part1 + part2);
                    }
                }
                partitions.Add(line);
            }

            BigInteger sum = 0;

            foreach (BigInteger num in partitions[n])
            {
                sum += num;
            }

            return sum;
        }

        static int Problem73(int max)
        {
            List<Frac> fracs = new List<Frac>();

            double valMin = 1.0 / 3.0;
            double valMax = 0.5;

            double imin = valMin * max;
            double imax = valMax * max;

            for (int i = (int)imin; i <= imax; i++)
            {
                int jmin = (int)Math.Floor(i / valMax);
                int jmax = (int)Math.Ceiling(i / valMin);

                for (int j = Math.Max(i + 1, jmin); j <= Math.Min(max, jmax); j++)
                {
                    if (Extensions.PGCD(i, j) == 1)
                    {
                        fracs.Add(new Frac(i, j));
                    }
                }
            }

            fracs.RemoveAll(f => f.ToString() == "1/2");
            fracs.RemoveAll(f => f.ToString() == "1/3");

            return fracs.Count;
        }

        static BigInteger Problem72(int max)
        {
            BigInteger nb = max - 1;

            List<int> primes = new List<int>();

            for (int i = 2; i < max; i++)
            {
                string part = new string('1', Math.Min(i - 1, max - i));
                if (part.Length == i - 1)
                {
                    part += "0";
                }

                IEnumerable<int> fact = primes.Where(pr => i % pr == 0);
                if (fact.Count() == 0)
                {
                    primes.Add(i);
                }
                else
                {
                    StringBuilder build = new StringBuilder(part);
                    foreach (int p in fact)
                    {
                        for (int j = p - 1; j < build.Length; j += p)
                        {
                            build[j] = '0';
                        }
                    }
                    part = build.ToString();
                }

                int div = (max - i) / i;
                int reste = (max - i) % i;

                string sub = part.Substring(0, reste);


                nb += div * part.Count(c => c == '1') + sub.Count(c => c == '1');
            }

            return nb;
        }

        static BigInteger Problem71(int max)
        {
            List<Frac> fracs = new List<Frac>();
            fracs.Add(new Frac(3, 7));

            double val = 3.0 / 7.0;
            double imax = val * max;

            for (int i = 1; i <= imax; i++)
            {
                int div = (int)Math.Ceiling(i / val);
                fracs.Add(new Frac(i, div));    
            }

            fracs.Sort();

            int index = fracs.FindIndex(f => f.ToString() == "3/7");

            return fracs[index - 1].Num;
        }

        static int Problem69(int max)
        {
            List<int> primes = new List<int>() { 2 };

            int product = 2;
            int prime = 1;

            while (product <= max)
            {
                prime += 2;
                if (primes.All(p => prime % p > 0))
                {
                    primes.Add(prime);
                    product *= prime;
                }
            }

            product /= prime;
            return product;
        }

        static int Problem67(string file)
        {
            string[] lines = File.ReadAllLines(file);

            int[][] tab = new int[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                tab[i] = lines[i].Split(' ').Select(s => Convert.ToInt32(s)).ToArray();
            }

            for (int i = tab.Length - 2; i >= 0; i--)
            {
                for (int j = 0; j < tab[i].Length; j++)
                {
                    tab[i][j] += Math.Max(tab[i + 1][j], tab[i + 1][j + 1]);
                }
            }

            return tab[0][0];
        }

        static string Problem59(string file)
        {
            string[] str = File.ReadAllText(file).Split(',');
            int[] nums = str.Select(s => int.Parse(s)).ToArray();

            char[] chars = nums.Select(n => (char)n).ToArray();
            return new string(chars);
        }

        static int Problem57(int max)
        {
            while (Root2List.Count <= max)
            {
                Root2List.Add(Root2(Root2List.Count));
            }

            int total = 0;
                 
            for (int i = 0; i < max; i++)
            {
                if ((1 + Root2List[i].Reverse).MoreNumDigits)
                {
                    total++;
                }
            }

            return total;
        }

        static List<Frac> Root2List = new List<Frac>();

        static Frac Root2(int iteration)
        {
            if (iteration < Root2List.Count)
            {
                return Root2List[iteration];
            }
            else if (iteration == 0)
            {
                return 2;
            }
            else
            {
                return 2 + Root2(iteration - 1).Reverse;
            }
        }

        static int Problem56(int max)
        {
            int sum = int.MinValue;

            for (int i = 1; i < max; i++)
            {
                for (int j = 1; j < max; j++)
                {
                    BigInteger pow = BigInteger.Pow(i, j);
                    int s = pow.ToString().DigitsSum();
                    if (s > sum)
                    {
                        sum = s;
                    }
                }
            }

            return sum;
        }

        static int Problem55(int max)
        {
            int nums = 0;

            for (int i = 1; i < max; i++)
            {
                if (IsLychrel(i))
                {
                    nums++;
                }
            }

            return nums;
        }

        static bool IsLychrel(BigInteger num)
        {
            int i = 0;

            do
            {
                num += Reverse(num);
                i++;
            }
            while (!IsPalindromic(num) && i < 50);

            return i == 50;
        }

        static BigInteger Reverse(BigInteger num)
        {
            string st = num.ToString();
            string newSt = new string(st.Reverse().ToArray());
            return BigInteger.Parse(newSt);
        }

        static int Problem53(int max, int min)
        {
            int nums = 0;

            for (int i = 1; i <= max; i++)
            {
                for (int j = 1; j <= i; j++)
                {
                    BigInteger comb = Combination(i, j);
                    if (comb > min)
                    {
                        nums++;
                    }
                }
            }

            return nums;
        }

        static BigInteger Combination(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        static int Problem52(int mult)
        {
            int i = 1;
            bool found = false;
            IEnumerable<int> range = Enumerable.Range(1, mult);

            while (!found)
            {
                List<string> numbers = range.Select(n => (n * i).ToString()).ToList();

                for (int j = 0; j < mult; j++)
                {
                    List<char> chars = numbers[j].ToList();
                    chars.Sort();
                    numbers[j] = new string(chars.ToArray());
                }

                if (numbers.All(n => n == numbers[0]))
                {
                    found = true;
                }
                else
                {
                    i++;
                }
            }

            return i;
        }

        static string Problem48(int max)
        {
            BigInteger sum = 0;

            for (int i = 1; i <= max; i++)
            {
                sum += BigInteger.Pow(i, i);
            }

            string str = sum.ToString();
            return str.Substring(str.Length - 10, 10);
        }

        static int Problem47(int num)
        {
            List<int> cons = new List<int>();

            int n = 2;

            List<int> primes = new List<int>() { 2 };
            int fact = 3;

            while (primes.Count < num)
            {
                if (primes.All(p => fact % p > 0))
                {
                    primes.Add(fact);
                    n *= fact;
                }
                fact += 2;
            }

            while (cons.Count < num)
            {
                if (DistinctPrimeFactors(n).BigSum() == num)
                {
                    cons.Add(n);
                }
                else
                {
                    cons.Clear();
                }
                n++;
            }

            return cons[0];
        }

        static List<BigInteger> DistinctPrimeFactors(BigInteger num)
        {
            List<BigInteger> factors = new List<BigInteger>();
            BigInteger fact = 2;

            while (num > 1)
            {
                if (num % fact == 0)
                {
                    factors.Add(fact);
                    while ((num % fact).IsZero)
                    {
                        num /= fact;
                    }
                }
                fact++;
            }

            return factors;
        }

        static long Problem43()
        {
            List<string> sol = Get3DigitsMult(17).ToList();
            List<int> primes = new List<int>() { 13, 11, 7, 5, 3, 2, 1 };

            foreach (int prime in primes)
            {
                List<string> tmp = new List<string>();
                foreach (string st in sol)
                {
                    string start = st.Substring(0, 2);

                    for (char c = '0'; c <= '9'; c++)
                    {
                        if (!st.Contains(c))
                        {
                            int num = Convert.ToInt32(c.ToString() + start);
                            if (num % prime == 0)
                            {
                                tmp.Add(c.ToString() + st);
                            }
                        }
                    }
                }
                sol = tmp;
            }

            return sol.Sum(s => Convert.ToInt64(s));
        }

        static IEnumerable<string> Get3DigitsMult(int mult)
        {
            int start = mult;
            while (start < 100)
            {
                start += mult;
            }

            for (int i = start; i < 1000; i += mult)
            {
                string s = i.ToString();
                if (s[0] != s[1] && s[0] != s[2] && s[1] != s[2])
                {
                    yield return s;
                }
            }
        }

        static int Problem42(string file)
        {
            string text = File.ReadAllText(file);
            string[] quotes = text.Split(',');
            List<string> words = quotes.Select(q => q.Substring(1, q.Length - 2)).ToList();

            const string alpha = " ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int triangles = 0;

            foreach (string word in words)
            {
                int index = word.Sum(c => alpha.IndexOf(c));

                int n = 1;
                int mult = 1;

                while (mult < index)
                {
                    n++;
                    mult = (n * (n + 1)) / 2;
                }

                if (index == (n * (n + 1)) / 2)
                {
                    triangles++;
                }
            }

            return triangles;
        }

        static int Problem39(int max)
        {
            return 0;
        }

        static Dictionary<int, int> GetDivisorsCouples(int n)
        {
            Dictionary<int, int> div = new Dictionary<int, int>();

            int sqrt = (int)Math.Sqrt(n);

            for (int i = 1; i <= sqrt; i++)
            {
                if (n % i == 0)
                {
                    div.Add(i, n / i);
                }
            }

            return div;
        }

        static int Problem37()
        {
            List<int> primes = new List<int>() { 2 };
            List<int> truncNum = new List<int>();
            int div = 3;
            int num = 11;

            while (num > 0)
            {
                if (primes.All(p => div % p > 0))
                {
                    primes.Add(div);
                    if (div > 9)
                    {
                        string st = div.ToString();
                        bool trunc = true;
                        int dix = 10;
                        for (int i = 0; i < st.Length - 1; i++)
                        {
                            int right = div % dix;
                            int left = (div - right) / dix;

                            if (!primes.Contains(right) || !primes.Contains(left))
                            {
                                trunc = false;
                                break;
                            }
                            dix *= 10;
                        }

                        if (trunc)
                        {
                            truncNum.Add(div);
                            num--;
                        }
                    }
                }
                div += 2;
            }

            return truncNum.Sum();
        }

        static int Problem36(int max)
        {
            int sum = 0;

            for (int i = 1; i < max; i += 2)
            {
                if (IsPalindromic(i))
                {
                    string bin = Convert.ToString(i, 2);
                    if (IsPalindromic(bin))
                    {
                        sum += i;
                    }
                }
            }

            return sum;
        }

        static int Problem31(int total, List<int> coins)
        {
            coins.Sort((x, y) => y.CompareTo(x));
            if (coins.Count == 1)
            {
                return total % coins[0] == 0 ? 1 : 0;
            }
            else
            {
                int sum = 0;
                for (int i = 0; i <= total / coins[0]; i++)
                {
                    sum += Problem31(total - i * coins[0], coins.GetRange(1, coins.Count - 1));
                }
                return sum;
            }
        }

        static int Problem29(int a, int b)
        {
            List<BigInteger> powers = new List<BigInteger>();

            for (int i = 2; i <= a; i++)
            {
                for (int j = 2; j <= b; j++)
                {
                    BigInteger power = BigInteger.Pow(i, j);
                    if (!powers.Contains(power))
                    {
                        powers.Add(power);
                    }
                }
            }

            return powers.Count;
        }

        static int Problem28(int side)
        {
            int num = 1;

            int tmp = 1;
            int i = 2;

            while (i < side)
            {
                for (int j = 0; j < 4; j++)
                {
                    tmp += i;
                    num += tmp;
                }
                i += 2;
            }

            return num;
        }

        static int Problem25(int digits)
        {
            List<BigInteger> list = new List<BigInteger>() { 1, 1 };

            while (list[list.Count - 1].ToString().Length < digits)
            {
                list.Add(list[list.Count - 2] + list[list.Count - 1]);
            }

            return list.Count;
        } 

        static int Problem23(int max)
        {
            return 0;
        }

        static bool IsPerfect(int num)
        {
            int sum = 0;

            for (int i = 1; i <= num / 2; i++)
            {
                if (num % i == 0)
                {
                    sum += i;
                }
            }

            return sum == num;
        }

        static long Problem22(string file)
        {
            string text = File.ReadAllText(file);
            string[] quotes = text.Split(',');
            List<string> names = quotes.Select(q => q.Substring(1, q.Length - 2)).ToList();
            names.Sort();

            const string alpha = " ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            long result = 0L;

            for (int i = 0; i < names.Count; i++)
            {
                int index = names[i].Sum(c => alpha.IndexOf(c));
                result += (i + 1) * index;
            }

            return result;
        }

        static int Problem21(int max)
        {
            List<int> skip = new List<int>();

            for (int i = 2; i < max; i++)
            {
                if (!skip.Contains(i))
                {
                    int sumDiv = SumDivisors(i);
                    if (i != sumDiv && SumDivisors(sumDiv) == i)
                    {
                        skip.Add(i);
                        skip.Add(sumDiv);
                    }
                }
            }

            return skip.Sum();
        }

        static int SumDivisors(int num)
        {
            int div = 1;
            for (int i = 2; i <= num / 2; i++)
            {
                if (num % i == 0)
                {
                    div += i;
                }
            }
            return div;
        }

        static int Problem20(int max)
        {
            BigInteger n = Factorial(max);

            return n.ToString().DigitsSum();
        }

        static List<BigInteger> factorials = new List<BigInteger>() { 1 };

        static BigInteger Factorial(int n)
        {
            BigInteger last = factorials[factorials.Count - 1];
            for (int i = factorials.Count; i <= n; i++)
            {
                last *= i;
                factorials.Add(last);
            }

            return factorials[n];
        }

        static int Problem19(DateTime start, DateTime end)
        {
            int sundays = 0;

            for (int i = start.Year; i <= end.Year; i++)
            {
                int startMonth = i == start.Year ? start.Month : 1;
                int endMonth = i == end.Year ? end.Month : 12;

                for (int j = startMonth; j <= endMonth; j++)
                {
                    DateTime firstDay = new DateTime(i, j, 1);
                    if (firstDay.DayOfWeek == DayOfWeek.Sunday)
                    {
                        sundays++;
                    }
                }
            }

            return sundays;
        }

        static int Problem18(string file)
        {
            string[] lines = File.ReadAllLines(file);

            int[][] tab = new int[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                tab[i] = lines[i].Split(' ').Select(s => Convert.ToInt32(s)).ToArray();
            }

            return GetMaxPyramid(tab, 0, 0);
        }

        static int GetMaxPyramid(int[][] tab, int row, int col)
        {
            int val = tab[row][col];

            if (row == tab.Length - 1)
            {
                return val;
            }
            else
            {
                return val + Math.Max(GetMaxPyramid(tab, row + 1, col), GetMaxPyramid(tab, row + 1, col + 1));
            }
        }

        static int Problem17(int max)
        {
            Dictionary<int, int> numbers = new Dictionary<int, int>()
            {
                { 1000, 8 },
                { 100, 7 },
                { 90 , 6 },
                { 80 , 7 },
                { 70 , 6 },
                { 60 , 5 },
                { 50 , 5 },
                { 40 , 5 },
                { 30 , 6 },
                { 20 , 6 },
                { 19 , 8 },
                { 18 , 8 },
                { 17 , 9 },
                { 16 , 7 },
                { 15 , 7 },
                { 14 , 8 },
                { 13 , 8 },
                { 12 , 6 },
                { 11 , 6 },
                { 10 , 3 },
                { 9 , 4 },
                { 8 , 5 },
                { 7 , 5 },
                { 6 , 3 },
                { 5 , 4 },
                { 4 , 4 },
                { 3 , 5 },
                { 2 , 3 },
                { 1 , 3 }
            };

            int total = 0;

            for (int i = 1; i <= max; i++)
            {
                int letters = 0;
                bool and = false;

                int num = i;
                foreach (var pair in numbers)
                {
                    if (num >= pair.Key)
                    {
                        if (pair.Key >= 100)
                        {
                            and = true;
                            int mult = 0;
                            while (num >= pair.Key)
                            {
                                mult++;
                                num -= pair.Key;
                            }
                            letters += numbers[mult] + pair.Value;
                        }
                        else
                        {
                            if (and)
                            {
                                and = false;
                                letters += 3;
                            }
                            num -= pair.Key;
                            letters += pair.Value;
                        }
                    }
                }

                total += letters;
            }

            return total;
        }

        static int Problem16(int pow)
        {
            BigInteger n = BigInteger.Pow(2, pow);

            return n.ToString().DigitsSum();
        }

        static long Problem14(int max)
        {
            long maxNumber = 1;
            int maxChain = 1;

            Dictionary<long, int> chains = new Dictionary<long, int>();

            for (int i = 1; i < max; i++)
            {
                int chain = 1;
                long num = i;

                while (num > 1)
                {
                    if (chains.ContainsKey(num))
                    {
                        chain += chains[num] - 1;
                        break;
                    }

                    if ((num & 1) == 0)
                    {
                        num /= 2;
                    }
                    else
                    {
                        num = 3 * num + 1;
                    }
                    chain++;
                }

                chains.Add(i, chain);

                if (maxChain < chain)
                {
                    maxNumber = i;
                    maxChain = chain;
                }
            }

            return maxNumber;
        }

        static string Problem13(string file)
        {
            string[] lines = File.ReadAllLines(file);

            BigInteger number = 0;

            foreach (string line in lines)
            {
                number += BigInteger.Parse(line);
            }

            return number.ToString().Substring(0, 10);
        }

        static int Problem12(int divisors)
        {
            int start = 1;
            int add = 2;

            while (NumDivisors(start) <= divisors)
            {
                start += add;
                add++;
            }

            return start;
        }

        static int NumDivisors(int num)
        {
            int product = 1;
            int div = 2;

            while (num > 1)
            {
                int val = 0;
                while (num % div == 0)
                {
                    val++;
                    num /= div;
                }
                div++;
                product *= val + 1;
            }

            return product;
        }

        static int Problem11(string file)
        {
            string[] lines = File.ReadAllLines(file);

            List<int[]> tab = new List<int[]>();

            foreach (string line in lines)
            {
                tab.Add(line.Split(' ').Select(n => Convert.ToInt32(n)).ToArray());
            }

            int width = tab[0].Length;
            int height = tab.Count;

            int max = int.MinValue;

            foreach (int[] line in tab)
            {
                for (int i = 0; i <= width - 4; i++)
                {
                    int product = 1;
                    for (int j = i; j < i + 4; j++)
                    {
                        product *= line[j];
                    }
                    if (max < product)
                    {
                        max = product;
                    }
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j <= height - 4; j++)
                {
                    int product = 1;
                    for (int k = j; k < j + 4; k++)
                    {
                        product *= tab[k][i];
                    }
                    if (max < product)
                    {
                        max = product;
                    }
                }
            }

            for (int i = 0; i <= width - 4; i++)
            {
                for (int j = 0; j <= height - 4; j++)
                {
                    int product = 1;
                    for (int k = 0; k < 4; k++)
                    {
                        product *= tab[j + k][i + k];
                    }
                    if (max < product)
                    {
                        max = product;
                    }

                    product = 1;
                    for (int k = 0; k < 4; k++)
                    {
                        product *= tab[j + 3 - k][i + k];
                    }
                    if (max < product)
                    {
                        max = product;
                    }
                }
            }

            return max;

        }

        static long Problem10(int max)
        {
            List<long> primes = new List<long>() { 2 };

            long i = 3;
            while (i < max)
            {
                if (primes.All(p => i % p > 0))
                {
                    primes.Add(i);
                }
                i += 2;
            }

            return primes.Sum();
        }

        static int Problem9(int sum)
        {
            int obj = sum / 2;

            for (int i = 1; i < sum; i++)
            {
                if (obj % i == 0)
                {
                    int div = obj / i;
                    if (2 * i > div)
                    {
                        int u = i;
                        int v = div - u;

                        int x = u * u - v * v;
                        int y = 2 * u * v;
                        int z = u * u + v * v;

                        return x * y * z;
                    }
                }
            }

            return 0;
        }

        static long Problem8(string digits, int seq)
        {
            long max = long.MinValue;

            for (int i = 0; i <= digits.Length - seq; i++)
            {
                string sub = digits.Substring(i, seq);

                long product = 1L;
                foreach (int n in sub.GetDigits())
                {
                    product *= n;
                }
                if (max < product)
                {
                    max = product;
                }
            }

            return max;
        }

        static int Problem7(int nth)
        {
            List<int> primes = new List<int>() { 2 };

            int i = 3;
            while (primes.Count < nth)
            {
                if (primes.All(p => i % p > 0))
                {
                    primes.Add(i);
                }
                i += 2;
            }

            return primes[nth - 1];
        }

        static long Problem6(int max)
        {
            List<int> numbers = Enumerable.Range(1, max).ToList();

            long sum = (max * (max + 1)) / 2;
            long squares = numbers.Sum(n => n * n);

            return sum * sum - squares;
        }

        static long Problem5(int max)
        {
            List<int> primes = new List<int>();

            long product = 1;
            for (int i = 2; i < max; i++)
            {
                int mult = i;
                foreach (int p in primes)
                {
                    if (mult % p == 0)
                    {
                        mult = p;
                        break;
                    }
                }
                
                while (product % i != 0)
                {
                    product *= mult;
                }
                primes.Add(i);
            }

            return product;
        }

        static int Problem4(int maxFact)
        {
            int maxNum = maxFact * maxFact;

            int factLength = maxFact.ToString().Length;
            int firstFact = (int)Math.Pow(10, factLength - 1);

            bool goodLength = false;

            while (!goodLength)
            {
                maxNum--;

                if (IsPalindromic(maxNum))
                {
                    for (int i = firstFact; i <= maxFact; i++)
                    {
                        if (maxNum % i == 0)
                        {
                            int div = maxNum / i;
                            if (div.ToString().Length == factLength)
                            {
                                goodLength = true;
                                break;
                            }
                        }
                    }
                }
            }

            return maxNum;
        }

        static bool IsPalindromic(object obj)
        {
            return IsPalindromic(obj.ToString());
        }

        static bool IsPalindromic(string num)
        {
            for (int i = 0; i < num.Length / 2; i++)
            {
                if (num[i] != num[num.Length - 1 - i])
                {
                    return false;
                }
            }

            return true;
        }

        static long Problem3(long number)
        {
            long max = 1;

            long prime = 2;

            while (number > 1)
            {
                while (number % prime == 0)
                {
                    max = prime;
                    number /= prime;
                }
                prime++;
            }

            return max;
        }

        static int Problem2(int max)
        {
            int a = 1;
            int b = 2;

            int sum = 2;

            while (b <= max)
            {
                int tmp = a + b;
                if (tmp <= max && (tmp & 1) == 0)
                {
                    sum += tmp;
                }
                a = b;
                b = tmp;
            }

            return sum;
        }

        static int Problem1(int max)
        {
            int sum = 0;

            for (int i = 2; i < max; i++)
            {
                if (i % 3 == 0 || i % 5 == 0)
                {
                    sum += i;
                }
            }

            return sum;
        }
    }
}
