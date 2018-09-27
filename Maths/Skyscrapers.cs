using System;
using System.Collections.Generic;
using System.Linq;

namespace Maths
{
    class Skyscrapers
    {
        /// <summary>
        /// Taille d'un côté
        /// </summary>
        static int size;

        /// <summary>
        /// Résoud le puzzle avec les indices adjacents correspondants
        /// </summary>
        /// <param name="clues">Indices</param>
        /// <returns>Puzzle</returns>
        public static int[][] SolvePuzzle(int[] clues)
        {
            if (clues.Length == 0 || clues.Length % 4 > 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            size = clues.Length / 4;

            Grid grid = new Grid(clues);

            grid.Iterate();

            return grid.Result;
        }

        public class Grid
        {
            Line[] rows;
            Line[] cols;

            public Grid(int[] clues)
            {
                rows = new Line[size];
                cols = new Line[size];

                for (int i = 0; i < size; i++)
                {
                    rows[i] = new Line(clues[4 * size - 1 - i], clues[size + i]);
                    cols[i] = new Line(clues[i], clues[3 * size - 1 - i]);
                }

                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        Level level = new Level(rows[i], cols[j]);
                        rows[i].levels[j] = level;
                        cols[j].levels[i] = level;
                    }
                }

                for (int i = 0; i < size; i++)
                {
                    rows[i].SetClues();
                    cols[i].SetClues();
                }
            }

            public void Iterate()
            {
                while (!Blocked)
                {
                    for (int i = 0; i < size; i++)
                    {
                        rows[i].Optimize();
                        cols[i].Optimize();
                    }
                }
            }

            bool Blocked
            {
                get
                {
                    return rows.All(r => r.Blocked);
                }
            }

            public int[][] Result
            {
                get
                {
                    return rows.Select(r => r.levels.Select(l => l.val).ToArray()).ToArray();
                }
            }
        }

        public class Line
        {
            public Level[] levels;
            int rightClue;
            int leftClue;
            List<int[]> possibilities;

            public Line(int left, int right)
            {
                levels = new Level[size];
                leftClue = left;
                rightClue = right;
                possibilities = new List<int[]>();

                foreach (IEnumerable<int> perm in Enumerable.Range(1, size).GetPermutations())
                {
                    possibilities.Add(perm.ToArray());
                }
            }

            public void SetClues()
            {
                if (leftClue == 1)
                {
                    levels[0].BlockValue(size);
                    possibilities.RemoveAll(p => p[0] != size);
                }
                else if (leftClue == size)
                {                 
                    for (int i = 0; i < size; i++)
                    {
                        levels[i].BlockValue(i + 1);
                    }
                    possibilities.Clear();
                    possibilities.Add(Enumerable.Range(1, size).ToArray());
                }
                else
                {
                    for (int i = 0; i < leftClue - 1; i++)
                    {
                        for (int j = size - leftClue + i + 2; j <= size; j++)
                        {
                            levels[i].RemovePos(j);
                            possibilities.RemoveAll(p => p[i] == j);
                        }
                    }
                }

                if (rightClue == 1)
                {
                    levels[size - 1].BlockValue(size);
                    possibilities.RemoveAll(p => p[size - 1] != size);
                }
                else if (rightClue == size)
                {
                    for (int i = 0; i < size; i++)
                    {
                        levels[i].BlockValue(size - i);
                    }
                    possibilities.Clear();
                    possibilities.Add(Enumerable.Range(1, size).Reverse().ToArray());
                }
                else
                {
                    for (int i = 0; i < rightClue - 1; i++)
                    {
                        for (int j = size - rightClue + i + 2; j <= size; j++)
                        {
                            levels[size - 1 - i].RemovePos(j);
                            possibilities.RemoveAll(p => p[size - 1 - i] == j);
                        }
                    }
                }
            }

            public void Optimize()
            {
                if (Blocked)
                {
                    return;
                }

                for (int i = 1; i <= size; i++)
                {
                    if (levels.Count(l => l.Possibilities.Contains(i)) == 1)
                    {
                        levels.First(l => l.Possibilities.Contains(i)).BlockValue(i);
                    }
                }

                for (int i = 0; i < size; i++)
                {
                    int[] posLevel = levels[i].Possibilities;
                    possibilities.RemoveAll(p => !posLevel.Contains(p[i]));
                }
                
                for (int i = 0; i < possibilities.Count; i++)
                {
                    int maxLeft = 0;
                    int left = 0;

                    foreach (int c in possibilities[i])
                    {
                        if (c > maxLeft)
                        {
                            maxLeft = c;
                            left++;
                        }
                    }

                    int maxRight = 0;
                    int right = 0;

                    for (int j = size - 1; j >= 0; j--)
                    {
                        if (possibilities[i][j] > maxRight)
                        {
                            maxRight = possibilities[i][j];
                            right++;
                        }
                    }

                    if (leftClue > 0 && left != leftClue || rightClue > 0 && right != rightClue)
                    {
                        possibilities.RemoveAt(i);
                        i--;
                    }
                }

                for (int i = 0; i < size; i++)
                {
                    int[] posLevel = levels[i].Possibilities;
                    foreach (int pos in posLevel)
                    {
                        if (possibilities.All(p => p[i] != pos))
                        {
                            levels[i].RemovePos(pos);
                        }
                    }
                }
            }

            public bool Blocked
            {
                get
                {
                    return levels.All(l => l.Blocked);
                }
            }
        }

        public class Level
        {
            public int val;
            Line row;
            Line col;
            List<int> possibilities;
            bool blocked;

            public Level(Line r, Line c)
            {
                row = r;
                col = c;
                possibilities = Enumerable.Range(1, size).ToList();
                blocked = false;
            }

            public void BlockValue(int v)
            {
                if (blocked)
                {
                    return;
                }

                val = v;
                possibilities.RemoveAll(p => p != v);
                blocked = true;

                for (int i = 0; i < size; i++)
                {
                    row.levels[i].RemovePos(val);
                    col.levels[i].RemovePos(val);
                }
            }

            public void RemovePos(int v)
            {
                if (blocked)
                {
                    return;
                }

                if (possibilities.Contains(v))
                {
                    possibilities.Remove(v);
                    if (possibilities.Count == 1)
                    {
                        BlockValue(possibilities[0]);
                    }
                }
            }

            public bool Blocked
            {
                get
                {
                    return blocked;
                }
            }

            public int[] Possibilities
            {
                get
                {
                    return possibilities.ToArray();
                }
            }
        }
    }
}
