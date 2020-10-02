using System.Collections.Generic;

namespace EnvironmentBuilder.Abstractions
{
    public interface IEnvironmentBundle : IEnvironmentBuilder
    {
        IEnumerable<IReadonlyEnvironmentConfiguration> Sources { get; }
        string Build();
        T Build<T>();
    }
}