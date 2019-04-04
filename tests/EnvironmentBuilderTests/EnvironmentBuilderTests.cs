using System;
using Xunit;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;

namespace EnvironmentBuilderTests
{
    public class EnvironmentBuilderTests
    {
        [Fact]
        public void CreateSimpleBuilderTest()
        {
            var builder = EnvironmentManager.Create();
        }
        [Fact]
        public void CreateSimpleBuilderWithConfigurationTest()
        {
            var builder = EnvironmentManager.Create(c => 
                c.SetValue("Foo", "Bar")
                    .SetFactoryValue("Foo", () => "Bar"));
        }
        [Fact]
        public void CreateEmptyBundleTest()
        {
            var builder = EnvironmentManager.Create().Bundle();
        }
        [Fact]
        public void CreateBundleWithGenericValueAndBuildTest()
        {
            var builder = EnvironmentManager.Create()
                .WithSource(cfg => "foo")
                .Build();
            Assert.Equal("foo",builder);
        }
        [Fact]
        public void CreateBundleWithGenericValueAndBuildAlt1Test()
        {
            var builder = EnvironmentManager.Create()
                .WithSource(cfg => cfg.GetFactoryValue<string>("foo"),
                    cfg=>cfg.SetFactoryValue("foo",()=>"foo"))
                .Build();
            Assert.Equal("foo", builder);
        }

        [Fact]
        public void CreateBundleWithGenericValueAndBuildAlt1WithConfigurationOverridesTest()
        {
            var builder = EnvironmentManager.Create()
                .WithSource(cfg => cfg.GetFactoryValue<string>("foo"),
                    cfg => cfg.SetFactoryValue("foo", () => "foo"))
                .WithSource(cfg => cfg.GetFactoryValue<string>("foo"),
                    cfg => cfg.SetFactoryValue("foo", () => "bar"))
                .Build();
            Assert.Equal("foo", builder);
        }

        [Fact]
        public void ConvetValueTest()
        {
            Assert.Equal(1,EnvironmentManager.Create().Default("1").Build<int>());
        }
        [Fact]
        public void ThrowsWhenCannotConvert()
        {
            Assert.Throws<FormatException>(()=>EnvironmentManager.Create().Default("x").Build<int>());
        }
    }

    public class EnvironmentVariableExtensionsTests
    {
        [Fact]
        public void VoidTest()
        {
            Environment.GetEnvironmentVariable("foo");
        }
    }

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
