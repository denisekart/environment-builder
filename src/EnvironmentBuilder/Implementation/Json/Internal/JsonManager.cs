using System;
using System.Collections;

namespace EnvironmentBuilder.Implementation.Json
{
    internal class JsonManager
    {
        public JsonManager()
        {
            
        }

        public static JNode Parse(string value)
        {
            var buffer=new StringBuffer(value);
            return JNode.ParseNode(buffer);
            
        }

        public static T Parse<T>(string value) where T : class
        {
            var node = Parse(value);
            return Parse<T>(node);
        }

        public static T Parse<T>(JNode value)
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)) && 
                value is JEnumerationNode enu)
            {
                //return ParseEnumerableContract<T>(value);
                var arg0 = JUtils.GetGenericTypeFromEnumerable<T>();
                var method = JUtils.GetCastMethodForInputType<JEnumerationNode>();
                var generic = method.MakeGenericMethod(arg0);
                var result = generic.Invoke(null, new[] {enu});
            }
            else if(value is JValueNode jvn)
            {
                //return ParseObjectContract<T>(value);
                return jvn.Cast<T>();
            }
            else if(value is JContentNode jcn)
            {
                return jcn.Cast<T>();
            }
            else
            {
                JUtils.ValidateOutOfBounds();
            }
            throw new NotImplementedException("This should not be hit. I'm responsible if it is.");
        }

        public static JNode Find(string expression, string json)
        {
            var node = Parse(json);
            return JSegment.Expand(expression, node);
        }
        public static JNode Find(string expression, JNode json)
        {
            return JSegment.Expand(expression, json);
        }

    }
}
