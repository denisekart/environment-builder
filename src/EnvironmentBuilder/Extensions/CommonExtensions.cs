using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using EnvironmentBuilder.Abstractions;

namespace EnvironmentBuilder.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Sets the type for the T in the configuration.
        /// This may be accessed with the GetBuildType or with the value of
        /// Abstractions.Constants.SourceRequiredTypeKey
        /// </summary>
        /// <typeparam name="T">the type to set</typeparam>
        /// <param name="configuration"></param>
        public static void SetBuildType<T>(this IEnvironmentConfiguration configuration)
        {
            configuration.SetValue(Constants.SourceRequiredTypeKey, typeof(T));
        }
        /// <summary>
        /// Gets the value of the current type of value requested from the build method
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>the type requested</returns>
        public static Type GetBuildType(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.GetValue<Type>(Constants.SourceRequiredTypeKey);
        }
        /// <summary>
        /// Sets the description value. Can be used on a source or an environment builder
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("Use AnnotationExtensions.WithDescription")]
        public static IEnvironmentConfiguration Description(this IEnvironmentConfiguration configuration, string value)
        {
            //if (configuration.HasValue(Constants.SourceDescriptionValueKey)
            //   && configuration.GetValue(Constants.SourceDescriptionValueKey) is string v)
            //    configuration.SetValue(Constants.SourceDescriptionValueKey, v + value);
            //else
            configuration.SetValue(Constants.SourceDescriptionValueKey, value);
            return configuration;
        }
        /// <summary>
        /// Gets the description from the configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [Obsolete("Use one of AnnotationExtensions.GetDescription(s)")]
        public static string Description(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.GetValue(Constants.SourceDescriptionValueKey);
        }
        /// <summary>
        /// Gets the description from the environment builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        [Obsolete("Use one of AnnotationExtensions.GetDescription(s)")]
        public static string Description(this IEnvironmentBuilder builder)
        {
            return builder.Configuration.Description();
        }
        /// <summary>
        /// Prints out the help including the descriptions and the traces
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        [Obsolete("Use AnnotationExtensions.GetHelp")]
        public static string Help(this IEnvironmentBuilder builder)
        {
            return $@"{builder.Description()}
{string.Join("-", Enumerable.Repeat("-", 15).ToArray())}
{string.Join(Environment.NewLine,builder.Bundles.Select(x=>string.Join(" >> ",x.Sources.Select(y=>y.Trace()??"??").ToArray())).ToArray())}";
        }
        /// <summary>
        /// Adds a trace to the current value
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Obsolete("Use AnnotationExtensions.WithTrace")]
        public static IEnvironmentConfiguration Trace(this IEnvironmentConfiguration configuration, string value)
        {
            return configuration.SetValue(Constants.SourceTraceValueKey,value);
        }
        /// <summary>
        /// Gets the trace value
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [Obsolete("Use AnnotationExtensions.GetTrace")]
        public static string Trace(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.GetValue(Constants.SourceTraceValueKey);
        }
        /// <summary>
        /// Adds the default value source to the pipe
        /// </summary>
        /// <typeparam name="T">the type of value</typeparam>
        /// <param name="builder"></param>
        /// <param name="value">the value to add</param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithDefaultValue<T>(this IEnvironmentBuilder builder, T value)
        {
            return builder.WithSource(_ => value,cfg=>cfg.WithTrace(value?.ToString(),"default"));
        }
        /// <summary>
        /// Shorthand alias for <see cref="WithDefaultValue{T}"/>
        /// </summary>
        /// <typeparam name="T">the type of value</typeparam>
        /// <param name="builder"></param>
        /// <param name="value">the value to add</param>
        /// <returns></returns>
        public static IEnvironmentBuilder Default<T>(this IEnvironmentBuilder builder, T value)
        {
            return builder.WithDefaultValue(value);
        }
        /// <summary>
        /// Adds the throwable source to the pipe. Throws an ArgumentException if hit
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="message">the message to throw</param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithException(this IEnvironmentBuilder builder, string message)
        {
            return builder.WithSource<object>(_ => throw new ArgumentException(message??"Missing required variable"),cfg=>cfg.WithTrace(message,"exception"));
        }
        /// <summary>
        /// Shorthand alias for <see cref="WithException"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="message">the message to throw</param>
        /// <returns></returns>
        public static IEnvironmentBuilder Throw(this IEnvironmentBuilder builder, string message=null)
        {
            return builder.WithException(message);
        }
        /// <summary>
        /// Adds the common key to the configuration to be consumed by other types
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="key">the key name</param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithCommonKey(this IEnvironmentBuilder builder, string key)
        {
            return builder.WithConfiguration(cfg => cfg.SetValue(Constants.SourceCommonKeyKey, key));
        }
        /// <summary>
        /// This is a shorthand for <see cref="WithCommonKey"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="key">the key name</param>
        /// <returns></returns>
        public static IEnvironmentBuilder With(this IEnvironmentBuilder builder, string key)
        {
            return builder.WithCommonKey(key);
        }
        /// <summary>
        /// Gets the common key for the source or environment.See also <seealso cref="WithCommonKey"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>the common key or null</returns>
        public static string GetCommonKey(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.GetValue(Constants.SourceCommonKeyKey);
        }
    }
}