using System;

namespace EnvironmentBuilder.Implementation.Json
{
    internal interface ITokenBuffer
    {
        char? MoveNextNonEmptyChar();
        char? MoveNext();
        string MoveNext(int count);
        string MoveNext(char c);
        string MoveNext(char c, Func<string,int,bool> validateEndCondition);
        string MoveNext(Func<string, int,char, bool> validateEndCondition);
        void MovePrev();
        void MovePrev(int count);
        string ToString();
        bool IsNext(string value, StringComparison comparison = StringComparison.CurrentCulture);
        int IndexOf(string substr, StringComparison comparison=StringComparison.CurrentCulture);
        string Substring(int length);
    }

    internal class StringBuffer : ITokenBuffer
    {
        private string _s;
        private int _index;

        internal StringBuffer(string s)
        {
            _s = s;
        }


        public char? MoveNextNonEmptyChar()
        {
            while (_s.Length > _index)
            {
                char c = _s[_index++];
                if (!Char.IsWhiteSpace(c))
                {
                    return c;
                }
            }

            return null;
        }

        public char? MoveNext()
        {
            if (_s.Length > _index)
            {
                return _s[_index++];
            }

            return null;
        }

        public string MoveNext(int count)
        {
            if (_s.Length >= _index + count)
            {
                string result = _s.Substring(_index, count);
                _index += count;

                return result;
            }

            return null;
        }

        public string MoveNext(char c)
        {
            int i = _index;
            while (_s.Length > i)
            {
                var v = _s[i++];
                if (v == c)
                {
                    var r = _s.Substring(_index, i - _index);
                    _index = i;
                    return r;
                }
            }

            return null;
        }

        public string MoveNext(char c, Func<string,int,bool> validateEndCondition)
        {
            int i = _index;
            while (_s.Length > i)
            {
                var v = _s[i++];
                if (v == c && validateEndCondition.Invoke(_s,i-1))
                {
                    var r = _s.Substring(_index, i - _index);
                    _index = i;
                    return r;
                }
            }

            return null;
        }

        public string MoveNext(Func<string, int,char, bool> validateEndCondition)
        {
            int i = _index;
            while (_s.Length > i)
            {
                var v = _s[i++];
                if (validateEndCondition.Invoke(_s,i-1,v))
                {
                    var r = _s.Substring(_index, i - _index);
                    _index = i;
                    return r;
                }
            }

            return null;
        }


        public void MovePrev()
        {
            if (_index > 0)
            {
                _index--;
            }
        }

        public void MovePrev(int count)
        {
            while (_index > 0 && count > 0)
            {
                _index--;
                count--;
            }
        }

        public override string ToString()
        {
            if (_s.Length > _index)
            {
                return _s.Substring(_index);
            }

            return String.Empty;
        }

        public bool IsNext(string value, StringComparison comparison = StringComparison.CurrentCulture)
        {
            return Substring(value.Length).Equals(value, comparison);
        }

        public int IndexOf(string substr, StringComparison comparison=StringComparison.CurrentCulture)
        {
            if (_s.Length > _index)
            {
                return _s.IndexOf(substr, _index, comparison) - _index;
            }

            return -1;
        }

        public string Substring(int length)
        {
            if (_s.Length > _index + length)
            {
                return _s.Substring(_index, length);
            }

            return ToString();
        }
    }
}