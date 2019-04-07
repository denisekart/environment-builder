using System;
using System.Collections.Generic;
using System.Linq;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class EnvironmentVariableExtensionsTests
    {
        [Fact]
        public void ReadFromSingleEnvVariable()
        {
            Environment.SetEnvironmentVariable("Foo","foo");
            var value = EnvironmentManager.Create().Env("Foo").Build();
            Assert.Equal("foo",value);
        }

        [Fact]
        public void ReadFromMultiEnvVariable()
        {
            Environment.SetEnvironmentVariable("Foo", "foo");
            var value = EnvironmentManager.Create().Env("Bar").Env("Foo").Build();
            Assert.Equal("foo", value);
        }
        [Fact]
        public void ReadFromVarWithGlobalPrefix()
        {
            Environment.SetEnvironmentVariable("x_Foo", "foo");
            var value = EnvironmentManager.Create(x=>x.WithEnvironmentVariablePrefix("x_")).Env("Foo").Build();
            Assert.Equal("foo", value);
        }
        [Fact]
        public void ReadFromVarWithLocalPrefix()
        {
            Environment.SetEnvironmentVariable("Foo", "foo");
            var value = EnvironmentManager.Create().With("Foo").Env().Build();
            Assert.Equal("foo", value);
        }
        [Fact]
        public void ReadFromVarWithArrayTypePrefix()
        {
            Environment.SetEnvironmentVariable("Foo", "foo;bar");
            var value = EnvironmentManager.Create().With("Foo").Env().Build<IEnumerable<string>>();
            Assert.True(new[]{"foo","bar"}.SequenceEqual(value));
        }

    }
}