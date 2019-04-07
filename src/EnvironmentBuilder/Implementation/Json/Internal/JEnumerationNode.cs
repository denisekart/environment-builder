using System.Collections.Generic;
using System.Linq;

namespace EnvironmentBuilder.Implementation.Json
{
    internal class JEnumerationNode : JNode
    {
        public object this[int key]
        {
            get => (Value as IEnumerable<JNode>)?.ElementAtOrDefault(key);
        }
        public JEnumerationNode(IEnumerable<JNode> nodes):base(NodeType.Enumeration,nodes)
        {
            
        }

        //public IEnumerable<T> OfType<T>()
        //{
        //    return (Value as IEnumerable<JNode>).Select(x=>x.)
        //}
    }
}