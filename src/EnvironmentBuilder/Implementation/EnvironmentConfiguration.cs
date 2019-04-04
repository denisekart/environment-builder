using System;
using System.Collections.Generic;
using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Implementation
{
    internal class EnvironmentConfiguration : IEnvironmentConfiguration
    {
        private Dictionary<string, object> _configuration=new Dictionary<string, object>();
        private Dictionary<string, Func<object>> _factories=new Dictionary<string, Func<object>>();

        public IEnvironmentConfiguration Clone()
        {
            var n=new EnvironmentConfiguration();
            foreach (var configurationKey in _configuration.Keys)
            {
                n._configuration[configurationKey] = _configuration[configurationKey];
            }

            foreach (var factory in _factories.Keys)
            {
                n._factories[factory] = _factories[factory];
            }
            return n;
        }

        public T GetValue<T>(string key)
        {
            if (_configuration.ContainsKey(key?.ToLower() ?? throw new InvalidOperationException()))
            {
                var v = _configuration[key.ToLower()];

                if (v == null)
                    return default;

                if (v is T t)
                    return t;

                if (Convert.ChangeType(v, typeof(T)) is T t2)
                    return t2;

            }
            return default;
        }
        public string GetValue(string key)
        {
            if (_configuration.ContainsKey(key?.ToLower() ?? throw new InvalidOperationException()))
            {
                var v= _configuration[key.ToLower()];

                if (v == null)
                    return default;

                if (v is string t)
                    return t;

                if (v.ToString() is string t2)
                    return t2;
            }
            return null;
        }
        public T GetFactoryValue<T>(string key)
        {
            if (_factories.ContainsKey(key?.ToLower() ?? throw new InvalidOperationException()))
            {
                if (_factories[key.ToLower()] is Func<object> f)
                {
                    var v = f.Invoke();

                    if (v == null)
                        return default;

                    if (v is T t)
                        return t;

                    if (Convert.ChangeType(v, typeof(T)) is T t2)
                        return t2;
                }
            }
            return default;
        }
        public IEnvironmentConfiguration SetFactoryValue<T>(string key, Func<T>value)
        {
            if (_factories.ContainsKey(key?.ToLower() ?? throw new InvalidOperationException()))
                _factories[key.ToLower()] = ()=>value.Invoke();
            else
                _factories.Add(key.ToLower(), ()=>value.Invoke());

            return this;
        }
        public bool HasValue(string key)
        {
            return _configuration.ContainsKey(key?.ToLower() ?? throw new InvalidOperationException());
        }
        public IEnvironmentConfiguration SetValue<T>(string key, T value)
        {
            if (_configuration.ContainsKey(key?.ToLower() ?? throw new InvalidOperationException()))
                _configuration[key.ToLower()]=value;
            else
                _configuration.Add(key.ToLower(),value);

            return this;
        }
    }
}