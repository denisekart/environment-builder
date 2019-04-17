using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using EnvironmentBuilderRandomContrib.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace EnvironmentBuilderTests
{
    public class PipingExpressionsTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public PipingExpressionsTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void PipeConfiguration1()
        {
            Assert.Equal("bar",EnvironmentManager.Create().With("foo").Arg().Env().Json("$.foo").Xml("/foo").Default("bar").Build());
        }


        [Fact]
        public void PipeConfigurationReadme1()
        {
            //creates a lightweight object and configures it to:
            //use a json file named "json1.json"
            //use an xml file named "xml1.xml"
            //prefix all of the environment variables with the word "Foo_"
            //and use a simple console output for logging with the level of trace
            var env = EnvironmentManager.Create(config =>
                config.WithJsonFile("json1.json")
                    .WithXmlFile("xml1.xml")
                    .WithEnvironmentVariablePrefix("Foo_")
                    .WithConsoleLogger(EnvironmentBuilder.Abstractions.LogLevel.Trace));
            var actionBundle = env.Arg("action").Arg("a").Env("environment_action").Json("$.action").Xml("/action")
                .Default("foo_action").Bundle();
            var action = actionBundle.Build();

            Assert.Equal("foo_action",action);
        }

        class Foo
        {
            public string Bar { get; set; }
            public int Baz { get; set; }
        }

        [Fact]
        public void PipeConfigurationReadme2()
        {
            Random rnd =new Random();
            
            var env = EnvironmentManager.Create(x=>x.WithJsonFile("json3.json"));
            var value = env.Json("$").Build<Foo>();
            Assert.True(value is Foo f && f.Bar=="some value" && f.Baz==2);

        }

        [Fact]
        public void RandomExtensionTestCase1()
        {
            var env = EnvironmentManager.Create(cfg => cfg.WithRandomNumberGenerator(new Random(42)));
            int next1 = env.Random().Build<int>();
            int next2 = env.Random().Build<int>();
            int next3 = env.Random().Build<int>();
            int next4 = env.Random().Build<int>();

            Assert.True(next2>next1);
            Assert.True(next3>next2);
            Assert.True(next4>next3);
        }


        [Fact]
        public void DocumentCommonExtesions()
        {
            Assert.Equal("Foo",EnvironmentManager.Create().Default("Foo").Build());
            //is the same as
            Assert.Equal("Foo",EnvironmentManager.Create().WithDefaultValue("Foo").Build());

            Assert.Throws<ArgumentException>(() => EnvironmentManager.Create().Throw().Build());
            //is the same as
            Assert.Throws<ArgumentException>(() => EnvironmentManager.Create().WithException(null).Build());

            //will search for values of env(Key) then arg(Key)
            var builder=EnvironmentManager.Create().With("Key").Env().Arg();
            Assert.Equal("Key",builder.Configuration.GetCommonKey());
        }

        [Fact]
        public void DocumentAnnotationExtensions()
        {
            var env = EnvironmentManager.Create(config => config.WithDescription("Main description"));
            Assert.Equal("Main description",env.GetDescription());

            env.WithDescription("some source description").Default("foo").Bundle();
            Assert.True(new[]{ "some source description" }.SequenceEqual(env.Bundles.GetDescriptions()));

            env.WithDescription("d2").Throw("Throw").Bundle();
            var expected = string.Format(
                $"{{0}}{Environment.NewLine}{Environment.NewLine}{{1}}{Environment.NewLine}{{2}}{Environment.NewLine}{{3}}{Environment.NewLine}{{4}}{Environment.NewLine}",
                "Main description",
                "- [default]foo",
                "\tsome source description",
                "- [exception]Throw",
                "\td2");
            var help = env.GetHelp();
            Assert.Equal(
                expected,
                help);
        }

        [Fact]
        public void LoggingExtensions()
        {
            var writer=new TestOutputHelperWriter(_outputHelper);
            var env = EnvironmentManager.Create(config =>
                config
                    .WithTextWriterLogger(writer)
                    .WithLogLevel(EnvironmentBuilder.Abstractions.LogLevel.Trace));
            writer.TextWritten += (s, e) => Assert.True(e.Text == "[TRACE] Foo");
            env.LogTrace("Foo");
        }

        [Fact]
        public void CommandLineArgumentExtensions()
        {
            var env = EnvironmentManager.Create();
            var var1 = env.Arg("longOption").Arg("l").Bundle();
            
        }

        [Fact]
        public void EnvironmentVariableExtensions()
        {
            Environment.SetEnvironmentVariable("option","bar");
            Environment.SetEnvironmentVariable("prefix_option","foo");
            var env = EnvironmentManager.Create(config=>config.WithEnvironmentVariablePrefix("prefix_"));
            Assert.Equal("foo",env.Env("option").Build());
            Assert.Equal("bar",env.Env("option",c=>c.WithNoEnvironmentVariablePrefix()).Build());

        }

        [Fact]
        public void JsonFileExtensions()
        {
            var env = EnvironmentManager.Create(config => 
                config.WithJsonFile("json1.json").WithJsonFile("json2.json"));
            Assert.Equal("bar",env.Json("$(json1).foo").Build());
            Assert.Equal("baz",env.Json("$(json2).bar").Build());
        }

        [Fact]
        public void XmlFileExtensions()
        {
            var env = EnvironmentManager.Create(config => config.WithXmlFile("xml1.xml"));
            Assert.Equal("bar",env.Xml("/foo").Build());
        }
    }
}
