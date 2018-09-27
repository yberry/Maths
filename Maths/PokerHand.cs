using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths
{
    public enum Result
    {
        Win = 1,
        Loss = -1,
        Tie = 0
    }

    public enum Hand
    {
        HighCard,
        Pair,
        TwoPairs,
        Three,
        Straight,
        Flush,
        Full,
        Four,
        StraightFlush
    }

    public struct PokerHand : IComparable, IComparable<PokerHand>
    {
        Dictionary<char, int> values;
        Dictionary<char, int> suits;
        Hand majorHand;
        string label;

        bool Pair
        {
            get
            {
                return values.ContainsValue(2);
            }
        }

        bool TwoPairs
        {
            get
            {
                return values.Values.Count(v => v == 2) == 2;
            }
        }

        bool Three
        {
            get
            {
                return values.ContainsValue(3);
            }
        }

        bool Straight
        {
            get
            {
                int suit = 0;
                foreach (int val in values.Values)
                {
                    if (val == 1)
                    {
                        if (++suit == 5)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        suit = 0;
                    }
                }

                return suit == 4 && values['A'] == 1;
            }
        }

        bool Flush
        {
            get
            {
                return suits.ContainsValue(5);
            }
        }

        bool Full
        {
            get
            {
                return Three && Pair;
            }
        }

        bool Four
        {
            get
            {
                return values.ContainsValue(4);
            }
        }

        bool StraightFlush
        {
            get
            {
                return Straight && Flush;
            }
        }

        public PokerHand(string hand)
        {
            values = new Dictionary<char, int>()
            {
              { 'A', 0 },
              { 'K', 0 },
              { 'Q', 0 },
              { 'J', 0 },
              { 'T', 0 },
              { '9', 0 },
              { '8', 0 },
              { '7', 0 },
              { '6', 0 },
              { '5', 0 },
              { '4', 0 },
              { '3', 0 },
              { '2', 0 }
            };

            suits = new Dictionary<char, int>()
            {
              { 'S', 0 },
              { 'H', 0 },
              { 'D', 0 },
              { 'C', 0 }
            };

            foreach (string card in hand.Split(' '))
            {
                values[card[0]]++;
                suits[card[1]]++;
            }

            majorHand = Hand.HighCard;
            label = hand;

            if (StraightFlush)
            {
                majorHand = Hand.StraightFlush;
            }
            else if (Four)
            {
                majorHand = Hand.Four;
            }
            else if (Full)
            {
                majorHand = Hand.Full;
            }
            else if (Flush)
            {
                majorHand = Hand.Flush;
            }
            else if (Straight)
            {
                majorHand = Hand.Straight;
            }
            else if (Three)
            {
                majorHand = Hand.Three;
            }
            else if (TwoPairs)
            {
                majorHand = Hand.TwoPairs;
            }
            else if (Pair)
            {
                majorHand = Hand.Pair;
            }
            else
            {
                majorHand = Hand.HighCard;
            }
        }

        public override string ToString()
        {
            return label;
        }

        public int CompareTo(object obj)
        {
            if (obj is PokerHand)
            {
                return CompareTo((PokerHand)obj);
            }

            return 0;
        }

        public int CompareTo(PokerHand hand)
        {
            int mh = majorHand.CompareTo(hand.majorHand);
            if (mh != 0)
            {
                return mh;
            }

            switch (majorHand)
            {
                case Hand.Four:
                    foreach (KeyValuePair<char, int> pair in values)
                    {
                        if (pair.Value == 4 && hand.values[pair.Key] == 4)
                        {
                            goto default;
                        }
                        else if (pair.Value == 4)
                        {
                            return 1;
                        }
                        else if (hand.values[pair.Key] == 4)
                        {
                            return -1;
                        }
                    }
                    return 0;

                case Hand.Full:
                    foreach (KeyValuePair<char, int> pair in values)
                    {
                        if (pair.Value == 3 && hand.values[pair.Key] == 3)
                        {
                            goto case Hand.Pair;
                        }
                        else if (pair.Value == 3)
                        {
                            return 1;
                        }
                        else if (hand.values[pair.Key] == 3)
                        {
                            return -1;
                        }
                    }
                    return 0;

                case Hand.Three:
                    foreach (KeyValuePair<char, int> pair in values)
                    {
                        if (pair.Value == 3 && hand.values[pair.Key] == 3)
                        {
                            goto default;
                        }
                        else if (pair.Value == 3)
                        {
                            return 1;
                        }
                        else if (hand.values[pair.Key] == 3)
                        {
                            return -1;
                        }
                    }
                    return 0;

                case Hand.TwoPairs:
                    bool first = true;
                    foreach (KeyValuePair<char, int> pair in values)
                    {
                        if (pair.Value == 2 && hand.values[pair.Key] == 2)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                goto default;
                            }
                        }
                        else if (pair.Value == 2)
                        {
                            return 1;
                        }
                        else if (hand.values[pair.Key] == 2)
                        {
                            return -1;
                        }
                    }
                    return 0;

                case Hand.Pair:
                    foreach (KeyValuePair<char, int> pair in values)
                    {
                        if (pair.Value == 2 && hand.values[pair.Key] == 2)
                        {
                            goto default;
                        }
                        else if (pair.Value == 2)
                        {
                            return 1;
                        }
                        else if (hand.values[pair.Key] == 2)
                        {
                            return -1;
                        }
                    }
                    return 0;

                default:
                    foreach (KeyValuePair<char, int> pair in values)
                    {
                        int comp = pair.Value.CompareTo(hand.values[pair.Key]);
                        if (comp != 0)
                        {
                            return comp;
                        }
                    }
                    return 0;
            }
        }

        public static bool operator <(PokerHand a, PokerHand b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <=(PokerHand a, PokerHand b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >(PokerHand a, PokerHand b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator >=(PokerHand a, PokerHand b)
        {
            return a.CompareTo(b) >= 0;
        }
    }
}
