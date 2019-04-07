using System.Linq;
using EnvironmentBuilder.Implementation.Json;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class Foo
    {
        public bool foo { get; set; } = true;
        public bool? bar { get; set; } = null;
    }

    public class Bar
    {
        public string str { get; set; } = "foo";
    }
    public class JsonParserTests
    {
        [Fact]
        public void ParseTrueTest()
        {
            var json = "{\"foo\":true,\"bar\":null}";
            var value = JsonManager.Parse(json) as JContentNode;

            Assert.Equal(true, (value["foo"] as JNode).Value);
            Assert.Null((value["bar"] as JNode).Value);
        }
        [Fact]
        public void ParseTrueGenericTest()
        {
            var json = "{\"foo\":true}";
            var value = JsonManager.Parse<Foo>(json);
            Assert.Equal(true,value.foo);
        }
        [Fact]
        public void ParseFalseTest()
        {
            var json = "{\"foo\":false}";
            var value = JsonManager.Parse(json) as JContentNode;
            Assert.Equal(false, (value["foo"] as JNode).Value);
        }

        [Fact]
        public void ParseStringTest()
        {
            var json = "{\"foo\":\"bar\"}";
            var value = JsonManager.Parse(json) as JContentNode;
            Assert.Equal("bar", (value["foo"] as JNode).Value);
        }
        [Fact]
        public void ParseEnumerationTest()
        {
            var json = "{\"foo\":[1,2,3]}";
            var value = JsonManager.Parse(json) as JContentNode;
            var enumeration = value["foo"] as JEnumerationNode;
            var values = enumeration.Cast<int>();
            Assert.True(new[]{1,2,3}.SequenceEqual(values));
        }
        [Fact]
        public void ParseNestedTest()
        {
            var json = "{\"foo\":{\"bar\":\"baz\",\"enum\":[1,2,3],\"bool\":true}}";
            var value = JsonManager.Parse(json) as JContentNode;
            var foo = value["foo"] as JContentNode;
            var bar = foo["bar"] as JValueNode;//baz
            var enu = foo["enum"] as JEnumerationNode;//1,2,3
            var b = foo["bool"] as JValueNode;//true
            Assert.True(b.Cast<bool>());
            Assert.True(new[]{1,2,3}.SequenceEqual(enu.Cast<int>()));
            Assert.Equal("baz",bar.Cast<string>());
        }

        [Fact]
        public void ExpandPathTest()
        {
            var json = "{\"foo\":1}";
            var endNode = JsonManager.Find("$.foo", json);
            Assert.Equal(1,JsonManager.Parse<int>(endNode));
        }
        [Fact]
        public void ExpandPathInnerTest()
        {
            var json = "{\"foo\":{\"bar\":\"baz\",\"enum\":[1,2,3],\"bool\":true}}";

            var endNode = JsonManager.Find("$.foo.enum[0]", json);
            Assert.Equal(1, JsonManager.Parse<int>(endNode));
        }
    }
}