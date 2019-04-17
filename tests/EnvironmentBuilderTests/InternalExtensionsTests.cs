using System;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class InternalExtensionsTests
    {
        

        [Fact]
        public void DefaultToValue()
        {
            var builder = EnvironmentManager.Create();
            var value = builder.Arg("foo").Default("x").Bundle();
            Assert.Equal("x",value.Build());
        }
        [Fact]
        public void DefaultToException()
        {
            var builder = EnvironmentManager.Create();
            var value = builder.Arg("foo").Throw("x").Bundle();
            Assert.Throws<ArgumentException>(() => value.Build());
            
        }

    }
}