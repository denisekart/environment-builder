using System;
using System.IO;
using System.Text;
using Xunit.Abstractions;

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
    internal class TestOutputHelperWriter : TextWriter
    {

        public event EventHandler<TextWrittenEventArgs> TextWritten;

        private readonly ITestOutputHelper _output;

        public TestOutputHelperWriter(ITestOutputHelper output)
        {
            _output = output;
        }
        public override Encoding Encoding =>Encoding.Default;
        public override void WriteLine(string value)
        {
            TextWritten?.Invoke(this,new TextWrittenEventArgs(value));
            _output.WriteLine(value);
        }
        public override void WriteLine(string value, params object[] args)
        {
            TextWritten?.Invoke(this, new TextWrittenEventArgs(string.Format(value,args)));
            _output.WriteLine(value,args);
        }
    }
}