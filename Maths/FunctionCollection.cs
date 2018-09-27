using System;
using System.Collections;
using System.Collections.Generic;

namespace Maths
{
    public class FunctionCollection : IEnumerable<double>, IEnumerable
    {
        double _start, _end, _step;
        Func<double, double> _function;

        public FunctionCollection(Func<double, double> function, double start, double end, double step = 1.0)
        {
            _start = start;
            _end = end;
            _step = step;
            _function = function;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_function, _start, _end, _step);
        }

        IEnumerator<double> IEnumerable<double>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<double>, IDisposable, IEnumerator
        {
            double _start, _end, _step, _index;
            Func<double, double> _function;

            public Enumerator(Func<double, double> function, double start, double end, double step)
            {
                _start = start;
                _end = end;
                _step = step;
                _function = function;
                _index = start - step;
            }

            public double Current
            {
                get
                {
                    return _function(_index);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                _index += _step;
                return _index <= _end;
            }

            public void Reset()
            {
                _index = _start - _step;
            }
        }
    }
}
