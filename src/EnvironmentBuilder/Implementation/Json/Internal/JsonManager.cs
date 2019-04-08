using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EnvironmentBuilder.Implementation.Json
{
    //TODO: Make some Try methods
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
            if (typeof(T) != typeof(string) && typeof(IEnumerable).IsAssignableFrom(typeof(T)) && 
                value is JEnumerationNode enu)
            {
                //return ParseEnumerableContract<T>(value);
                var arg0 = JUtils.GetGenericTypeFromEnumerable<T>();
                var method = JUtils.GetCastMethodForInputType<JEnumerationNode>();
                var generic = method.MakeGenericMethod(arg0);
                var result = generic.Invoke(null, new[] {enu});
                return (T) result;
            }
            else if (value is JEnumerationNode jen)
            {
                var data = jen.Cast<T>();
                if (data == null) data = Enumerable.Empty<T>();
                var enumerable = data as T[] ?? data.ToArray();
                if(enumerable.Length>1)
                    JUtils.ValidateOutOfBounds();

                return enumerable.FirstOrDefault();
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
            return Find(expression, node);
        }
        public static JNode Find(string expression, JNode json)
        {
            return JSegment.Expand(expression, json);
        }
        public static JNode Find(IEnumerable<JSegment> expression, JNode json)
        {
            return JSegment.Expand(expression, json);
        }

        public static T Find<T>(string expression, string json)
        {
            var node = Parse(json);
            return Find<T>(expression, node);
        }
        public static T Find<T>(string expression, JNode json)
        {
            var result = Find(expression, json);
            return Parse<T>(result);
        }
        public static T Find<T>(IEnumerable<JSegment> expression, JNode json)
        {
            var result = Find(expression, json);
            return Parse<T>(result);
        }

    }
}
