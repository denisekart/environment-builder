using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Extensions
{
    public static class XmlFileExtensions
    {
        /// <summary>
        /// Adds the xml file to the configuration. Multiple different files can be added.
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="file"></param>
        /// <param name="namespaces">The xml namespaces to load</param>
        /// <param name="eagerLoad"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithXmlFile(this IEnvironmentConfiguration configuration, string file,
            IDictionary<string, string> namespaces = null, bool eagerLoad = false)
        {

            XmlFileParser parser = null;
            if (configuration.HasValue(typeof(XmlFileParser).FullName))
                parser = configuration.GetValue<XmlFileParser>(typeof(XmlFileParser).FullName);
            else
                configuration.SetValue(typeof(XmlFileParser).FullName, parser = new XmlFileParser());
            if (parser.Files.Keys.Any(x => String.Equals(x, file, StringComparison.OrdinalIgnoreCase)))
                return configuration;
            else
            {
                var data = new XmlFileParser.FileDataHolder();
                if (eagerLoad)
                    try
                    {
                        data.ParsedFile = XDocument.Parse(File.ReadAllText(file));
                    }
                    catch
                    {
                        data.ParsingFailed = true;
                    }

                data.Path = file;
                if(namespaces != null && namespaces.Any())
                    foreach (var keyValuePair in namespaces)
                    {
                        data.NamespaceManager.AddNamespace(keyValuePair.Key,keyValuePair.Value);
                    }

                parser.Files.Add(file, data);
            }

            return configuration;
        }
        /// <summary>
        /// Adds the xml file to the pipe
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="xPath"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithXml(this IEnvironmentBuilder builder, string xPath, string file=null)
        {
            return builder.WithSource(cfg =>
            {
                if (!cfg.HasValue(typeof(XmlFileParser).FullName))
                    return null;//no json file parser was registered
                var reqType = cfg.GetBuildType();
                var parser = cfg.GetValue<XmlFileParser>(typeof(XmlFileParser).FullName);
                return parser.Value(xPath,reqType, file);
            });
        }
        /// <summary>
        /// This is a shorthand for the <see cref="WithXml"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="xPath"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Xml(this IEnvironmentBuilder builder, string xPath, string file = null)
        {
            return builder.WithXml(xPath, file);
        }
    }
}