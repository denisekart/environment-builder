using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EnvironmentBuilder.Extensions
{
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
            return !string.IsNullOrEmpty(arg?.Trim()) && ((arg.StartsWith("--") || arg.StartsWith("-")) && arg.Length - arg.TrimStart('-').Length > 1);
        }
        private static string NormalizeKey(string key)
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
                            if (value?.StartsWith("\"") ?? false)
                                value = value.Substring(1);
                            if (value?.EndsWith("\"") ?? false)
                                value = value.Substring(0, value.Length - 1);
                            //value = value?.TrimStart('"');
                            //value = value?.TrimEnd('"');
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

                if (_floats.Any())
                {
                    foreach (var f in _floats)
                    {
                        if(KeyEqualsValueParsible(f,out var k,out var v))
                            _pairs.Add(new Tuple<string, string>(k,v));
                        else 
                            _pairs.Add(new Tuple<string, string>(NormalizeKey(f),null));
                    }
                }
            }
            finally
            {
                _arranged = true;
            }
        }

        private static bool KeyEqualsValueParsible(string raw, out string key, out string value)
        {
            key = null;
            value = null;

            if (raw.Contains("="))
            {
                //key=value
                var parts = raw.Split('=');
                key = NormalizeKey(parts[0]);
                if (parts.Length > 1)
                {
                    value = string.Join("=", parts.Skip(1).ToArray());

                }
                if (value?.StartsWith("\"") ?? false)
                    value = value.Substring(1);
                if (value?.EndsWith("\"") ?? false)
                    value = value.Substring(0, value.Length - 1);
                return true;
            }

            return false;
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
}