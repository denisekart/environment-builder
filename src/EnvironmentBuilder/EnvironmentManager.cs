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

        /// <summary>
        /// Creates a new IEnvironmentBuilder
        /// </summary>
        /// <returns></returns>
        public static IEnvironmentBuilder Create()=>new EnvironmentManager();
        /// <summary>
        /// Creates a new IEnvironmentBuilder and configures it using the <see cref="configuration"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Create(Action<IEnvironmentConfiguration> configuration)=>new EnvironmentManager(configuration);

        internal EnvironmentManager()
        {
            _configuration.WithNoopLogger();
        }
        internal EnvironmentManager(Action<IEnvironmentConfiguration> configuration) :this()
        {
            configuration?.Invoke(_configuration);
        }
        /// <summary>
        /// Configures the current instance with the <see cref="configuration"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IEnvironmentBuilder WithConfiguration(Action<IEnvironmentConfiguration> configuration)
        {
            configuration?.Invoke(_configuration);
            return this;
        }
        

        /// <summary>
        /// Adds the source to the pipeline
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source)
        {
            _instance.AddSource(source, _configuration.Clone());
            return this;
        }
        /// <summary>
        /// Adds the source to the pipeline
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnvironmentBuilder WithSource<T>(Func<T> source)
        {
            return WithSource(cfg => source.Invoke());
        }
        /// <summary>
        /// Adds the source to the pipeline and configures it with the <see cref="configuration"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
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

