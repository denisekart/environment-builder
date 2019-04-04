namespace EnvironmentBuilder.Abstractions
{
    public interface IReadonlyEnvironmentConfiguration
    {
        IEnvironmentConfiguration Clone();
        string GetValue(string key);
        T GetValue<T>(string key);
        T GetFactoryValue<T>(string key);
        bool HasValue(string key);
    }
}