using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maths
{
    class Matrix : IEquatable<Matrix>
    {
        Frac[,] tab;

        public Frac this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return tab[row, col];
            }

            set
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                {
                    throw new ArgumentOutOfRangeException();
                }

                tab[row, col] = value;
            }
        }

        public int Rows
        {
            get
            {
                return tab.GetLength(0);
            }
        }

        public int Cols
        {
            get
            {
                return tab.GetLength(1);
            }
        }

        public bool IsSquare
        {
            get
            {
                return Cols == Rows;
            }
        }

        public bool IsUnique
        {
            get
            {
                return Rows == 1 && Cols == 1;
            }
        }

        public bool IsLine
        {
            get
            {
                return Rows == 1 || Cols == 1;
            }
        }

        public Frac Determinant
        {
            get
            {
                if (!IsSquare)
                {
                    throw new FormatException();
                }

                if (IsUnique)
                {
                    return tab[0, 0];
                }

                Frac sum = 0;
                Frac mult = 1;

                for (int i = 0; i < Rows; i++)
                {
                    sum += tab[i, 0] * mult * GetSubMatrix(i, 0).Determinant;
                    mult *= -1;
                }

                return sum;
            }
        }

        public Matrix Transposed
        {
            get
            {
                Matrix tr = new Matrix(Cols, Rows);

                for (int i = 0; i < Cols; i++)
                {
                    tr.SetRow(i, GetCol(i));
                }

                return tr;
            }
        }

        public Matrix Inversed
        {
            get
            {
                Frac det = Determinant;

                if (det.IsZero)
                {
                    throw new DivideByZeroException();
                }

                int n = Rows;

                Matrix comat = new Matrix(n, n);

                int mult;
                for (int i = 0; i < n; i++)
                {
                    mult = 1 - 2 * (i % 2);
                    for (int j = 0; j < n; j++)
                    {
                        comat.tab[j, i] = mult * GetSubMatrix(i, j).Determinant / det;
                        mult *= -1;
                    }
                }

                return comat;
            }
        }

        public Matrix Diagonal
        {
            get
            {
                if (!IsSquare)
                {
                    throw new FormatException();
                }

                Matrix diag = new Matrix(Rows, 1);

                for (int i = 0; i < Rows; i++)
                {
                    diag.tab[i, 0] = tab[i, i];
                }

                return diag;
            }
        }

        public Frac Trace
        {
            get
            {
                return Diagonal.Sum;
            }
        }

        public bool IsZero
        {
            get
            {
                return tab.All(f => f.IsZero);
            }
        }

        public bool IsIdentity
        {
            get
            {
                if (!IsSquare)
                {
                    return false;
                }

                int n = Rows;

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (tab[i, j] != (i == j ? 1 : 0))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        public Frac Sum
        {
            get
            {
                Frac sum = 0;

                foreach (Frac f in tab)
                {
                    sum += f;
                }

                return sum;
            }
        }

        public Matrix(int rows, int cols) : this(rows, cols, 0) { }

        public Matrix(int rows, int cols, Frac val)
        {
            if (rows < 1 || cols < 1)
            {
                throw new OverflowException();
            }

            tab = Extensions.Repeat(val, rows, cols);
        }

        public Matrix(Matrix other) : this(other.tab) { }

        public Matrix(Frac[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            if (rows < 1 || cols < 1)
            {
                throw new OverflowException();
            }

            tab = new Frac[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tab[i, j] = array[i, j];
                }
            }
        }

        public Matrix(Frac[][] doubleArray)
        {
            int rows = doubleArray.Length;
            int cols = 0;

            if (rows > 1)
            {
                cols = doubleArray[0].Length;
                
                if (cols == 0 || doubleArray.Any(line => line.Length != cols))
                {
                    throw new OverflowException();
                }
            }
            else
            {
                throw new OverflowException();
            }

            tab = new Frac[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    tab[i, j] = doubleArray[i][j];
                }
            }

        }

        public Matrix GetSubMatrix(int row, int col)
        {
            if (IsLine || row < 0 || row >= Rows || col < 0 || col >= Cols)
            {
                throw new ArgumentOutOfRangeException();
            }

            Matrix sub = new Matrix(Rows - 1, Cols - 1);
            for (int i = 0; i < Rows - 1; i++)
            {
                for (int j = 0; j < Cols - 1; j++)
                {
                    sub.tab[i, j] = tab[i + (i < row ? 0 : 1), j + (j < col ? 0 : 1)];
                }
            }

            return sub;
        }

        public Frac[] GetRow(int row)
        {
            if (row < 0 || row >= Rows)
            {
                throw new ArgumentOutOfRangeException();
            }

            Frac[] line = new Frac[Cols];

            for (int i = 0; i < Cols; i++)
            {
                line[i] = tab[row, i];
            }

            return line;
        }

        public Frac[] GetCol(int col)
        {
            if (col < 0 || col >= Cols)
            {
                throw new ArgumentOutOfRangeException();
            }

            Frac[] line = new Frac[Rows];

            for (int i = 0; i < Rows; i++)
            {
                line[i] = tab[i, col];
            }

            return line;
        }

        public void SetRow(int row, Frac[] line)
        {
            if (row < 0 || row >= Rows || line.Length != Cols)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Cols; i++)
            {
                tab[row, i] = line[i];
            }
        }

        public void SetCol(int col, Frac[] line)
        {
            if (col < 0 || col >= Cols || line.Length != Rows)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < Rows; i++)
            {
                tab[i, col] = line[i];
            }
        }

        private void Resize(int rows, int cols)
        {
            if (rows < 1 || cols < 1)
            {
                throw new OverflowException();
            }

            Frac[,] resize = Extensions.Repeat(Frac.zero, rows, cols);

            for (int i = 0; i < Math.Min(Rows, rows); i++)
            {
                for (int j = 0; j < Math.Min(Cols, cols); j++)
                {
                    resize[i, j] = tab[i, j];
                }
            }

            tab = resize;
        }

        public void AddRow(int rows = 1)
        {
            Resize(Rows + rows, Cols);
        }

        public void AddRow(Frac[] row)
        {
            if (row.Length != Cols)
            {
                throw new ArgumentOutOfRangeException();
            }

            AddRow();

            SetRow(Rows - 1, row);
        }

        public void AddCol(int cols = 1)
        {
            Resize(Rows, Cols + cols);
        }

        public void AddCol(Frac[] col)
        {
            if (col.Length != Rows)
            {
                throw new ArgumentOutOfRangeException();
            }

            AddCol();

            SetCol(Cols - 1, col);
        }

        public void RemoveRow(int row)
        {
            if (Rows == 1 || row < 0 || row >= Rows)
            {
                throw new ArgumentOutOfRangeException();
            }

            Frac[,] resize = new Frac[Rows - 1, Cols];

            for (int i = 0; i < Rows - 1; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    resize[i, j] = tab[i + (i < row ? 0 : 1), j];
                }
            }

            tab = resize;
        }

        public void RemoveCol(int col)
        {
            if (Cols == 1 || col < 0 || col >= Cols)
            {
                throw new ArgumentOutOfRangeException();
            }

            Frac[,] resize = new Frac[Rows, Cols - 1];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols - 1; j++)
                {
                    resize[i, j] = tab[i, j + (j < col ? 0 : 1)];
                }
            }

            tab = resize;
        }

        public void ConcatRows(Matrix other)
        {
            if (Cols != other.Cols)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < other.Rows; i++)
            {
                AddRow(other.GetRow(i));
            }
        }

        public void ConcatCols(Matrix other)
        {
            if (Rows != other.Rows)
            {
                throw new ArgumentOutOfRangeException();
            }

            for (int i = 0; i < other.Cols; i++)
            {
                AddCol(other.GetCol(i));
            }
        }

        public bool Contains(Frac f)
        {
            return tab.Contains(f);
        }

        public Matrix Passage(Matrix passage)
        {
            return passage.Transposed * this * passage;
        }

        public bool Equals(Matrix other)
        {
            if (Rows != other.Rows || Cols != other.Cols)
            {
                return false;
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (tab[i, j] != other.tab[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is Matrix)
            {
                return Equals(obj as Matrix);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return tab.GetHashCode();
        }

        public override string ToString()
        {
            string[][] str = new string[Rows][];
            int[] maxs = Enumerable.Repeat(int.MinValue, Cols).ToArray();

            for (int i = 0; i < Rows; i++)
            {
                str[i] = new string[Cols];
                for (int j = 0; j < Cols; j++)
                {
                    string s = tab[i, j].ToString();
                    if (maxs[j] < s.Length)
                    {
                        maxs[j] = s.Length;
                    }
                    str[i][j] = s;
                }
            }

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    str[i][j] = str[i][j].PadLeft(maxs[j], ' ');
                }
            }

            IEnumerable<string> lines = str.Select(line => "[ " + string.Join(", ", line) + " ]");

            return string.Join("\n", lines);
        }

        public static Matrix Identity(int n)
        {
            Matrix mat = new Matrix(n, n);

            for (int i = 0; i < n; i++)
            {
                mat.tab[i, i] = 1;
            }

            return mat;
        }

        public static Matrix Pow(Matrix matrix, int exp)
        {
            if (exp < 0 || !matrix.IsSquare)
            {
                throw new ArgumentOutOfRangeException();
            }

            Matrix pow = Identity(matrix.Rows);

            for (int i = 0; i < exp; i++)
            {
                pow *= matrix;
            }

            return pow;
        }

        public static Matrix Add(Matrix a, Matrix b)
        {
            int rows = a.Rows;
            int cols = a.Cols;

            if (rows != b.Rows || cols != b.Rows)
            {
                throw new ArgumentOutOfRangeException();
            }

            Matrix add = new Matrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    add.tab[i, j] = a.tab[i, j] + b.tab[i, j];
                }
            }

            return add;
        }

        public static Matrix Substract(Matrix a, Matrix b)
        {
            int rows = a.Rows;
            int cols = a.Cols;

            if (rows != b.Rows || cols != b.Rows)
            {
                throw new ArgumentOutOfRangeException();
            }

            Matrix sub = new Matrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    sub.tab[i, j] = a.tab[i, j] - b.tab[i, j];
                }
            }

            return sub;
        }

        public static Matrix Multiply(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
            {
                throw new ArgumentOutOfRangeException();
            }

            Matrix mult = new Matrix(a.Rows, b.Cols);

            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Cols; j++)
                {
                    Frac sum = 0;
                    for (int k = 0; k < a.Cols; k++)
                    {
                        sum += a.tab[i, k] * b.tab[k, j];
                    }
                    mult.tab[i, j] = sum;
                }
            }

            return mult;
        }

        public static Matrix Multiply(Matrix a, Frac b)
        {
            Matrix mult = new Matrix(a);

            for (int i = 0; i < mult.Rows; i++)
            {
                for (int j = 0; j < mult.Cols; j++)
                {
                    mult.tab[i, j] *= b;
                }
            }

            return mult;
        }        

        public static Matrix operator -(Matrix m)
        {
            return Multiply(m, -1);
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            return !a.Equals(b);
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            return Add(a, b);
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            return Substract(a, b);
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            return Multiply(a, b);
        }

        public static Matrix operator *(Matrix m, Frac f)
        {
            return Multiply(m, f);
        }

        public static Matrix operator *(Frac f, Matrix m)
        {
            return Multiply(m, f);
        }

        public static Matrix operator /(Matrix m, Frac f)
        {
            return Multiply(m, f.Reverse);
        }
    }
}
