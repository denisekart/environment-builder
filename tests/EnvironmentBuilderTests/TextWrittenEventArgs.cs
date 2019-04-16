using System;

namespace EnvironmentBuilderTests
{
    public class TextWrittenEventArgs:EventArgs
    {
        public TextWrittenEventArgs(string text)
        {
            Text = text;
        }
        public string Text { get; set; }
    }
}