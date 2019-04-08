using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using EnvironmentBuilder.Implementation.Json;

namespace EnvironmentBuilder.Extensions
{
    internal class XmlFileParser
    {
        public class FileDataHolder
        {
            public bool ParsingFailed { get; set; }
            public XDocument ParsedFile { get; set; }
            public string Path { get; set; }
            public XmlNamespaceManager NamespaceManager { get; set; } = new XmlNamespaceManager(new NameTable());
        }
        public IDictionary<string, FileDataHolder> Files { get; set; } = new Dictionary<string, FileDataHolder>();

        public object Value(string xPath, Type type, string file=null)
        {
            if (FindMatchingFile(file) is XmlFileParser.FileDataHolder data)
            {
                if (data.ParsingFailed)
                    return null;
                if (data.ParsedFile == null)
                {
                    try
                    {
                        var doc = XDocument.Parse(File.ReadAllText(data.Path));
                        data.ParsedFile = doc;
                    }
                    catch/*(Exception e)*/
                    {
                        data.ParsingFailed = true;
                    }
                }
                if (data.ParsingFailed)
                    return null;
                try
                {
                    if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
                    {
                        var selection=((data.ParsedFile) ?? throw new InvalidOperationException())
                            .XPathSelectElements(xPath,data.NamespaceManager);
                        var genericType = JUtils.GetGenericTypeFromEnumerable(type);
                        var untypedSelection = selection.Select(x => x.Value);
                        var enumerable = untypedSelection as string[] ?? untypedSelection.ToArray();
                        if (enumerable.All(x => JUtils.CanConvertToType(x, genericType)))
                        {
                            return enumerable.Select(x => JUtils.ConvertToType(x, genericType));
                        }
                    }
                    else
                    {
                        var selection = ((data.ParsedFile) ?? throw new InvalidOperationException())
                            .XPathSelectElement(xPath, data.NamespaceManager);
                        if (JUtils.CanConvertToType(selection.Value, type))
                            return JUtils.ConvertToType(selection.Value, type);
                    }
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
            if ((string.IsNullOrEmpty(expression) && Files.Count == 1) || Files.Count == 1)
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