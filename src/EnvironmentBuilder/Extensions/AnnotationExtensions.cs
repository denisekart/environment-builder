using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Extensions
{
    public static class AnnotationExtensions
    {
        /// <summary>
        /// Sets the global description. This value will be used to print out the info/help
        /// </summary>
        /// <param name="configuration">the configuration to modify</param>
        /// <param name="description">the description to set</param>
        /// <returns>the configuration</returns>
        public static IEnvironmentConfiguration WithDescription(this IEnvironmentConfiguration configuration, string description)
        {
            return configuration.SetValue(Constants.GlobalDescriptionValueKey,description);
        }
        /// <summary>
        /// Sets the scoped description for the current bundle. This value will be used to print out the info for the current bundle.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithDescription(this IEnvironmentBuilder builder, string description)
        {
            return builder.WithConfiguration(config =>
                config.SetValue(Constants.SourceDescriptionValueKey, description));
        }
        /// <summary>
        /// Gets the global description or null if none exist
        /// </summary>
        /// <param name="builder">the builder</param>
        /// <returns>the description</returns>
        public static string GetDescription(this IEnvironmentBuilder builder)
        {
            return builder.Configuration.GetValue(Constants.GlobalDescriptionValueKey);
        }

        /// <summary>
        /// Gets a list of descriptions for the bundles in the correct order the bundles were added
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetDescriptions(this IEnumerable<IEnvironmentBundle> bundles)
        {
            var raw = bundles.Select(x =>
                    x.Sources.LastOrDefault(y => y.HasValue(Constants.SourceDescriptionValueKey))
                        ?.GetValue(Constants.SourceDescriptionValueKey))
                .ToList();

            string currentDescription = null;
            int currentDescriptionOwner = -1;
            for (int i = 0; i < raw.Count; i++)
            {
                if ( currentDescription != null && currentDescription.Equals(raw[i]) && currentDescriptionOwner<i)
                {
                    raw[i] = null;
                }
                else
                {
                    currentDescription = raw[i];
                    currentDescriptionOwner = i;
                }
            }

            return raw;
        }

        /// <summary>
        /// Gets the formatted string containing the descriptions of the bundles and the utility.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>the formatted help string</returns>
        public static string GetHelp(this IEnvironmentBuilder builder)
        {
            var help = $"{{0}}{Environment.NewLine}{Environment.NewLine}{{1}}";
            var parameter0 = builder.GetDescription();
            bool parameter0valid = !string.IsNullOrEmpty(parameter0?.Trim());

            string tabFormat = "\t";
            var descriptions = builder.Bundles.GetDescriptions().ToArray();
            var traces = builder.Bundles.Select(x => x.Sources.Select(y => y.GetTrace()).ToArray()).ToArray();
            var range = Math.Max(descriptions.Length, traces.Length);
            var combined = Enumerable.Range(0, range)
                .Select(x =>
                {
                    StringBuilder sb=new StringBuilder();
                    if (x < traces.Length)
                        sb.AppendLine($"- {(string.Join(", ",traces[x]))}");
                    else
                        sb.AppendLine($"- [unknown]");


                    if (x < descriptions.Length)
                        sb.AppendLine(tabFormat+descriptions[x]);
                    else
                        sb.AppendLine();

                    return sb.ToString();
                }).ToArray();
            var parameter1=string.Join(string.Empty,combined);
            //var parameter1 = string.Join(Environment.NewLine ,
            //    builder.Bundles.GetDescriptions()
            //        .Select(x=>$"{(parameter0valid?tabFormat:string.Empty)}{x??string.Empty}").ToArray()
            //);
            return parameter0valid ? string.Format(help, parameter0, parameter1) : parameter1;
        }


        /// <summary>
        /// Adds a trace to the current value. Useful for tracing the surces and source types
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="value">the trace value</param>
        /// <param name="sourceType">the optional source type (will ne displayed in brackets before the value)</param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithTrace(this IEnvironmentConfiguration configuration, string value, string sourceType=null)
        {
            return configuration.SetValue(Constants.SourceTraceValueKey, 
                string.IsNullOrEmpty(sourceType?.Trim())
                    ?value
                    :$"[{sourceType}]{value}");
        }
        /// <summary>
        /// Gets the trace value
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>the trace value or null if not provided</returns>
        public static string GetTrace(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.GetValue(Constants.SourceTraceValueKey);
        }
    }
}