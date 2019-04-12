using System;
using System.Collections.Generic;
using EnvironmentBuilder;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Extensions;
using EnvironmentBuilderRandomContrib.Extensions;
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
    }
}

namespace EnvironmentBuilderRandomContrib.Extensions
{
    // The static class that will hold the extension methods
    public static class RandomExtensions
    {
        // Unique keys to use in the configuration - they should be unique system wide, perhaps the assembly name and description
        public const string RngKey = "RandomExtensions.RngKey";
        public const string SaverKey = "RandomExtensions.NumberSaverKey";

        // This method will be used to add the RNG to the global configuation
        public static IEnvironmentConfiguration WithRandomNumberGenerator(this IEnvironmentConfiguration configuration,
            Random random)
        {
            return configuration.SetFactoryValue(RngKey, () => random.Next());
        }
        // this method will be used to get the next randomly incremented number
        public static IEnvironmentBuilder Random(this IEnvironmentBuilder builder)
        {
            if (!builder.Configuration.HasValue(SaverKey))
            {
                builder.WithConfiguration(cfg => cfg.SetValue(SaverKey, new NumberSaver()));
            }

            return builder.WithSource(config =>
            {
                var nextValue = config.GetFactoryValue<int>(RngKey);
                var saver = config.GetValue<NumberSaver>(SaverKey);
                var oldKey = saver.Prev;
                saver.Prev += nextValue;
                return oldKey;

            }, config => config.Trace("[my random source]"));
        }

        private class NumberSaver
        {
            public int Prev { get; set; } = 0;
        }

    }
}
