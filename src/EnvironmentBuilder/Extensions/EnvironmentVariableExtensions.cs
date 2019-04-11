using System;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Implementation;

namespace EnvironmentBuilder.Extensions
{
    public static class EnvironmentVariableExtensions
    {
        /// <summary>
        /// Sets the target for the environment variable resolvation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="target"></param>
        public static void SetEnvironmmentTarget(this IEnvironmentConfiguration configuration, EnvironmentVariableTarget target)
        {
            configuration.SetValue(Constants.EnvironmentVariableTargetKey, target);
        }
        /// <summary>
        /// Gets the value of the environment variable store used
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static EnvironmentVariableTarget GetEnvironmentTarget(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.HasValue(Constants.EnvironmentVariableTargetKey)
                ?configuration.GetValue<EnvironmentVariableTarget>(Constants.EnvironmentVariableTargetKey) :
                EnvironmentVariableTarget.Process;
        }
        /// <summary>
        /// Sets the environment variable prefix to the source or environment
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithEnvironmentVariablePrefix(
            this IEnvironmentConfiguration configuration, string prefix)
        {
            return configuration.SetValue(Constants.EnvironmentVariablePrefixKey, prefix);
        }
        /// <summary>
        /// Clears the environment variable prefix for the source or the environment
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithNoEnvironmentVariablePrefix(
            this IEnvironmentConfiguration configuration)
        {
            return configuration.SetValue<string>(Constants.EnvironmentVariablePrefixKey, null);
        }
        /// <summary>
        /// Gets the environment variable prefix. Defaults to null
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string GetEnvironmentVariablePrefix(this IReadonlyEnvironmentConfiguration configuration)
        {
            return configuration.HasValue(Constants.EnvironmentVariablePrefixKey)
                ?configuration.GetValue(Constants.EnvironmentVariablePrefixKey)
                :null;
        }
        /// <summary>
        /// Adds the environment variable source to te pipe
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithEnvironmentVariable(this IEnvironmentBuilder builder, string name, Action<IEnvironmentConfiguration> configuration=null)
        {
            return builder.WithSource(cfg =>
            {
                var parser = cfg.GetValue<EnvironmentVariableParser>(typeof(EnvironmentVariableParser).FullName);
                var requiredType = cfg.GetBuildType();
                var target = cfg.GetEnvironmentTarget();
                var prefix = cfg.GetEnvironmentVariablePrefix();
                return parser.Value(name, requiredType,target,prefix);
            }, cfg =>
            {
                configuration?.Invoke(cfg);
                cfg.Trace($"[environment]{name}");
                if (!cfg.HasValue(typeof(EnvironmentVariableParser).FullName))
                {
                    cfg.SetValue(typeof(EnvironmentVariableParser).FullName,
                        new EnvironmentVariableParser());
                }

            });
        }

        /// <summary>
        /// Shorthand for <see cref="WithEnvironmentVariable"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Env(this IEnvironmentBuilder builder, string name, Action<IEnvironmentConfiguration> configuration = null)
        {
            return builder.WithEnvironmentVariable(name,configuration);
        }

        /// <summary>
        /// Shorthand alias for <see cref="WithEnvironmentVariable"/> using the common key set beforehand
        /// <seealso cref="CommonExtensions.WithCommonKey"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Env(this IEnvironmentBuilder builder, Action<IEnvironmentConfiguration> configuration = null)
        {
            return builder.WithEnvironmentVariable(builder.Configuration.GetCommonKey(),configuration);
        }
        
    }
}