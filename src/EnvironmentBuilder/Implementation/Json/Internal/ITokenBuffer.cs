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
}