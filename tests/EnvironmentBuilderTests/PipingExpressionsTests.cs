using System;
using System.Collections.Generic;
using System.Text;
using EnvironmentBuilder;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class PipingExpressionsTests
    {
        [Fact]
        public void PipeConfiguration1()
        {

        }
    }
    public class LoggerExtensionsClass
    {
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
            EnvironmentManager.Create(c => c.WithConsoleLogger(LogLevel.Trace)).LogTrace("Foo");
        }
    }
}
