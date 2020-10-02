using System;
using System.Collections.Generic;
using System.Linq;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Extensions;

namespace EnvironmentBuilder.Implementation
{
    internal class EnvironmentBundle :IEnvironmentBundle
    {
        public struct Source
        {
            public Func<IEnvironmentConfiguration, object> SourceItem;
            public IEnvironmentConfiguration Configuration;
        }

        private IList<Source> _orderedSources=new List<Source>();
        private IEnvironmentBuilder _builder;

        public EnvironmentBundle(IEnvironmentBuilder builder)
        {
            this._builder = builder;
        }

        public void AddSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source, IEnvironmentConfiguration configuration)
        {
            _orderedSources.Add(new Source{Configuration = configuration,SourceItem = cfg=>source.Invoke(cfg)});
        }

        public IEnumerable<IReadonlyEnvironmentConfiguration> Sources => _orderedSources.Select(x => x.Configuration).ToArray();

        public IReadonlyEnvironmentConfiguration Configuration => _builder.Configuration;

        public IEnumerable<IEnvironmentBundle> Bundles => _builder.Bundles;

        public IEnvironmentBundle Bundle()
        {
            return _builder.Bundle();
        }

        public string Build()
        {
            foreach (var source in _orderedSources)
            {
                var config = source.Configuration;
                config.SetBuildType<string>();
                var value = source.SourceItem?.Invoke(config);
                if (value != null)
                {
                    if (value is string s)
                        return s;
                    return value.ToString();
                }
            }
            return null;
        }
        public T Build<T>()
        {
            foreach (var source in _orderedSources)
            {

                var config = source.Configuration;
                config.SetBuildType<T>();
                var value = source.SourceItem?.Invoke(config);

                if (value != null)
                {
                    if (value is T s)
                        return s;
                    else if (Convert.ChangeType(value, typeof(T)) is T s2)
                        return s2;
                }
            }
            return default;
        }

        public IEnvironmentBuilder WithConfiguration(Action<IEnvironmentConfiguration> configuration)
        {
            return _builder.WithConfiguration(configuration);
        }

        public IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source)
        {
            return _builder.WithSource(source);
        }

        public IEnvironmentBuilder WithSource<T>(Func<T> source)
        {
            return _builder.WithSource(source);
        }

        public IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source, Action<IEnvironmentConfiguration> configuration)
        {
            return _builder.WithSource(source, configuration);
        }
    }
}