using System;
using System.Threading;
using EnvironmentBuilder;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace EnvironmentBuilderTests
{
    public class LoggerExtensionsTests
    {
        private readonly ITestOutputHelper _output;
        private readonly TestOutputHelperWriter _writer;

        public LoggerExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
            _writer=new TestOutputHelperWriter(output);
        }

        [Fact]
        public void AddExplicitNoopLoggerMultipleTimesDoesNotThrow()
        {
            EnvironmentManager.Create(x => x.WithNoopLogger().WithNoopLogger());
        }
        [Fact]
        public void AddNullLoggerThrows()
        {
            
            Assert.Throws<ArgumentException>(() => 
                EnvironmentManager.Create(c => c.WithLogger(null, LogLevel.Off)));
        }
        [Fact]
        public void LogTrace()
        {
             bool hit=false;
             _writer.TextWritten += (s, t) => hit = t.Text.Equals("[TRACE] Foo") || hit;
             EnvironmentManager.Create(c => c.WithTextWriterLogger(_writer,LogLevel.Trace).LogTrace("Foo"));
             Assert.True(hit);
        }
        [Fact]
        public void LogDebug()
        {
            bool hit = false;
            _writer.TextWritten += (s, t) => hit = t.Text.Equals("[DEBUG] Foo") || hit;
            EnvironmentManager.Create(c => c.WithTextWriterLogger(_writer, LogLevel.Trace).LogDebug("Foo"));
            Assert.True(hit);
        }
        [Fact]
        public void LogInformation()
        {
            bool hit = false;
            _writer.TextWritten += (s, t) => hit = t.Text.Equals("[INFO] Foo") || hit;
            EnvironmentManager.Create(c => c.WithTextWriterLogger(_writer, LogLevel.Trace).LogInformation("Foo"));
            Assert.True(hit);
        }
        [Fact]
        public void LogWarning()
        {
            bool hit = false;
            _writer.TextWritten += (s, t) => hit = t.Text.Equals("[WARNING] Foo") || hit;
            EnvironmentManager.Create(c => c.WithTextWriterLogger(_writer, LogLevel.Trace).LogWarning("Foo"));
            Assert.True(hit);
        }
        [Fact]
        public void LogError()
        {
            bool hit = false;
            _writer.TextWritten += (s, t) => hit = t.Text.Equals("[ERROR] Foo") || hit;
            EnvironmentManager.Create(c=>c.WithTextWriterLogger(_writer, LogLevel.Trace).LogError("Foo"));
            Assert.True(hit);
        }
        [Fact]
        public void LogFatal()
        {
            bool hit = false;
            _writer.TextWritten += (s, t) => hit = t.Text.Equals("[FATAL] Foo") || hit;
            EnvironmentManager.Create(c=>c.WithTextWriterLogger(_writer, LogLevel.Trace).LogFatal("Foo"));
            Assert.True(hit);
        }

        [Fact]
        public void DoNotLogWarning()
        {
            bool hit = false;
            _writer.TextWritten += (s, t) => hit = t.Text.Equals("[WARNING] Foo") || hit;
            EnvironmentManager.Create(c => c.WithTextWriterLogger(_writer, LogLevel.Error).LogWarning("Foo"));
            Assert.False(hit);
        }
        [Fact]
        public void DoNotLogTrace()
        {
            bool hit = false;
            _writer.TextWritten += (s, t) => hit = t.Text.Equals("[TRACE] Foo") || hit;
            EnvironmentManager.Create(c => c.WithTextWriterLogger(_writer, LogLevel.Off).LogTrace("Foo"));
            Assert.False(hit);
        }
    }
}