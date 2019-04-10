using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EnvironmentBuilder.Implementation
{
    internal class EnvironmentVariableParser
    {
        private string NormalizeKey(string key)
        {
            return key?.Trim();
        }
        public object Value(string name, Type type, EnvironmentVariableTarget target, string prefix)
        {
            if (string.IsNullOrEmpty(name?.Trim()))
                return null;
            if (type == null)
                type = typeof(string);
            var key = NormalizeKey($"{prefix ?? string.Empty}{name}");

            var value = Environment.GetEnvironmentVariable(key, target);
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