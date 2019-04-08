using System;
using System.IO;
using System.Linq;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Implementation.Json;

namespace EnvironmentBuilder.Extensions
{
    public static class JsonFileExtensions
    {
        /// <summary>
        /// Adds the json file to the configuration. Multiple different files can be added.
        /// Use root expression syntax for file selection <example>$(filename).some.path</example>
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="file"></param>
        /// <param name="eagerLoad"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithJsonFile(this IEnvironmentConfiguration configuration, string file, bool eagerLoad=false)
        {
            JsonFileParser parser = null;
            if (configuration.HasValue(typeof(JsonFileParser).FullName))
                parser = configuration.GetValue<JsonFileParser>(typeof(JsonFileParser).FullName);
            else
                configuration.SetValue(typeof(JsonFileParser).FullName,parser = new JsonFileParser());
            if (parser.Files.Keys.Any(x => String.Equals(x, file, StringComparison.OrdinalIgnoreCase)))
                return configuration;
            else
            {
                var data=new JsonFileParser.FileDataHolder();
                if(eagerLoad)
                    try
                    {
                        data.ParsedFile = JsonManager.Parse(File.ReadAllText(file));
                    }
                    catch
                    {
                        data.ParsingFailed = true;
                    }

                data.Path = file;
                parser.Files.Add(file,data);
            }

            return configuration;
        }
        /// <summary>
        /// Adds the json file to the pipeline
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="jPath"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithJson(this IEnvironmentBuilder builder, string jPath)
        {
            //next line wil trigger an exception on malformed syntax
            var expr = JSegment.Parse(jPath).ToList();
            return builder.WithSource(cfg =>
            {
                if (!cfg.HasValue(typeof(JsonFileParser).FullName))
                    return null;//no json file parser was registered
                var reqType = cfg.GetBuildType();
                var parser = cfg.GetValue<JsonFileParser>(typeof(JsonFileParser).FullName);
                return parser.Value(expr, reqType);

            });
        }
        /// <summary>
        /// This is a shorthand for the <see cref="WithJson"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="jPath"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Json(this IEnvironmentBuilder builder, string jPath)
        {
            return builder.WithJson(jPath);
        }
    }
}