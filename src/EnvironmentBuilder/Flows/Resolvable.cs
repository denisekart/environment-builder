using System;

namespace EnvironmentBuilder.Flows
{
    public class Resolvable<T>
    {
        private readonly Func<T> _valueFactory;
        private T _value;
        private bool _resolved = false;

        public FlowScope Scope { get; set; }

        public Resolvable(T value)
        {
            _value = value;
            _resolved = true;
        }

        public Resolvable(Func<T> valueFactory)
        {
            _valueFactory = valueFactory;
        }

        public T Value => _resolved
            ? _value
            : ResolveValue();

        private T ResolveValue()
        {
            _value = _valueFactory.Invoke();
            _resolved = true;
            return _value;
        }

        public static implicit operator T(Resolvable<T> wrapper)
        {
            return wrapper.Value;
        }
    }
}