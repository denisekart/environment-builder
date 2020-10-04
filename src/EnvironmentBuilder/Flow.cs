using EnvironmentBuilder.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace EnvironmentBuilder
{
    public struct FlowResult<T>
    {
        public T Model { get; }
        public IFlow Flow { get; }

        public FlowResult(T model, IFlow flow)
        {
            Model = model;
            Flow = flow;
        }
    }
    public class Flow : IFlow
    {
        private readonly IEnvironmentBuilder _builder;
        private readonly IFlow _parent;

        private readonly Stack<IFlow> _scopes = new Stack<IFlow>();

        public void CreateScope()
        {
            _scopes.Push(new Flow(_builder, this));
        }
        public void CommitScope()
        {
            var scope = _scopes.Pop();
        }

        public static IFlow Create(Action<IEnvironmentConfiguration> configuration = null)
        {
            var builder = EnvironmentManager.Create(configuration);

            return new Flow(builder);
        }

        public static FlowResult<T> Create<T>(Func<IFlow, T> definition)
        {
            var flow = Create();
            return new FlowResult<T>(definition.Invoke(flow), flow);
        }

        private Flow(IEnvironmentBuilder builder, IFlow parent = null)
        {
            _builder = builder;
            _parent = parent;
        }

        #region Implementation of IEnvironmentBuilder

        public IReadonlyEnvironmentConfiguration Configuration => _builder.Configuration;

        public IEnumerable<IEnvironmentBundle> Bundles => _builder.Bundles;

        public IEnvironmentBundle Bundle()
        {
            return _builder.Bundle();
        }

        public string Build()
        {
            return _builder.Build();
        }

        public T Build<T>()
        {
            return _builder.Build<T>();
        }

        public IEnvironmentBuilder WithConfiguration(Action<IEnvironmentConfiguration> configuration)
        {
            _builder.WithConfiguration(configuration);
            return this;
        }

        public IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source)
        {
            _builder.WithSource(source);
            return this;
        }

        public IEnvironmentBuilder WithSource<T>(Func<T> source)
        {
            _builder.WithSource(source);
            return this;
        }

        public IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source, Action<IEnvironmentConfiguration> configuration)
        {
            _builder.WithSource(source, configuration);
            return this;
        }

        #endregion
    }

    public class Resolvable<T> {
        private readonly Func<T> _valueFactory;
        private T _value;
        private bool _resolved = false;

        public IFlow Scope { get; set; }

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
            //todo what if the value throws?
            Scope?.CreateScope();
            _value = _valueFactory.Invoke();
            _resolved = true;
            Scope?.CommitScope();
            return _value;
        }

        public static implicit operator T(Resolvable<T> wrapper)
        {
            return wrapper.Value;
        }
    }

    
    public interface IFlow : IEnvironmentBuilder
    {
        void CreateScope();
        void CommitScope();
    }


    public static class FlowExtensions
    {
        public static Resolvable<T> Verify<T>(this Resolvable<T> resolvable, Func<T, bool> verificationFactory)
        {

        }

        public static Resolvable<T> As<T>(this IEnvironmentBuilder bundle)
        {
            return new Resolvable<T>(bundle.Bundle().Build<T>);
        }

        public static T And<T>(this IEnvironmentBuilder builder, Func<T> section)
        {
            if (builder.Bundle().Build<bool>())
            {
                return new Resolvable<T>(section)
                {
                    Scope = builder as IFlow
                }.Value;
            }

            return default;
        }
    }
}
