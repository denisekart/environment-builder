namespace EnvironmentBuilder.Flows
{
    public struct FlowResult<T>
    {
        public T Value { get; }
        public IFlow Flow { get; }

        public FlowResult(T value, IFlow flow)
        {
            Value = value;
            Flow = flow;
        }
    }
}