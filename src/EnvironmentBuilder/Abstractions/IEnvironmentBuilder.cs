using System;
using System.Collections.Generic;

namespace EnvironmentBuilder.Abstractions
{
    public interface IEnvironmentBuilder
    {
        IReadonlyEnvironmentConfiguration Configuration { get; }
        IEnumerable<IEnvironmentBundle> Bundles { get; }
        IEnvironmentBundle Bundle();
        string Build();
        T Build<T>();
        IEnvironmentBuilder WithConfiguration(Action<IEnvironmentConfiguration> configuration);
        IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source);
        IEnvironmentBuilder WithSource<T>(Func<T> source);
        IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source,
            Action<IEnvironmentConfiguration> configuration);
    }
}