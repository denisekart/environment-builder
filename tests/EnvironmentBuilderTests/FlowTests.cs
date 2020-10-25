using System;
using System.Runtime.CompilerServices;
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class FlowTests
    {
        public static void Setx(string name, string value, [CallerMemberName] string caller = null)
        {
            Environment.SetEnvironmentVariable($"{nameof(FlowTests)}_{caller}_{name}", value);
        }
        public static string Getx(string name, [CallerMemberName] string caller = null)
        {
            return $"{nameof(FlowTests)}_{caller}_{name}";
        }

        [Fact]
        public void ShouldResolveSimpleOption()
        {
            Setx("foo", "bar");
            var flowSpec = Flow.Create(x => new
            {
                optionA = x.Env(Getx("foo")).As<string>()
            });

            Assert.NotNull(flowSpec.Value);
            Assert.Equal("bar", flowSpec.Value.optionA);
        }

        [Fact]
        public void ShouldResolveSimpleNestedOption()
        {
            Setx("foo", "bar");
            var flowSpec = Flow.Create(x => new
            {
                optionA = new
                {
                    optionB = x.Env(Getx("foo")).As<string>()
                }
            });

            Assert.NotNull(flowSpec.Value);
            Assert.Equal("bar", flowSpec.Value.optionA.optionB);
        }

        [Fact]
        public void ShouldResolveSimpleCorrectNestedOption_BasedOnCondition()
        {
            Setx("foo", "bar");
            var flowSpec = Flow.Create(x => new
            {
                optionA = new
                {
                    optionB = x.Env(Getx("foo"))
                        .When("no", ()=>"incorrect")
                        .When("bar", ()=>"correct")
                        .When("baz", ()=> "incorrect")
                        .When("baf", ()=> "incorrect")
                }
            });

            Assert.NotNull(flowSpec.Value);
            Assert.Equal("correct", flowSpec.Value.optionA.optionB);
        }

        [Fact]
        public void ShouldResolveComplexCorrectNestedOption_BasedOnCondition()
        {
            Setx("foo", "bar");
            var flowSpec = Flow.Create(x => new
            {
                optionA = new
                {
                    optionB = x.Env(Getx("foo"))
                        .When("no", () => new {optionC = "incorrect"})
                        .When("bar", () => new { optionC = "correct" })
                        .When("baz", () => new { optionC = "incorrect" })
                        .Value
                }
            });

            Assert.NotNull(flowSpec.Value);
            Assert.Equal("correct", flowSpec.Value.optionA.optionB.optionC);
        }

        [Fact]
        public void ReturnsNoMatchesWhenNothingMatches()
        {
            Setx("foo", "bar");
            var flowSpec = Flow.Create(x => new
            {
                optionA = new
                {
                    optionB = x.Env(Getx("foo"))
                        .When("no", () => new { optionC = "incorrect" })
                        .When("bah", () => new { optionC = "incorrect" })
                        .When("baz", () => new { optionC = "incorrect" })
                        .Value
                }
            });

            Assert.NotNull(flowSpec.Value);
            Assert.Null(flowSpec.Value.optionA.optionB);
        }

        [Fact]
        public void ShouldResolveEntryConfiguration()
        {
            var flow = Flow.Create(x => new EntryConfiguration
            {
                Pack = x.Arg("p").Default(true).As<bool>(),
            });
            Assert.True(flow.Value.Pack);
        }

        [Fact]
        public void ShouldThrowLazilyWhenMissingRequiredProperty()
        {
            var flowSpec = Flow.Create(x => new
            {
                required = x.Env("required").Required<string>()
            });

            Assert.Throws<ArgumentException>(() => !string.IsNullOrEmpty(flowSpec.Value.required));
        }

        [Fact]
        public void ShouldThrowEagerlyWhenMissingRequiredPropertyOnValidateCall()
        {
            var flowSpec = Flow.Create(x => new
            {
                required = x.Env("required").Required<string>()
            });

            var ex = Assert.Throws<AggregateException>(() => flowSpec.Verify());
            Assert.StartsWith("The flow model is invalid.", ex.Message);
            Assert.Single(ex.InnerExceptions);
        }

        [Fact]
        public void ShouldNotThrowWhenRequiredPropertyExists()
        {
            Setx("required", "IAmAString");
            var flowSpec = Flow.Create(x => new
            {
                required = x.Env(Getx("required")).Required<string>()
            });

            flowSpec.Verify();
        }

        [Fact]
        public void ShouldResolveComplexScenarioWithEnumType()
        {
            Setx("a", "false");
            Setx("b", "true");
            Setx("c", "RandomValue2");

            var flow = EnvironmentBuilder.Flow.Create(x => new
            {
                isConsumer = x.Env(Getx("a")).As<bool>(),
                producer = x.Env(Getx("b"))
                    .When(true, () => new
                    {
                        scenario = x.Env("nonExistent").Env(Getx("c")).As<RandomEnum>().Required(),
                        NumberOfRequests = x.Arg("n").Arg("number").Default(1).As<int>(),
                        Timeout = x.Arg("t").Arg("timeout").Default(0).As<int>()
                    })
            }).Verify();
        }

        public class EntryConfiguration
        {
            public bool Pack { get; set; }
            public bool Unpack { get; set; }
            public bool Clean { get; set; }
            public bool Help { get; set; }
            public string Configuration { get; set; }

        }
    }
}
