using System.Collections.Generic;
using System.Linq;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using EnvironmentBuilder.Implementation.Json;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class ArgumentExtensionsTests
    {
        [Fact]
        public void WithBuilderTest()
        {
            var builder = EnvironmentManager.Create(x => x.SetValue(typeof(ArgumentParser).FullName,
                new ArgumentParser("--foo foo --bar bar --baz baz"
                    .Split(' ').ToArray())));
            var value=builder.Arg("foo").Build();
            Assert.Equal("foo",value);
        }

        [Fact]
        public void ArgumentParserStringTest()
        {
            var parser=new ArgumentParser(new []{"--foo", "foo", "--bar","bar"});
            var value = parser.Value("foo", typeof(string));
            Assert.Equal("foo",value as string);
        }

        [Fact]
        public void ArgumentParserStringReplayTest()
        {
            var parser = new ArgumentParser(new[] { "--foo", "foo", "--foo", "bar" });
            var value = parser.Value("foo", typeof(string));
            Assert.Equal("bar", value as string);
        }

        [Fact]
        public void ArgumentParserBoolWithValueTest()
        {
            var parser = new ArgumentParser(new[] { "--foo", "true"});
            var value = parser.Value("foo", typeof(bool));
            Assert.Equal(true, value);
        }

        [Fact]
        public void ArgumentParserBoolWithNoValueTest()
        {
            var parser = new ArgumentParser(new[] { "--foo" });
            var value = parser.Value("foo", typeof(bool));
            Assert.Equal(true, value);
        }
        [Fact]
        public void ArgumentParserDuplicateValueMergeTest()
        {
            var parser = new ArgumentParser(new[] { "--foo","foo","bar" });
            var value = parser.Value("foo", typeof(string));
            Assert.Equal("foo bar", value);
        }
        [Fact]
        public void ArgumentParserValueWithEqualStringTest()
        {
            var parser = new ArgumentParser(new[] { "--foo=foo", "--bar=true" });
            var value = parser.Value("foo", typeof(string));
            Assert.Equal("foo", value);
        }
        [Fact]
        public void ArgumentParserValueWithEqualBoolTest()
        {
            var parser = new ArgumentParser(new[] { "--foo=foo", "--bar=true" });
            var value = parser.Value("bar", typeof(bool));
            Assert.Equal(true, value);
        }

        [Fact]
        public void ArgumentParserValueWithEqualBoolWhenStringTest()
        {
            var parser = new ArgumentParser(new[] { "--foo=foo", "--bar" });
            var value = parser.Value("bar", typeof(string));
            Assert.Equal(true.ToString(), value);
        }

        [Fact] 
        public void ArgumentParserValueCaseInsensitiveTest()
        {
            var parser = new ArgumentParser(new[] { "--foo=foo", "--bar" });
            var value = parser.Value("BAR", typeof(string));
            Assert.Equal(true.ToString(), value);
        }

        [Fact]
        public void ArgumentParserValueComplex1Test()
        {
            var parser = new ArgumentParser(new[] { "--foo=\"foo bar baz\"" });
            var value = parser.Value("foo", typeof(string));
            Assert.Equal("foo bar baz", value);
        }
        [Fact]
        public void ArgumentParserValueComplex2Test()
        {
            var parser = new ArgumentParser(new[] { "--foo=\"\"foo bar baz\"\"" });
            var value = parser.Value("foo", typeof(string));
            Assert.Equal("\"foo bar baz\"", value);
        }
        [Fact]
        public void ArgumentParserValueComplex3Test()
        {
            var parser = new ArgumentParser(new[] { "--foo=\" \"" });
            var value = parser.Value("foo", typeof(string));
            Assert.Equal(" ", value);
        }

        [Fact]
        public void ArgumentParserValueComplex4Test()
        {
            var parser = new ArgumentParser(new[] { "--foo=\"\"\"" });
            var value = parser.Value("foo", typeof(string));
            Assert.Equal("\"", value);
        }
        [Fact]
        public void ArgumentParserValueComplex5Test()
        {
            var parser = new ArgumentParser(new[] { "--foo=\"\"" });
            var value = parser.Value("foo", typeof(string));
            Assert.Equal(string.Empty, value);
        }

        [Fact]
        public void EnumerableStringTest()
        {
            var parser = new ArgumentParser(new[] { "--foo", "foo", "--foo","bar" });
            var value = parser.Value("foo", typeof(IEnumerable<string>));
            Assert.True(new[] {"foo","bar"}.SequenceEqual(value as IEnumerable<string>)); 
        }
    }
}