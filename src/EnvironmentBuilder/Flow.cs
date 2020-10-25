using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Flows;
using System;
using System.Collections.Generic;

namespace EnvironmentBuilder
{
    public class Flow : IFlow
    {
        private readonly IEnvironmentBuilder _builder;

        private static IFlow Create(Action<IEnvironmentConfiguration> configuration)
        {
            var builder = EnvironmentManager.Create(configuration);

            return new Flow(builder);
        }

        public static FlowResult<T> Create<T>(Func<IFlow, T> definition, Action<IEnvironmentConfiguration> configuration = null)
        {
            var flow = Create(configuration);
            return new FlowResult<T>(definition.Invoke(flow), flow);
        }

        private Flow(IEnvironmentBuilder builder)
        {
            _builder = builder;
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
}
