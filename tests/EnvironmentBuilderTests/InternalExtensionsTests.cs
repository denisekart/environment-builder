using System;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class InternalExtensionsTests
    {
        [Fact]
        public void CreateBuilderWithGlobalDescription()
        {
            var builder=EnvironmentManager.Create(x => x.Description("Foo description"));
            Assert.Equal("Foo description", builder.Configuration.Description());
        }
        [Fact(Skip = "Obsolete - not doing that")]
        public void CreateBuilderWithGlobalDescriptionMulti()
        {
            var builder = EnvironmentManager.Create(x => x.Description("Foo description").Description("Bar description"));
            Assert.Equal("Foo descriptionBar description", builder.Configuration.Description());
        }
        [Fact]
        public void CreateBundleWithDescription()
        {
            var builder = EnvironmentManager.Create(x => x.Description("Foo description"));
            var bundle = builder.Arg("foo").Arg("bar").Default("foo").Bundle();
            bundle = builder.Arg("foo").Arg("bar").Default("foo").Bundle();
            var help = builder.Help();
        }

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