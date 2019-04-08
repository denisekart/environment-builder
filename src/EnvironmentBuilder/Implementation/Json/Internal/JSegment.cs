using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EnvironmentBuilder.Implementation.Json
{

    //https://goessner.net/articles/JsonPath/index.html#e2
    internal class JSegment
    {
        private readonly SegmentType _type;
        private readonly Func<JNode, JNode> _expression;
        private readonly object _referenceValue;

        public SegmentType Type => _type;
        public object ReferenceValue => _referenceValue;
        public override string ToString()
        {
            return $"{_type}({_referenceValue})";
        }

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
            var segments = Parse(expression);
            return Expand(segments, json);
        }

        public static JNode Expand(IEnumerable<JSegment> segments, JNode json)
        {
            JNode value = json;
            foreach (var jSegment in segments)
            {
                value = jSegment._expression.Invoke(value);
            }

            return value;
        }

        public static IEnumerable<JSegment> Parse(string path)
        {
            var buffer=new StringBuffer(path);
            return Parse(buffer);
        }
        public static IEnumerable<JSegment> Parse(ITokenBuffer path)
        {
            var segments=new List<JSegment>();
            var next = path.MoveNextNonEmptyChar();
            if (next.HasValue && next.Value == JConstants.SegmentRootToken)
            {
                var root = new JSegment(SegmentType.Root, ExpandRoot);
                next = path.MoveNextNonEmptyChar();
                if (!next.HasValue)
                {
                    segments.Add(root);
                    return segments;
                }

                if (next == JConstants.ExpressionStartToken)
                {//i contain an expression
                    var expr = path.MoveNext((s, i, c) => s[i + 1] == JConstants.ExpressionEndToken);
                    if (!string.IsNullOrEmpty(expr))
                    {
                        root=new JSegment(SegmentType.Root,ExpandRoot,expr);
                    }
                    JUtils.ValidateChar(path.MoveNext(),JConstants.ExpressionEndToken);
                    next = path.MoveNextNonEmptyChar();
                }
                segments.Add(root);
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
                {
                    JUtils.ValidateChar(next, JConstants.SegmentSeparatorToken);
                }
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
            if (path == JConstants.WildcardSegmentToken.ToString())
                singleNodeType = SegmentType.Wildcard;

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
                processed.AddRange(ProcessRawIndexer(indexer));
            }

            return processed;
        }

        private static IEnumerable<JSegment> ProcessRawIndexer(string segment)
        {
            //assume starts with [ and ends with ] but may include extra segmentation ']['
            //multiple indexers may exist in a row
            var buffer=new StringBuffer(segment);
            var next = buffer.MoveNextNonEmptyChar();
            JUtils.ValidateChar(next,JConstants.SegmentIndexerStart);

            List<JSegment> indexers=new List<JSegment>();
            bool hasMoreSegments = true;
            while (hasMoreSegments)
            {
                var segmentValue = buffer.MoveNext((s, i, c) => s[i + 1] == JConstants.SegmentIndexerEnd);
                next = buffer.MoveNext();
                indexers.Add(new JSegment(JSegment.SegmentType.Indexer,n=>ExpandIndexer(n,segmentValue),segmentValue));
                next = buffer.MoveNextNonEmptyChar();
                if (!next.HasValue)
                    hasMoreSegments = false;
                else
                {
                    JUtils.ValidateChar(next, JConstants.SegmentIndexerStart, "Expected an indexer but got an incomplete token.");
                }
            }

            return indexers;
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
            throw new NotImplementedException("This feature is not implemented yet.");
            //TODO
            //all of the child nodes should be expanded into a single content node recusively
            //all of the immediate child nodes shoul be expanded into an enumeration

#pragma warning disable 162
            if (node is JEnumerationNode jen)
                return jen;
            if (node is JValueNode jvn)
                return jvn;
            if(node is JContentNode jcn)
                return new JEnumerationNode((jcn.Value as IDictionary<string, JNode>)?.Select(x => x.Value));
            JUtils.ValidateOutOfBounds("Cannot expand a wildcard node. Path does not conform.");
            throw new NotImplementedException("This exception should never be hit");
#pragma warning restore 162

        }
        private static JNode ExpandRoot(JNode node)
        {
            return node ?? null;
        }
    }
}