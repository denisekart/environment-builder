using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Extensions;
using EnvironmentBuilder.Implementation;

[assembly:InternalsVisibleTo("EnvironmentBuilderTests")]
namespace EnvironmentBuilder
{
    public class EnvironmentManager : IEnvironmentBuilder
    {
        private readonly IList<IEnvironmentBundle> _bundles=new List<IEnvironmentBundle>();
        private EnvironmentBundle _instance=new EnvironmentBundle();
        private IEnvironmentConfiguration _configuration=new EnvironmentConfiguration();
        public static IEnvironmentBuilder Create()=>new EnvironmentManager();
        public static IEnvironmentBuilder Create(Action<IEnvironmentConfiguration> configuration)=>new EnvironmentManager(configuration);

        internal EnvironmentManager()
        {
            _configuration.WithNoopLogger();
        }
        internal EnvironmentManager(Action<IEnvironmentConfiguration> configuration) :this()
        {
            configuration?.Invoke(_configuration);
        }

        public IEnvironmentBuilder WithConfiguration(Action<IEnvironmentConfiguration> configuration)
        {
            configuration?.Invoke(_configuration);
            return this;
        }

        public IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source)
        {
            _instance.AddSource(source, _configuration.Clone());
            return this;
        }

        public IEnvironmentBuilder WithSource<T>(Func<T> source)
        {
            return WithSource(cfg => source.Invoke());
        }

        public IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source, Action<IEnvironmentConfiguration> configuration)
        {
            var cfg = _configuration?.Clone();
            configuration?.Invoke(cfg);
            _instance.AddSource(source, cfg);
            return this;
        }

        public IReadonlyEnvironmentConfiguration Configuration => _configuration;
        public IEnumerable<IEnvironmentBundle> Bundles => _bundles;

        public IEnvironmentBundle Bundle()
        {
            var instance = _instance;
            _instance=new EnvironmentBundle();
            _bundles.Add(instance);
            return instance;
        }
        public string Build()
        {
            return Bundle().Build();
        }
        public T Build<T>()
        {
            return Bundle().Build<T>();
        }  
    }
}

