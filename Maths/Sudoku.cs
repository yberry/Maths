using System.Collections.Generic;
using System.Linq;
using System;

namespace Maths
{
    class Sudoku
    {
        Line[] squares;
        Line[] rows;
        Line[] cols;

        public Sudoku() : this(9) { }

        public Sudoku(int size)
        {
            if (size < 0)
            {
                throw new FormatException();
            }

            int root = (int)Math.Sqrt(size);
            if (root * root != size)
            {
                throw new FormatException();
            }

            squares = new Line[size];
            rows = new Line[size];
            cols = new Line[size];

            for (int i = 0; i < size; i++)
            {
                rows[i] = new Line(size);
                cols[i] = new Line(size);
                squares[i] = new Line(size);
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int index = root * (i / root) + j / root;
                    Level level = new Level(squares[index], rows[i], cols[j], size);
                    squares[index].levels[root * (i % root) + j % root] = level;
                    rows[i].levels[j] = level;
                    cols[j].levels[i] = level;
                }
            }
        }

        public Sudoku(int[][] clues) : this(clues.Length)
        {
            int size = clues.Length;
            if (clues.Any(r => r.Length != size))
            {
                throw new FormatException();
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (clues[i][j] > 0 && clues[i][j] <= size)
                    {
                        rows[i].levels[j].BlockValue(clues[i][j]);
                    }
                }
            }
        }

        public Sudoku(int[,] clues) : this(clues.GetLength(0))
        {
            int size = clues.GetLength(0);
            if (size != clues.GetLength(1))
            {
                throw new FormatException();
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (clues[i, j] > 0 && clues[i, j] <= size)
                    {
                        rows[i].levels[j].BlockValue(clues[i, j]);
                    }
                }
            }
        }

        public bool Solve()
        {
            while (!Done)
            {
                bool changed = false;
                for (int i = 0; i < squares.Length; i++)
                {
                    changed |= squares[i].Solve();
                    changed |= rows[i].Solve();
                    changed |= cols[i].Solve();
                }

                if (!changed)
                {
                    return false;
                }
            }

            return true;
        }

        public int TopLeftCorner
        {
            get
            {
                Level[] levels = rows[0].levels;
                return 100 * levels[0].Val + 10 * levels[1].Val + levels[2].Val;
            }
        }

        public bool Done
        {
            get
            {
                return rows.All(r => r.Blocked);
            }
        }

        public override string ToString()
        {
            return string.Join<Line>("\n", rows);
        }

        public class Line
        {
            public Level[] levels;

            public Line(int size)
            {
                levels = new Level[size];
            }

            public bool Solve()
            {
                if (Blocked)
                {
                    return false;
                }

                bool changed = false;
                for (int i = 1; i <= levels.Length; i++)
                {
                    Level[] blockables = levels.Where(l => !l.Blocked && l.HasPos(i)).ToArray();
                    if (blockables.Length == 1)
                    {
                        blockables[0].BlockValue(i);
                        changed = true;
                    }
                    else if (blockables.Length == 2)
                    {
                        if (blockables[0].Row == blockables[1].Row)
                        {
                            blockables[0].Row.RemovePos(i, blockables);
                        }
                        else if (blockables[0].Col == blockables[1].Col)
                        {
                            blockables[0].Col.RemovePos(i, blockables);
                        }
                    }
                }

                return changed;
            }

            public void RemovePos(int v, params Level[] except)
            {
                foreach (Level level in levels)
                {
                    if (!except.Contains(level))
                    {
                        level.RemovePos(v);
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

            public override string ToString()
            {
                return string.Join<Level>(" ", levels);
            }
        }

        public class Level
        {
            int val;
            Line row;
            Line col;
            Line square;
            List<int> possibilities;
            bool blocked;

            public bool Blocked => blocked;

            public int Val => val;

            public Line Row => row;

            public Line Col => col;

            public Level(Line s, Line r, Line c, int size)
            {
                square = s;
                row = r;
                col = c;
                possibilities = Enumerable.Range(1, size).ToList();
                val = 0;
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

                row.RemovePos(v);
                col.RemovePos(v);
                square.RemovePos(v);
            }

            public bool HasPos(int p)
            {
                return possibilities.Contains(p);
            }

            public void RemovePos(int v)
            {
                if (blocked)
                {
                    return;
                }

                if (possibilities.Remove(v) && possibilities.Count == 1)
                {
                    BlockValue(possibilities[0]);
                }
            }

            public override string ToString()
            {
                return val.ToString();
            }
        }
    }
}
