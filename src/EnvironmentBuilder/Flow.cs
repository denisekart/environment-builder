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

        //public IList<IFlow> OneOfFlows { get; set; } = new List<IFlow>();
        //public IList<IFlow> AllOfFlows { get; set; } = new List<IFlow>();
        //public IList<IFlow> AnyOfFlows { get; set; } = new List<IFlow>();

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

        public static FlowResult<T> Create<T>(Func<IEnvironmentBuilder, T> definition)
        {
            var flow = Create();
            return new FlowResult<T>(definition.Invoke(flow), flow);
        }

        private Flow(IEnvironmentBuilder builder, IFlow parent = null)
        {
            _builder = builder;
            _parent = parent;
        }

        //public T Case<T>(Func<IFlow, T> definition) where T : class
        //{
        //    return definition.Invoke(null);
        //}

        //public Resolvable<T> Case<T>(Func<IEnvironmentBundle, Resolvable<T>> definition)
        //{
        //    return definition.Invoke(null);
        //}

        //public IFlow Case(Func<IEnvironmentBuilder, IEnvironmentBundle> sources)
        //{
        //    var bundle = sources?.Invoke(_builder);
        //    throw new NotImplementedException();
        //}

        //public IFlow OneOf(params Action<IFlow>[] cases)
        //{
        //    throw new NotImplementedException();
        //}

        //public IFlow AllOf(params Action<IFlow>[] cases)
        //{
        //    throw new NotImplementedException();
        //}

        //public IFlow AnyOf(params Action<IFlow>[] cases)
        //{
        //    throw new NotImplementedException();
        //}

        //public IFlow And => _parent ?? throw new InvalidOperationException();
        //public IFlow Or => _parent ?? throw new InvalidOperationException();

        //public void Validate()
        //{

        //}


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
        //Resolvable<T> Case<T>(Func<IEnvironmentBundle, Resolvable<T>> definition);
        //T Case<T>(Func<IFlow, T> definition) where T : class;
        //IFlow Case(Func<IEnvironmentBuilder, IEnvironmentBundle> sources);
        //IFlow OneOf(params Action<IFlow>[] cases);
        //IFlow AllOf(params Action<IFlow>[] cases);
        //IFlow AnyOf(params Action<IFlow>[] cases);
        //IFlow And { get; }
        //IFlow Or { get; }
    }


    public static class FlowExtensions
    {
        public static Resolvable<T> As<T>(this IEnvironmentBuilder bundle)
        {
            return new Resolvable<T>(bundle.Bundle().Build<T>);
        }

        public static T And<T>(this IEnvironmentBuilder builder, Func<T> section)
        {
            if (builder.Bundle().Build<bool>())
            {
                // start here from the begining and just re-set the parent/child flow when done
                return new Resolvable<T>(section)
                {
                    Scope = builder as IFlow
                }.Value;
            }

            return default;
        }
    }
}
