using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using EnvironmentBuilder.Implementation;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class XmlFileExtensionsTests
    {
        [Fact]
        public void WithXmlFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithXmlFile("xml1.xml"));
            var parser = builder.Configuration.GetValue<XmlFileParser>(typeof(XmlFileParser).FullName);
            Assert.Contains("xml1.xml", parser.Files.Keys);
        }
        [Fact]
        public void WithMissingXmlFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithXmlFile("xmlxxxxxxxx.xml"));
            var parser = builder.Configuration.GetValue<XmlFileParser>(typeof(XmlFileParser).FullName);
            //does not throw
        }
        [Fact]
        public void WithMultipleXmlFilesTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithXmlFile("xml1.xml").WithXmlFile("xml2.xml"));
            var parser = builder.Configuration.GetValue<XmlFileParser>(typeof(XmlFileParser).FullName);
            Assert.Contains("xml1.xml", parser.Files.Keys);
            Assert.Contains("xml2.xml", parser.Files.Keys);
        }

        [Fact]
        public void WithValidStringPathAndSingleFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithXmlFile("xml1.xml"));
            var bundle = builder.Xml("/foo").Bundle();
            var value = bundle.Build();
            Assert.Equal("bar",value);
        }
        [Fact]
        public void WithValidStringPathAndMultiFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithXmlFile("xml1.xml").WithXmlFile("xml2.xml"));
            var bundle = builder.Xml("/bar","xml2").Bundle();
            var value = bundle.Build();
            Assert.Equal("baz", value);
        }
        [Fact]
        public void WithInvalidPathTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithXmlFile("xml1.xml"));
            var bundle = builder.Xml("/xxx").Bundle();
            var value = bundle.Build();
            Assert.Null(value);
        }
        [Fact]
        public void WithInvalidFileTest()
        {
            var builder = EnvironmentManager.Create(x => x.WithXmlFile("xml1đđxx.xml"));
            var bundle = builder.Xml("/xxx").Bundle();
            var value = bundle.Build();
            Assert.Null(value);
        }
    }
}