using System.Collections.Generic;

namespace EnvironmentBuilder.Abstractions
{
    public interface IEnvironmentBundle
    {
        IEnumerable<IReadonlyEnvironmentConfiguration> Sources { get; }

        string Build();
        T Build<T>();
        //void AddSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source, IEnvironmentConfiguration configuration);
    }
}