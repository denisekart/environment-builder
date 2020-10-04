using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class FlowTests
    {
        //    [Fact]
        //    public void ShouldBranchWhenAnyOf()
        //    {
        //        var flow = Flow.Create()
        //            .OneOf(
        //                f => f.Case(c => c.Arg("a").Bundle()),
        //                f => f.AllOf(c =>
        //                      c.Case(x => x.Arg("b").Bundle())
        //                          .AnyOf(
        //                            b => b.Case(x => x.Arg("C").Bundle())
        //                          )
        //                    )
        //            );
        //    }

        //    [Fact]
        //    public void ShouldObeyLogicalAndRule()
        //    {
        //        var flow = Flow.Create()
        //            .Case(x => x.Arg("a").Bundle())
        //            .And
        //            .Case(x => x.Arg("b").Bundle());
        //    }

        [Fact]
        public void ShouldResolveStrongTypedCase()
        {
            var flow = Flow.Create(x => new
            {
                optionA = x.Arg("foo").As<int>(),
                optionB = new
                {
                    optionC = x.Arg("bar").Default(true)
                        .And(() => new
                        {
                            optionD = x.Arg("baz").As<string>(),
                            optionE = x.Arg("baz2").Default(true).And(() => new
                            {

                            })
                        })
                }
            });
            var value = flow.Model.optionB.optionC.optionD == "2";
        }

        [Fact]
        public void ShouldResolveEntryConfiguration()
        {
            var flow = Flow.Create(x => new EntryConfiguration
            {
                Pack = x.Arg("p").Default(true).As<bool>(),
                
            });
            Assert.True(flow.Model.Pack);
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
