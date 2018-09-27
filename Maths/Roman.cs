using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Maths
{
    public static class Roman
    {
        static Regex regex = new Regex(@"^[MDCLXVI]*$");

        static readonly Dictionary<string, int> dico = new Dictionary<string, int>()
        {
            { "M", 1000 },
            { "CM", 900 },
            { "D", 500 },
            { "CD", 400 },
            { "C", 100 },
            { "XC", 90 },
            { "L", 50 },
            { "XL", 40 },
            { "X", 10 },
            { "IX", 9 },
            { "V", 5 },
            { "IV", 4 },
            { "I", 1 }
        };

        public static int ToDecimal(string roman)
        {
            if (!regex.IsMatch(roman))
            {
                throw new FormatException();
            }

            int dec = 0;
            int index = 0;
            foreach (KeyValuePair<string, int> pair in dico)
            {
                int size = pair.Key.Length;
                while (index <= roman.Length - size && roman.Substring(index, size) == pair.Key)
                {
                    dec += pair.Value;
                    index += size;
                }
            }

            return dec;
        }

        public static string ToRoman(int dec)
        {
            if (dec < 0)
            {
                throw new FormatException();
            }

            StringBuilder roman = new StringBuilder();

            foreach (KeyValuePair<string, int> pair in dico)
            {
                while (dec >= pair.Value)
                {
                    roman.Append(pair.Key);
                    dec -= pair.Value;
                }
            }

            return roman.ToString();
        }
    }
}
