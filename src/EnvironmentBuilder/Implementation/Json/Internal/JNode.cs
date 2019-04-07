using System;
using System.Collections.Generic;
using System.Linq;

namespace EnvironmentBuilder.Implementation.Json
{
    internal abstract class JNode
    {
        private readonly NodeType _type;
        //private string _key;

        public enum NodeType
        {
            Unknown,
            Leaf,
            Node,
            Enumeration
        }

        public JNode(NodeType type,object value)
        {
            _type = type;
            Value = value;
        }

        //public string Key
        //{
        //    get => _key;
        //    private set => _key = value;
        //}

        private IDictionary<string, JNode> _nodeValue;
        private IEnumerable<JNode> _enumerationValue;
        private object _value;

        public NodeType Type => _type;
        public virtual object Value
        {
            get
            {
                switch (_type)
                {
                    case NodeType.Leaf:
                        return _value;
                        break;
                    case NodeType.Node:
                        return _nodeValue;
                        break;
                    case NodeType.Enumeration:
                        return _enumerationValue;
                        break;
                    case NodeType.Unknown:
                    default:
                        throw new ArgumentException();
                }
            }
            private set
            {
                switch (_type)
                {
                    case NodeType.Leaf:
                        _value = value;
                        break;
                    case NodeType.Node:
                        _nodeValue=value as IDictionary<string,JNode>??new Dictionary<string,JNode>();
                        break;
                    case NodeType.Enumeration:
                        _enumerationValue=value as IEnumerable<JNode>??new List<JNode>();
                        break;
                    case NodeType.Unknown:
                    default:
                        throw new ArgumentException();
                }
            }
        }

        public static JContentNode ParseNode(StringBuffer buffer)
        {

            var next = buffer.MoveNextNonEmptyChar();
            JUtils.ValidateChar(next, JConstants.NodeOpeningToken);

            Dictionary<string, JNode> node=new Dictionary<string, JNode>();
            bool hasMoreNodes = true;

            while (hasMoreNodes)
            {
                var key = ParseNodeKey(buffer);
                JUtils.ValidateChar(buffer.MoveNextNonEmptyChar(), JConstants.KeyValueSeparationToken);
                var value = ParseNodeValue(buffer);
                //value.Key = key;
                node.Add(key,value);
                next = buffer.MoveNextNonEmptyChar();
                if (next == JConstants.NodeClosingToken)
                    hasMoreNodes = false;
                else
                    JUtils.ValidateChar(next,JConstants.SeparatorToken);
            }

            return new JContentNode(node);
        }
        public static JNode ParseNodeValue(StringBuffer buffer)
        {
            var next = buffer.MoveNextNonEmptyChar();
            JUtils.ValidateChar(next);
            buffer.MovePrev();
            // ReSharper disable once PossibleInvalidOperationException - see ValidateChar
            switch (next.Value)
            {
                case JConstants.NodeOpeningToken://parsing object
                    return ParseNode(buffer);
                case JConstants.NodeClosingToken://parsing an empty node
                    //i came here because the node that triggered just ended without info
                    return new JContentNode(null);
                case JConstants.EnumerationOpeningToken://parsing array
                    return ParseEnumerationValue(buffer);
                    break;
                case JConstants.EnumerationClosingToken://todo - i need it here
                    return new JEnumerationNode(null);
                    break;
                case JConstants.KeyValueOpeningClosingToken://parsing string end value
                    buffer.MoveNext();
                    var val = buffer.MoveNext((s, i, c) => s[i+1]==JConstants.KeyValueOpeningClosingToken);
                    JUtils.ValidateChar(buffer.MoveNext(),JConstants.KeyValueOpeningClosingToken);
                    return new JValueNode(val);
                default://parsing boolean,null, number end value
                    return ParseLowLevelNodeValue(buffer, next);
                    break;
            }

            
        }

        private static JNode ParseLowLevelNodeValue(StringBuffer buffer, char? next)
        {
            if (char.IsDigit(next.Value) || next == '+' | next == '-')
            {
                //possible number
                var number = buffer.MoveNext((str, idx, chr) =>
                    idx + 1 < str.Length &&
                    new[]
                        {
                            JConstants.NodeClosingToken,
                            JConstants.SeparatorToken,
                            JConstants.EnumerationClosingToken
                        }
                        .Contains(str[idx + 1]));
                if (number.Contains(".") || number.Contains(","))
                {
                    if (long.TryParse(number, out var l))
                        return new JValueNode(l);
                }
                else
                {
                    if (double.TryParse(number, out var d))
                        return new JValueNode(d);
                }

                JUtils.ValidateOutOfBounds();
            }
            else
            {
                //possible boolean, null
                bool? token = buffer.IsNext("true", StringComparison.CurrentCultureIgnoreCase)
                    ? true
                    : buffer.IsNext("false", StringComparison.CurrentCultureIgnoreCase)
                        ? false
                        : (bool?) null;
                if (!token.HasValue || token.Value) buffer.MoveNext(4);
                else buffer.MoveNext(5);

                if (!token.HasValue)
                    return new JValueNode(null);
                else
                    return new JValueNode(token.Value);
                ;
            }

            throw new NotImplementedException("If you see this, I screwed up.");
        }

        private static JNode ParseEnumerationValue(StringBuffer buffer)
        {
            char? next;
            //array entry point
            buffer.MoveNext();
            List<JNode> entries = new List<JNode>();
            bool hasNextValue = true;
            while (hasNextValue)
            {
                var entry = ParseNodeValue(buffer);
                entries.Add(entry);
                next = buffer.MoveNextNonEmptyChar();
                if (next == JConstants.EnumerationClosingToken)
                    hasNextValue = false;
                else
                    JUtils.ValidateChar(next, JConstants.SeparatorToken);
            }

            return new JEnumerationNode(entries);
        }

        public static string ParseNodeKey(StringBuffer buffer)
        {
            var next = buffer.MoveNextNonEmptyChar();
            JUtils.ValidateChar(next,JConstants.KeyValueOpeningClosingToken);
            var value = buffer.MoveNext(JConstants.KeyValueOpeningClosingToken);
            buffer.MovePrev();
            value = value.Substring(0, value.Length - 1);
            JUtils.ValidateNonEmpty(value);
            next = buffer.MoveNext();
            JUtils.ValidateChar(next,JConstants.KeyValueOpeningClosingToken);
            return value;
        }
    }
}