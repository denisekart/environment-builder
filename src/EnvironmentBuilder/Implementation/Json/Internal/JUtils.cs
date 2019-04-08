using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EnvironmentBuilder.Implementation.Json
{
    internal static class JUtils
    {
        public static void ValidateGeneral(string exceptionMessage)
        {
            throw new JException(exceptionMessage??"Something went wrong in the Json engine");
        }
        public static void ValidateOutOfBounds(string exceptionMessage = null)
        {
            throw new JException(exceptionMessage??"Reached an invalid control block.");
        }
        public static void ValidateChar(char? value, params char[] expected)
        {
            if (!value.HasValue || expected.All(x => x != value.Value))
                throw new JException($"Invalid token. Expected any of '{string.Join(", ",expected.Select(x=>x.ToString()).ToArray())}', got '{(value.HasValue ? value.ToString() : "empty")}'.");
        }

        public static void ValidateChar(char? value, char expected, string exceptionMessage = null)
        {
            if(!value.HasValue || value != expected)
                throw new JException(exceptionMessage??$"Invalid token. Expected '{expected}', got '{(value.HasValue?value.ToString():"empty")}'.");
        }
        public static void ValidateChar(char? value, string exceptionMessage = null)
        {
            if (!value.HasValue)
                throw new JException(exceptionMessage ?? $"Invalid token. Expected a value, got empty.");
        }
        public static void ValidateNonEmpty(string value, string exceptionMessage = null)
        {
            if(string.IsNullOrEmpty(value))
                throw new JException(exceptionMessage??"Expected non empty value.");
        }

        public static void ValidateInterCast<T>()
        {
            if (typeof(JNode).IsAssignableFrom(typeof(T)))
                throw new JException("Casting between node or self types is not allowed");
        }

        public static bool CanConvertToType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            IConvertible convertible = value as IConvertible;

            if (convertible == null)
            {
                return false;
            }

            return true;
        }

        public static bool ConformsToType<T>(object value)
        {
            if (default(T) == null && value == null)
                return true;
            if (value is T)
                return true;
            return CanConvertToType(value, typeof(T));
        }

        public static Type GetGenericTypeFromEnumerable<T>()
        {
            var o = typeof(T);
            if (o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return o.GetGenericArguments()[0];
            }
            return null;
        }

        private static IDictionary<Type, MethodInfo> _methodCache=new Dictionary<Type, MethodInfo>
        {
            {typeof(JValueNode),typeof(JUtils)
                .GetMethods().FirstOrDefault(x=>x.Name==nameof(Cast) && x.IsPublic && x.IsStatic &&
                                                x.GetParameters().Any(z=>z.ParameterType==typeof(JValueNode))) },
            {typeof(JEnumerationNode),typeof(JUtils)
                .GetMethods().FirstOrDefault(x=>x.Name==nameof(Cast) && x.IsPublic && x.IsStatic &&
                                                x.GetParameters().Any(z=>z.ParameterType==typeof(JEnumerationNode))) },
            {typeof(JContentNode),typeof(JUtils)
                .GetMethods().FirstOrDefault(x=>x.Name==nameof(Cast) && x.IsPublic && x.IsStatic &&
                                                x.GetParameters().Any(z=>z.ParameterType==typeof(JContentNode))) },
        };
        public static MethodInfo GetCastMethodForInputType<T>()
        {
            if (!_methodCache.ContainsKey(typeof(T)))
                return null;
            return _methodCache[typeof(T)];
        }
        public static MethodInfo GetCastMethodForInputType(Type t)
        {
            if (!_methodCache.ContainsKey(t))
                return null;
            return _methodCache[t];
        }


        public static T Cast<T>(this JContentNode node)
        {
            //case sensitive as per spec
            var keys = ((IDictionary<string, JNode>)node.Value).Keys;
            var t = typeof(T);
            var obj = Activator.CreateInstance<T>();
            foreach (var propertyInfo in
                t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => keys.Contains(x.Name)))
            {
                var nodex = node[propertyInfo.Name] as JNode;
                var targetType = propertyInfo.PropertyType;
                MethodInfo method = null;
                switch (nodex.Type)
                {
                    case JNode.NodeType.Leaf:
                        method = GetCastMethodForInputType<JValueNode>();
                        break;
                    case JNode.NodeType.Node:
                        method = GetCastMethodForInputType<JContentNode>();
                        break;
                    case JNode.NodeType.Enumeration:
                        method = GetCastMethodForInputType<JEnumerationNode>();
                        break;
                }

                var genericMethod = method.MakeGenericMethod(targetType);
                var result = genericMethod.Invoke(null, new object[] { nodex });
                propertyInfo.SetValue(obj, result, null);
            }

            return (T)obj;
        }
        public static T Cast<T>(this JValueNode node)
        {
            ValidateInterCast<T>();
            var actualValue = node.Value;

            if (actualValue is T t)
                return t;

            if (typeof(T).IsPrimitive)
            {
                if (actualValue == null && Nullable.GetUnderlyingType(typeof(T)) != null)
                    return default;
                else
                    return (T)Convert.ChangeType(actualValue, typeof(T));
            }else if (typeof(T) == typeof(string))
            {
                return actualValue?.ToString() is T tx?(T)tx:default;
            }else if (typeof(T) == typeof(DateTime))
            {
                if (actualValue == null && Nullable.GetUnderlyingType(typeof(T)) != null)
                    return default;
                return DateTime.TryParse(actualValue?.ToString(), out var r) ? (T)(object)r : default;
            }
            ValidateOutOfBounds($"Could not parse the value of type {typeof(T)}");
            throw new NotImplementedException("This should never be hit. My mistake if it is.");
        }
        public static IEnumerable<T> Cast<T>(this JEnumerationNode node)
        {
            var list = node.Value as IEnumerable<JNode>;
            if (list == null || !list.Any())
                return Enumerable.Empty<T>();
            if(!list.All(x=>ConformsToType<T>(x.Value)))
                ValidateGeneral("Cannot cast to enumeration of type {typeof(T)}. All of the items may not conform to the type.");
            return list.Select(x =>
            {
                if (default(T) == null && x == null)
                    return default;
                if (x is T a)
                    return a;
                return (T)Convert.ChangeType(x.Value, typeof(T));
            });
        }


    }
}