using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Sets the type for the T in the configuration.
        /// This may be accessed with the GetBuildType or with the value of
        /// Abstractions.Constants.SourceRequiredTypeKey
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        public static void SetBuildType<T>(this IEnvironmentConfiguration configuration)
        {
            configuration.SetValue(Constants.SourceRequiredTypeKey, typeof(T));
        }
        /// <summary>
        /// Gets the value of the current type of value requested from the build method
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Type GetBuildType(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.GetValue<Type>(Constants.SourceRequiredTypeKey);
        }

        /// <summary>
        /// Sets the description value. Can be used on a source or an environment builder
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration Description(this IEnvironmentConfiguration configuration, string value)
        {
            //if (configuration.HasValue(Constants.SourceDescriptionValueKey)
            //   && configuration.GetValue(Constants.SourceDescriptionValueKey) is string v)
            //    configuration.SetValue(Constants.SourceDescriptionValueKey, v + value);
            //else
                configuration.SetValue(Constants.SourceDescriptionValueKey, value);
            return configuration;
        }
        /// <summary>
        /// Gets the description from the configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string Description(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.GetValue(Constants.SourceDescriptionValueKey);
        }
        /// <summary>
        /// Gets the description from the environment builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static string Description(this IEnvironmentBuilder builder)
        {
            return builder.Configuration.Description();
        }
        /// <summary>
        /// Prints out the help including the descriptions and the traces
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static string Help(this IEnvironmentBuilder builder)
        {
            return $@"{builder.Configuration.Description()}
{string.Join("-", Enumerable.Repeat("-", 15).ToArray())}
{string.Join(Environment.NewLine,builder.Bundles.Select(x=>string.Join(" >> ",x.Sources.Select(y=>y.Trace()??"??").ToArray())).ToArray())}";
        }

        /// <summary>
        /// Adds a trace to the current value
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration Trace(this IEnvironmentConfiguration configuration, string value)
        {
            return configuration.SetValue(Constants.SourceTraceValueKey,value);
        }
        /// <summary>
        /// Gets the trace value
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string Trace(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.GetValue(Constants.SourceTraceValueKey);
        }

        /// <summary>
        /// Adds the default value source to the pipe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithDefaultValue<T>(this IEnvironmentBuilder builder, T value)
        {
            return builder.WithSource(_ => value,cfg=>cfg.Trace($"[default]{value}"));
        }
        /// <summary>
        /// Shorthand alias for <see cref="WithDefaultValue{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Default<T>(this IEnvironmentBuilder builder, T value)
        {
            return builder.WithDefaultValue(value);
        }
        /// <summary>
        /// Adds the throwable source to the pipe
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithException(this IEnvironmentBuilder builder, string message)
        {
            return builder.WithSource<object>(_ => throw new ArgumentException(message??"Missing required variable"),cfg=>cfg.Trace($"[exception]"));
        }
        /// <summary>
        /// Shorthand alias for <see cref="WithException"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Throw(this IEnvironmentBuilder builder, string message)
        {
            return builder.WithException(message);
        }
    }
    public static class CommandLineArgumentExtensions
    {
        /// <summary>
        /// Adds the command line argument source to the pipe
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithArgument(this IEnvironmentBuilder builder,string name)
        {
            return builder.WithSource(cfg =>
            {
                var parser = cfg.GetValue<ArgumentParser>(typeof(ArgumentParser).FullName);
                var requiredType = cfg.GetBuildType();
                return parser.Value(name,requiredType);
            },cfg =>
            {
                cfg.Trace($"[argument]{name}");
                if (!cfg.HasValue(typeof(ArgumentParser).FullName))
                {
                    cfg.SetValue(typeof(ArgumentParser).FullName, 
                        new ArgumentParser(Environment.GetCommandLineArgs()));
                }

            });
        }
        /// <summary>
        /// Shorthand alias for <see cref="WithArgument"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Arg(this IEnvironmentBuilder builder, string name)
        {
            return builder.WithArgument(name);
        }

    }


    internal class ArgumentParser
    {
#if NET35
            public class Tuple<T1, T2>
            {
                public T1 Item1 { get; private set; }
                public T2 Item2 { get; private set; }
                internal Tuple(T1 first, T2 second)
                {
                    Item1 = first;
                    Item2 = second;
                }
            }
#endif
        private readonly string[] _args;
        private readonly IList<Tuple<string, string>> _pairs = new List<Tuple<string, string>>();
        private readonly IList<string> _floats = new List<string>();
        private bool _arranged;
        public ArgumentParser(string[] args)
        {
            _args = args ?? new string[0];
            //ArrangeArgumentsOnce();
        }
        /// <summary>
        /// is option only if starts with - or -- .
        ///
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static bool IsOption(string arg)
        {
            return !string.IsNullOrEmpty(arg?.Trim()) && (arg.StartsWith("--") || arg.StartsWith("-") && arg.Length - arg.TrimStart('-').Length > 2);
        }
        private string NormalizeKey(string key)
        {
            return key?.ToLower().TrimStart('-');
        }
        private void ArrangeArgumentsOnce()
        {
            if (_arranged)
                return;
            try
            {
                int index = -1;
                while (++index < _args.Length)
                {
                    var key = _args[index];
                    string value = null;
                    if (!IsOption(key))
                    {
                        _floats.Add(key);
                    }
                    else
                    {
                        if (key.Contains("="))
                        {
                            //key=value
                            var parts = key.Split('=');
                            key = NormalizeKey(parts[0]);
                            if (parts.Length > 1)
                            {
                                value = string.Join("=", parts.Skip(1).ToArray());
                            }
                        }
                        else if (index + 1 < _args.Length &&
                                 !IsOption(_args[index + 1]))
                        {
                            //key value value etc. 
                            key = NormalizeKey(key);
                            while (index + 1 < _args.Length && !IsOption(_args[index + 1]))
                            {
                                index++;
                                if (value == null)
                                    value = _args[index];
                                else
                                    value += $" {_args[index]}";
                            }
                        }
                        else // key(boolean) value=null
                            key = NormalizeKey(key);
                        _pairs.Add(new Tuple<string, string>(key, value));
                    }
                }
            }
            finally
            {
                _arranged = true;
            }
        }

        public object Value(string name, Type type)
        {
            if (string.IsNullOrEmpty(name?.Trim()))
                return null;
            if (type == null)
                type = typeof(string);
            ArrangeArgumentsOnce();
            var key = NormalizeKey(name);

            var tuples = _pairs.Where(x => x.Item1 == key).ToList();
            var tuple = tuples?.LastOrDefault();

            if (typeof(string) == type)
            {
                return ExtractStringValue(tuple);
            }
            else if (typeof(bool) == type)
            {
                return ExtractBooleanValue(tuple);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                return ExtractEnumerableValue(type, tuple, tuples);
            }
            else
            {
                //TODO: support other types
            }

            return null;
        }

        private static object ExtractEnumerableValue(Type type, Tuple<string, string> tuple, List<Tuple<string, string>> tuples)
        {
            var genericTypes = GetGenericIEnumerableTypes(type)?.ToList();
            if (genericTypes?.Any(x => typeof(string) == x) ?? false)
            {
                if (tuple == null)
                    return null;

                return tuples.Select(x => x.Item2).ToArray();
            }
            else
            {
                if (genericTypes == null)
                    throw new ArgumentException("Enumerable does not contain a type to parse");
                else
                    throw new ArgumentException(
                        $"Parser for enumerable of any type [{string.Join(", ", genericTypes.Select(x => x.Name).ToArray())}] is not implemented.");
            }
        }

        private static object ExtractBooleanValue(Tuple<string, string> tuple)
        {
            if (tuple == null)
                return null;
            if (tuple.Item2 == null)
                return true;
            if (bool.TryParse(tuple.Item2, out var t))
                return t;
            if (tuple.Item2.Trim().ToLower() is string v)
            {
                if (new[] { "yes", "1" }.Contains(v))
                    return true;
                if (new[] { "no", "0" }.Contains(v))
                    return false;
            }

            return null;
        }

        private static object ExtractStringValue(Tuple<string, string> tuple)
        {
            if (tuple == null)
                return null;
            if (tuple.Item2 == null)
                return true.ToString();
            return tuple.Item2;
        }

        private static IEnumerable<Type> GetGenericIEnumerableTypes(Type o)
        {
            if (o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return new[]{o.GetGenericArguments()[0]};
            }
            return o
                .GetInterfaces()
                .Where(t => t.IsGenericType
                            && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GetGenericArguments()[0]);
        }
    }

    public static class EnvironmentVariableExtensions
    {
        /// <summary>
        /// Sets the target for the environment variable resolvation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        public static void SetEnvironmmentTarget(this IEnvironmentConfiguration configuration, EnvironmentVariableTarget target)
        {
            configuration.SetValue(Constants.EnvironmentVariableTargetKey, target);
        }
        /// <summary>
        /// Gets the value of the environment variable store used
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static EnvironmentVariableTarget GetEnvironmentTarget(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.HasValue(Constants.EnvironmentVariableTargetKey)
                ?configuration.GetValue<EnvironmentVariableTarget>(Constants.EnvironmentVariableTargetKey) :
            EnvironmentVariableTarget.Process;
        }

        public static IEnvironmentBuilder WithEnvironmentVariable(this IEnvironmentBuilder builder, string name, EnvironmentVariableTarget environmentTarget)
        {
            return builder.WithSource(cfg =>
            {
                var parser = cfg.GetValue<EnvironmentVariableParser>(typeof(EnvironmentVariableParser).FullName);
                var requiredType = cfg.GetBuildType();
                var target = cfg.GetEnvironmentTarget();
                return parser.Value(name, requiredType,target);
            }, cfg =>
            {
                cfg.Trace($"[environment]{name}").SetEnvironmmentTarget(environmentTarget);
                if (!cfg.HasValue(typeof(EnvironmentVariableParser).FullName))
                {
                    cfg.SetValue(typeof(EnvironmentVariableParser).FullName,
                        new EnvironmentVariableParser());
                }

            });
        }

        internal class EnvironmentVariableParser
        {
            private string NormalizeKey(string key)
            {
                return key?.Trim();
            }
            public object Value(string name, Type type,EnvironmentVariableTarget target)
            {
                if (string.IsNullOrEmpty(name?.Trim()))
                    return null;
                if (type == null)
                    type = typeof(string);
                var key = NormalizeKey(name);
                
                var value = Environment.GetEnvironmentVariable(key,target);
                if (typeof(string) == type)
                {
                    return value;
                }
                else if (typeof(bool) == type)
                {
                    return ExtractBooleanValue(value);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return ExtractEnumerableValue(type, value);
                }
                else
                {
                    //TODO: support other types
                }

                return null;
            }

            private object ExtractEnumerableValue(Type type, string value)
            {
                var genericTypes = GetGenericIEnumerableTypes(type)?.ToList();
                if (genericTypes?.Any(x => typeof(string) == x) ?? false)
                {
                    if (value == null)
                        return null;

                    return value.Split(';').ToArray();
                }
                else
                {
                    if (genericTypes == null)
                        throw new ArgumentException("Enumerable does not contain a type to parse");
                    else
                        throw new ArgumentException(
                            $"Parser for enumerable of any type [{string.Join(", ", genericTypes.Select(x => x.Name).ToArray())}] is not implemented.");
                }
            }

            private object ExtractBooleanValue(string value)
            {
                if (value == null)
                    return null;
                
                if (bool.TryParse(value, out var t))
                    return t;
                if (value.Trim().ToLower() is string v)
                {
                    if (new[] { "yes", "1" }.Contains(v))
                        return true;
                    if (new[] { "no", "0" }.Contains(v))
                        return false;
                }

                return null;
            }
            private static IEnumerable<Type> GetGenericIEnumerableTypes(Type o)
            {
                if (o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return new[] { o.GetGenericArguments()[0] };
                }
                return o
                    .GetInterfaces()
                    .Where(t => t.IsGenericType
                                && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(t => t.GetGenericArguments()[0]);
            }
        }
    }

    public static class JsonFileExtensions
    {

    }

    public static class YamlFileExtensions
    {

    }

    public static class MsSqlDatabaseExtensions
    {

    }
}
