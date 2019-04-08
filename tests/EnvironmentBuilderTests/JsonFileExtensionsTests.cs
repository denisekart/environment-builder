using System.Linq;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class JsonFileExtensionsTests
    {
        [Fact]
        public void WithJsonFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithJsonFile("json1.json"));
            var parser = builder.Configuration.GetValue<JsonFileParser>(typeof(JsonFileParser).FullName);
            Assert.Contains("json1.json", parser.Files.Keys);
        }
        [Fact]
        public void WithMissingJsonFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithJsonFile("jsonxxx.json"));
            var parser = builder.Configuration.GetValue<JsonFileParser>(typeof(JsonFileParser).FullName);
            //does not throw
        }
        [Fact]
        public void WithEagerLoadedJsonFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithJsonFile("json1.json",true));
            var parser = builder.Configuration.GetValue<JsonFileParser>(typeof(JsonFileParser).FullName);
            Assert.Contains("json1.json", parser.Files.Keys);
            Assert.NotNull(parser.Files.Values.ElementAt(0).ParsedFile);
        }
        [Fact]
        public void WithMultipleFilesTest()
        {
            var builder = EnvironmentManager.Create(x => 
                x.WithJsonFile("json1.json").WithJsonFile("json2.json"));
            var parser = builder.Configuration.GetValue<JsonFileParser>(typeof(JsonFileParser).FullName);
            Assert.Contains("json1.json", parser.Files.Keys);
            Assert.Contains("json2.json", parser.Files.Keys);

        }
        [Fact]
        public void WithEagerLoadedMultipleFilesTest()
        {
            var builder = EnvironmentManager.Create(x =>
                x.WithJsonFile("json1.json",true).WithJsonFile("json2.json",true));
            var parser = builder.Configuration.GetValue<JsonFileParser>(typeof(JsonFileParser).FullName);
            Assert.Contains("json1.json", parser.Files.Keys);
            Assert.Contains("json2.json", parser.Files.Keys);
            Assert.NotNull(parser.Files.Values.ElementAt(0).ParsedFile);
            Assert.NotNull(parser.Files.Values.ElementAt(1).ParsedFile);

        }
        [Fact]
        public void WithSingleFileSingleStringKeyTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithJsonFile("json1.json"));
            var bundle = builder.Json("$.foo").Bundle();
            var value = bundle.Build<string>();
            Assert.Equal("bar",value);
        }
        [Fact]
        public void WithMultipleFilesSingleStringKeyTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithJsonFile("json1.json").WithJsonFile("json1.json"));
            var bundle = builder.Json("$(json1).foo").Bundle();
            var value = bundle.Build<string>();
            Assert.Equal("bar", value);
        }
        [Fact]
        public void WithMultipleFilesSingleStringKeyWithExtensionTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithJsonFile("json1.json").WithJsonFile("json2.json"));
            var bundle = builder.Json("$(json1.json).foo").Bundle();
            var value = bundle.Build<string>();
            Assert.Equal("bar", value);
        }

        [Fact]
        public void WithInvalidPathTest()
        { 
            var builder = EnvironmentManager.Create(x => x.WithJsonFile("json1.json").WithJsonFile("json2.json"));
            var bundle = builder.Json("--qwertz").Bundle();
            var value = bundle.Build<string>();
            Assert.Null(value);
        }
        [Fact]
        public void WithInvalidFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithJsonFile("json1.json").WithJsonFile("json2.json"));
            var bundle = builder.Json("$(json2)..foo").Bundle();
            var value = bundle.Build<string>();
            Assert.Null(value);
        }
    }
}