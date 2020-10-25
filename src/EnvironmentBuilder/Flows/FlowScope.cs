using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Flows
{
    public class FlowScope
    {
        public IEnvironmentBuilder PreviousState { get; set; }
    }
}