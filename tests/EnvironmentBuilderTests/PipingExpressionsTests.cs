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
    }
}
