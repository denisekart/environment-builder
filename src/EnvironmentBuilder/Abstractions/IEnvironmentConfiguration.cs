using System;

namespace EnvironmentBuilder.Abstractions
{
    public interface IEnvironmentConfiguration : IReadonlyEnvironmentConfiguration
    {
        IEnvironmentConfiguration SetValue<T>(string key, T value);
        IEnvironmentConfiguration SetFactoryValue<T>(string key, Func<T> value);
    }
}