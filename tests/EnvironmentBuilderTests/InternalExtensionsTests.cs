using System;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class InternalExtensionsTests
    {
        //[Fact(Skip = "Changed the implementation, breaking change"),Obsolete]
        internal void CreateBuilderWithGlobalDescription()
        {
#pragma warning disable 618
            var builder=EnvironmentManager.Create(x => x.Description("Foo description"));
#pragma warning restore 618
            Assert.Equal("Foo description", builder.Configuration.Description());
        }
        //[Obsolete,Fact(Skip = "Obsolete - not doing that")]
        internal void CreateBuilderWithGlobalDescriptionMulti()
        {
#pragma warning disable 618
            var builder = EnvironmentManager.Create(x => x.Description("Foo description").Description("Bar description"));
#pragma warning restore 618
            Assert.Equal("Foo descriptionBar description", builder.Configuration.Description());
        }
        //[Fact(Skip = "Changed the implementation, breaking change"), Obsolete]
        internal void CreateBundleWithDescription()
        {
#pragma warning disable 618
            var builder = EnvironmentManager.Create(x => x.Description("Foo description"));
#pragma warning restore 618
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