using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Implementation;

namespace EnvironmentBuilder.Extensions
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Adds a logger to the pipeline
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="loggerFascade"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithLogger(this IEnvironmentConfiguration configuration,
            ILoggerFascade loggerFascade, LogLevel level)
        {
            if(loggerFascade==null)
                throw new ArgumentException("Cannot set a null logger fascade",nameof(loggerFascade));

            loggerFascade.LogLevel = level;
            return configuration.SetValue(typeof(ILoggerFascade).FullName,(ILoggerFascade)loggerFascade);
        }
        /// <summary>
        /// Adds the default console logger to the pipeline
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithConsoleLogger(this IEnvironmentConfiguration configuration, LogLevel level=LogLevel.Information)
        {
            return configuration.WithLogger(new ConsoleLoggerFascade(),level);
        }
        /// <summary>
        /// Adds a No-Op logger to the pipeline. This logger does nothing.
        /// This is also the default logger configured
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithNoopLogger(this IEnvironmentConfiguration configuration)
        {
            return configuration.WithLogger(new NoopLoggerFascade(),LogLevel.Off);
        }

        /// <summary>
        /// Logs the message of level <see cref="level"/> to the logger.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public static void Log(this IEnvironmentConfiguration configuration, LogLevel level, object message)
        {
            if (configuration.HasValue(typeof(ILoggerFascade).FullName))
                configuration.GetValue<ILoggerFascade>(typeof(ILoggerFascade).FullName)?.Log(level, message);
            else
                configuration.WithNoopLogger();
        }

        /// <summary>
        /// Logs the trace message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogTrace(this IEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Trace,message);
        }
        /// <summary>
        /// Logs the trace message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogTrace(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg=>cfg.Log(LogLevel.Trace, message));
        }

        /// <summary>
        /// Logs the debug message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogDebug(this IEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Debug, message);
        }
        /// <summary>
        /// Logs the debug message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogDebug(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Debug, message));
        }


        /// <summary>
        /// Logs the information message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogInformation(this IEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Information, message);
        }
        /// <summary>
        /// Logs the information message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogInformation(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Information, message));
        }

        /// <summary>
        /// Logs the warning message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogWarning(this IEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Warning, message);
        }
        /// <summary>
        /// Logs the warning message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogWarning(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Warning, message));
        }

        /// <summary>
        /// Logs the error message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogError(this IEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Error, message);
        }
        /// <summary>
        /// Logs the error message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogError(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Error, message));
        }

        /// <summary>
        /// Logs the fatal message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogFatal(this IEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Fatal, message);
        }
        /// <summary>
        /// Logs the fatal message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message"></param>
        public static void LogFatal(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Fatal, message));
        }
    }
}
