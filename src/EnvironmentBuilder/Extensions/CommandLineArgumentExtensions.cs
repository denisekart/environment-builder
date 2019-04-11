using System;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Implementation;

namespace EnvironmentBuilder.Extensions
{
    public static class CommandLineArgumentExtensions
    {
        /// <summary>
        /// Adds the command line argument source to the pipe
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithArgument(this IEnvironmentBuilder builder,string name,Action<IEnvironmentConfiguration> configuration=null)
        {
            return builder.WithSource(cfg =>
            {
                var parser = cfg.GetValue<ArgumentParser>(typeof(ArgumentParser).FullName);
                var requiredType = cfg.GetBuildType();
                return parser.Value(name,requiredType);
            },cfg =>
            {
                configuration?.Invoke(cfg);
                cfg.Trace($"[argument]{name}");
                if (!cfg.HasValue(typeof(ArgumentParser).FullName))
                {
                    cfg.SetValue(typeof(ArgumentParser).FullName, 
                        new ArgumentParser(Environment.GetCommandLineArgs()));
                }

            });
        }
        /// <summary>
        /// Shorthand alias for <see cref="WithArgument"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Arg(this IEnvironmentBuilder builder, string name)
        {
            return builder.WithArgument(name);
        }
        /// <summary>
        /// Shorthand alias for <see cref="WithArgument"/> using the common key set beforehand
        /// <seealso cref="CommonExtensions.WithCommonKey"/>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Arg(this IEnvironmentBuilder builder)
        {
            return builder.WithArgument(builder.Configuration.GetCommonKey());
        }
    }
}
