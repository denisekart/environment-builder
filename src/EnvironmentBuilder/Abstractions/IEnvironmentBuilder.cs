using System;
using System.Collections.Generic;

namespace EnvironmentBuilder.Abstractions
{
    public interface IEnvironmentBuilder
    {
        /// <summary>
        /// Returns the current configuration
        /// </summary>
        IReadonlyEnvironmentConfiguration Configuration { get; }
        /// <summary>
        /// Returns all of the bundles already bundled
        /// </summary>
        IEnumerable<IEnvironmentBundle> Bundles { get; }
        /// <summary>
        /// Bundles the current pipe
        /// </summary>
        /// <returns></returns>
        IEnvironmentBundle Bundle();
        /// <summary>
        /// Builds the current pipe
        /// </summary>
        /// <returns>the value</returns>
        string Build();
        /// <summary>
        /// Builds the current pipe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>the typed value</returns>
        T Build<T>();
        /// <summary>
        /// Configures the current instance with the confguration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IEnvironmentBuilder WithConfiguration(Action<IEnvironmentConfiguration> configuration);
        /// <summary>
        /// Adds a source to the pipe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source);
        /// <summary>
        /// Adds a source to the pipe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        IEnvironmentBuilder WithSource<T>(Func<T> source);
        /// <summary>
        /// Adds a souce to the pipe and configures it with the configuration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source,
            Action<IEnvironmentConfiguration> configuration);
    }
}