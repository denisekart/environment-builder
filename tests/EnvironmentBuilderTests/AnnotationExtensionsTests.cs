using System.Linq;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class AnnotationExtensionsTests
    {
        [Fact]
        public void WithGlobalDescriptionTest()
        {
            var d = "some description";
            var env = EnvironmentManager.Create(x => x.WithDescription(d));
            Assert.Equal(d,env.GetDescription());
        }
        [Fact]
        public void WithGlobalDescriptionRepeatedLastOneWinsTest()
        {
            var d = "some description";
            var env = EnvironmentManager.Create(x => x.WithDescription("some other description").WithDescription(d));
            Assert.Equal(d, env.GetDescription());
        }

        [Fact]
        public void WithBundleDescriptionTest()
        {
            var d = "some description";
            var env = EnvironmentManager.Create();
            var bundle = env.WithDescription(d).Default("foo").Bundle();

            Assert.True(new[]{d}.SequenceEqual(env.Bundles.GetDescriptions()));

        }
        [Fact]
        public void WithBundleDescriptionOnMultiConfiguration1Test()
        {
            var d = "some description";
            var env = EnvironmentManager.Create();
            env.Default("foo").Bundle();
            env.WithDescription(d).Default("foo").Bundle();

            Assert.True(new[] { null,d }.SequenceEqual(env.Bundles.GetDescriptions()));

        }
        [Fact]
        public void WithBundleDescriptionOnMultiConfiguration2Test()
        {
            var d = "some description";
            var env = EnvironmentManager.Create();

            env.WithDescription(d).Default("foo").Bundle();
            env.Default("foo").Bundle();

            Assert.True(new[] {  d,null }.SequenceEqual(env.Bundles.GetDescriptions()));

        }

        [Fact]
        public void WithBundleDescriptionOnMultiConfiguration3Test()
        {
            var d = "some description";
            var env = EnvironmentManager.Create();
            env.Default("foo").Bundle();
            env.WithDescription(d).Default("foo").Bundle();
            env.Default("foo").Bundle();


            Assert.True(new[] {null, d, null }.SequenceEqual(env.Bundles.GetDescriptions()));

        }
        [Fact]
        public void WithBundleDescriptionOnMultiConfiguration4Test()
        {
            var d = "some description1";
            var d2 = "some description2";
            var env = EnvironmentManager.Create();
            env.WithDescription(d).Default("foo").Bundle();
            env.Default("foo").Bundle();

            env.WithDescription(d2).Default("foo").Bundle();
            env.Default("foo").Bundle();


            Assert.True(new[] { d, null,d2,null }.SequenceEqual(env.Bundles.GetDescriptions()));

        }

        [Fact]
        public void MergedDescriptionsTest()
        {
            var g = "some global description";
            var d = "some description1";
            var d2 = "some description2";
            var env = EnvironmentManager.Create(x=>x.WithDescription(g));
            env.WithDescription(d).Default("foo").Bundle();
            env.Default("foo").Bundle();

            env.WithDescription(d2).Default("foo").Bundle();
            env.Default("foo").Bundle();


            Assert.True(new[] { d, null, d2, null }.SequenceEqual(env.Bundles.GetDescriptions()));
            Assert.Equal(g,env.GetDescription());
        }
    }
}