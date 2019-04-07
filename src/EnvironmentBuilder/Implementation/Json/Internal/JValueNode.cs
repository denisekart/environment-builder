namespace EnvironmentBuilder.Implementation.Json
{
    internal class JValueNode : JNode
    {
        public JValueNode(object value): base(NodeType.Leaf,value)
        {
            
        }

    }
}