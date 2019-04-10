using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EnvironmentBuilder.Implementation.Json;

namespace EnvironmentBuilder.Implementation
{
    internal class JsonFileParser
    {
        public class FileDataHolder
        {
            public bool ParsingFailed { get; set; }
            public JNode ParsedFile { get; set; }
            public string Path { get; set; }
        }
        public IDictionary<string,FileDataHolder> Files { get; set; }=new Dictionary<string, FileDataHolder>();

        public object Value(IEnumerable<JSegment> path, Type type)
        {
            if (path.FirstOrDefault() is JSegment rootSegment && rootSegment.Type == JSegment.SegmentType.Root &&
                FindMatchingFile(rootSegment.ReferenceValue?.ToString()) is JsonFileParser.FileDataHolder
                    data)
            {
                if (data.ParsingFailed)
                    return null;
                if (data.ParsedFile == null)
                {
                    try
                    {
                        data.ParsedFile = JsonManager.Parse(File.ReadAllText(data.Path));
                    }
                    catch
                    {
                        data.ParsingFailed = true;
                    }
                }
                if (data.ParsingFailed)
                    return null;
                try
                {
                    var method = typeof(JsonManager).GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(x => x.Name == "Find" && x.IsGenericMethod &&
                                             x.GetParameters()
                                                 .Select(y => y.ParameterType)
                                                 .All(z=>z == typeof(JNode) || z== typeof(IEnumerable<JSegment>)));
                    var generic = method.MakeGenericMethod(type);
                    var result = generic.Invoke(null, new object[] {path, data.ParsedFile});
                    return result;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public FileDataHolder FindMatchingFile(string expression)
        {
            if (Files.Count == 0)
                return null;
            if ((string.IsNullOrEmpty(expression) && Files.Count == 1) || Files.Count==1)
            {
                return Files.Values.First();
            }

            try
            {

                var candidates = Files.Keys.Where(x => x.Equals(expression, StringComparison.OrdinalIgnoreCase) ||
                                                       Path.GetFileName(x).Equals(Path.GetFileName(expression),
                                                           StringComparison.OrdinalIgnoreCase) ||
                                                       Path.GetFileNameWithoutExtension(x)
                                                           .Equals(Path.GetFileNameWithoutExtension(expression),
                                                               StringComparison.OrdinalIgnoreCase));
                var singleKey = candidates.SingleOrDefault();
                if (singleKey != null)
                    return Files[singleKey];
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}