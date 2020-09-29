using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
using Xunit;

namespace EnvironmentBuilderTests
{
    public class FlowTests
    {
        [Fact]
        public void ShouldBranchWhenAnyOf()
        {
            var flow = Flow.Create()
                .OneOf(
                    f => f.Case(c => c.Arg("a").Bundle()),
                    f => f.AllOf(c =>
                          c.Case(x => x.Arg("b").Bundle())
                        )
                );
        }
    }
}
