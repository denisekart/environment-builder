using System.Collections.Generic;

namespace EnvironmentBuilder.Implementation.Json
{
    internal class JContentNode : JNode
    {
        public object this[string key]
        {
            get => (Value as IDictionary<string, JNode>)?[key];
        }
        public JContentNode(IDictionary<string,JNode> node) : base(NodeType.Node,node)
        {
            
        }
    }
}