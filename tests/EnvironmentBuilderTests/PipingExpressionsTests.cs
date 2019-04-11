using System.Collections.Generic;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class PipingExpressionsTests
    {
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
    }
}
