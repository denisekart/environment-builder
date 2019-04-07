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
    //https://goessner.net/articles/JsonPath/index.html#e2
    internal class JSegment
    {
        private readonly SegmentType _type;
        private readonly Func<JNode, JNode> _expression;
        private readonly object _referenceValue;

        public enum SegmentType
        {
            Root,
            Wildcard,
            Indexer,
            Path,
            RecursiveDescent
        }

        public JSegment(SegmentType type, Func<JNode,JNode> expression, object referenceValue=null)
        {
            _type = type;
            _expression = expression;
            _referenceValue = referenceValue;
        }

        public static JNode Expand(string expression, JNode json)
        {
            var segments = ParsePath(expression);
            JNode value = json;
            foreach (var jSegment in segments)
            {
                value = jSegment._expression.Invoke(value);
            }

            return value;
        }

        public static IEnumerable<JSegment> ParsePath(string path)
        {
            var buffer=new StringBuffer(path);
            return ParsePath(buffer);
        }
        public static IEnumerable<JSegment> ParsePath(ITokenBuffer path)
        {
            var segments=new List<JSegment>();
            var next = path.MoveNextNonEmptyChar();
            if (next.HasValue && next.Value == JConstants.SegmentRootToken)
            {
                segments.Add(new JSegment(SegmentType.Root,ExpandRoot));
                next = path.MoveNext();
                if (!next.HasValue)
                    return segments;
                JUtils.ValidateChar(next,JConstants.SegmentSeparatorToken);
            }
            bool hasMoreSegments = true;
            while (hasMoreSegments)
            {
                var segment = path.MoveNext((s,i,c)=>
                    i==s.Length-1||s[i+1]==JConstants.SegmentSeparatorToken);
                var processedSegments = ProcessRawSegment(segment);
                segments.AddRange(processedSegments);
                next = path.MoveNext();
                if (!next.HasValue)
                    hasMoreSegments = false;
                else
                    JUtils.ValidateChar(next,JConstants.SegmentSeparatorToken);
            }

            return segments; 
        }

        private static IEnumerable<JSegment> ProcessRawSegment(string segment)
        {
            var processed=new List<JSegment>();
            var singleNodeType = JSegment.SegmentType.Path;
            
            if (segment.StartsWith("."))
            {//recursive descent .. -but the first . was already parsed

                segment = segment.Substring(1);
                singleNodeType = JSegment.SegmentType.RecursiveDescent;
            }

            var i = 0;
            while (i<segment.Length)
            {
                if (segment[i] == JConstants.SegmentIndexerStart)
                    break;
                i++;
            }

            var path = segment.Substring(0, i);
            Func<JNode, JNode> expr = n => ExpandValue(n, path);
            Func<JNode, JNode> expr2 = n => ExpandValueRecursive(n, path);
            Func<JNode, JNode> expr3 = n => ExpandWildcard(n);
            processed.Add(
                new JSegment(singleNodeType,
                    singleNodeType==SegmentType.RecursiveDescent
                        ?expr2
                        :singleNodeType==SegmentType.Wildcard
                            ?expr3
                            :expr,
                    path));

            if (i < segment.Length)
            {
                var indexer = segment.Substring(i, segment.Length - i);
                if (indexer[0] != JConstants.SegmentIndexerStart &&
                    indexer[indexer.Length - 1] != JConstants.SegmentIndexerEnd) 
                    JUtils.ValidateOutOfBounds("Expected an indexer but got an incomplete token.");
                indexer = indexer.Substring(1, indexer.Length - 2);
                processed.Add(
                    new JSegment(
                        SegmentType.Indexer,
                        n=>ExpandIndexer(n,indexer),
                        indexer));
            }

            return processed;
        }

        private static JNode ExpandIndexer(JNode node, string indexer)
        {
            if (node is JEnumerationNode jen && jen.Value is IEnumerable<JNode> itms)
            {
                if (int.TryParse(indexer, out var idx))
                    return jen[idx] as JNode;
                
                if (indexer.Contains(":"))
                {
                    int? start=null, end=null;
                    var parts = indexer.Split(':');
                    if (parts.Length !=2)
                        JUtils.ValidateOutOfBounds("Range indexer is incorrectly formed.");
                    if (int.TryParse(parts[0], out var s))
                        start = s;
                    if (int.TryParse(parts[1], out var e))
                        end = e;
                    if (!start.HasValue) start = 0;
                    if (!end.HasValue || end.Value == -1) end = itms.Count() - 1;
                    List<JNode> subrange = new List<JNode>();
                    for (int i = start.Value; i < end; i++)
                        subrange.Add(itms.ElementAt(i));
                    return new JEnumerationNode(subrange);
                }
                else if (indexer.Contains(","))
                {
                    var values = indexer.Split(',').Select(int.Parse);
                    List<JNode> subrange = new List<JNode>();
                    foreach (var value in values)
                        subrange.Add(itms.ElementAt(value));
                    return new JEnumerationNode(subrange);
                }
            }
            JUtils.ValidateOutOfBounds("Cannot expand an indexer. Path cannot be indexed.");
            throw new NotImplementedException("This exception should never be hit");

        }
        private static JNode ExpandValueRecursive(JNode node, string name)
        {
            if(node.Type==JNode.NodeType.Leaf)
                JUtils.ValidateOutOfBounds("Cannot expand a node. Path does not exist.");
            if(node.Type==JNode.NodeType.Enumeration)
                JUtils.ValidateOutOfBounds("Cannot expand a node. Path does not exist.");
            var combined = ExpandNodesRecursive(node as JContentNode, name);
            return new JEnumerationNode(combined);
        }

        private static IEnumerable<JNode> ExpandNodesRecursive(JContentNode node, string name)
        {
            if (node.Value is IDictionary<string, JNode> dct)
            {
                if(dct.ContainsKey(name))
                    yield return dct[name];
                else
                {
                    foreach (var jNode in dct.Values)
                    {
                        if (jNode is JContentNode jcn)
                        {
                            foreach (var child in ExpandNodesRecursive(jcn, name))
                            {
                                yield return child;
                            }
                        }
                    }
                }
            }
        }

        private static JNode ExpandValue(JNode node, string name)
        {
            if (node is JContentNode jcn)
                return jcn[name] as JNode;
            JUtils.ValidateOutOfBounds("Cannot expand a node. Path does not exist.");
            throw new NotImplementedException("This exception should never be hit");
        }

        private static JNode ExpandWildcard(JNode node)
        {
            if (node is JEnumerationNode jen)
                return jen;
            if (node is JValueNode jvn)
                return jvn;
            if(node is JContentNode jcn)
                return new JEnumerationNode((jcn.Value as IDictionary<string, JNode>)?.Select(x => x.Value));
            JUtils.ValidateOutOfBounds("Cannot expand a wildcard node. Path does not conform.");
            throw new NotImplementedException("This exception should never be hit");

        }
        private static JNode ExpandRoot(JNode node)
        {
            return node ?? null;
        }
    }
}