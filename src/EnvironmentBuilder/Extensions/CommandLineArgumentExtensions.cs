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
        /// <param name="name">the argument name</param>
        /// <param name="configuration">additional configurration scoped to the argument</param>
        /// <returns></returns>
        public static T WithArgument<T>(this T builder,string name,Action<IEnvironmentConfiguration> configuration=null) where T : class, IEnvironmentBuilder
        {
            if (!builder.Configuration.HasValue(typeof(ArgumentParser).FullName))
            {
                builder.WithConfiguration(cfg => cfg.SetValue(typeof(ArgumentParser).FullName,
                    new ArgumentParser(Environment.GetCommandLineArgs())));
            }
            return builder.WithSource(cfg =>
            {
                var parser = cfg.GetValue<ArgumentParser>(typeof(ArgumentParser).FullName);
                var requiredType = cfg.GetBuildType();
                return parser.Value(name,requiredType);
            },cfg =>
            {
                configuration?.Invoke(cfg);
                cfg.WithTrace($"{name}","argument");
                //if (!cfg.HasValue(typeof(ArgumentParser).FullName))
                //{
                //    cfg.SetValue(typeof(ArgumentParser).FullName, 
                //        new ArgumentParser(Environment.GetCommandLineArgs()));
                //}

            }) as T;
        }

        /// <summary>
        /// Shorthand alias for "WithArgument"
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name">the argument name</param>
        /// <param name="configuration">the scoped configuration to add</param>
        /// <returns></returns>
        public static T Arg<T>(this T builder, string name,Action<IEnvironmentConfiguration> configuration) where T : class, IEnvironmentBuilder
        {
            return builder.WithArgument(name,configuration);
        }

        /// <summary>
        /// Shorthand alias for "WithArgument"
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name">the argument name</param>
        /// <returns></returns>
        public static T Arg<T>(this T builder, string name) where T : class, IEnvironmentBuilder
        {
            return builder.WithArgument(name);
        }
        /// <summary>
        /// Shorthand alias for "WithArgument" using the common key set beforehand
        /// See also "CommonExtensions.WithCommonKey"
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IEnvironmentBuilder Arg(this IEnvironmentBuilder builder)
        {
            return builder.WithArgument(builder.Configuration.GetCommonKey());
        }
    }
}
