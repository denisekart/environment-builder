using System;
using System.IO;
using EnvironmentBuilder.Abstractions;
using EnvironmentBuilder.Implementation;

namespace EnvironmentBuilder.Extensions
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// Sets the current active log level for the configured logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="level">the log level to set</param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithLogLevel(this IEnvironmentConfiguration configuration,
            LogLevel level)
        {
            if (configuration.HasValue(typeof(ILoggerFacade).FullName))
            {
                if (configuration.GetValue<ILoggerFacade>(typeof(ILoggerFacade).FullName) is ILoggerFacade facade)
                {
                    facade.LogLevel = level;
                }
            }

            return configuration;
        }

        /// <summary>
        /// Sets the current active log level for the configured logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="level">the log level to set</param>
        /// <returns></returns>
        public static IEnvironmentBuilder WithLogLevel(this IEnvironmentBuilder configuration,
            LogLevel level)
        {
            if (configuration.Configuration.HasValue(typeof(ILoggerFacade).FullName))
            {
                if (configuration.Configuration.GetValue<ILoggerFacade>(typeof(ILoggerFacade).FullName) is ILoggerFacade facade)
                {
                    facade.LogLevel = level;
                }
            }

            return configuration;
        }



        /// <summary>
        /// Adds a logger to the pipeline
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="loggerFacade">the logger to use</param>
        /// <param name="level">the logging level to use</param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithLogger(this IEnvironmentConfiguration configuration,
            ILoggerFacade loggerFacade, LogLevel level)
        {
            if(loggerFacade==null)
                throw new ArgumentException("Cannot set a null logger facade",nameof(loggerFacade));

            loggerFacade.LogLevel = level;
            return configuration.SetValue(typeof(ILoggerFacade).FullName,(ILoggerFacade)loggerFacade);
        }
        /// <summary>
        /// Adds the default console logger to the pipeline
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="level">the logging level to use</param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithConsoleLogger(this IEnvironmentConfiguration configuration, LogLevel level=LogLevel.Information)
        {
            return configuration.WithLogger(new ConsoleLoggerFacade(),level);
        }

        /// <summary>
        /// Adds a text writer logger to the pipeline
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="writer">the text writer</param>
        /// <param name="level">the logging level to use</param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithTextWriterLogger(this IEnvironmentConfiguration configuration,TextWriter writer, LogLevel level = LogLevel.Information)
        {
            return configuration.WithLogger(new TextWriterLoggerFacade(writer), level);
        }
        /// <summary>
        /// Adds a No-Op logger to the pipeline. This logger does nothing.
        /// This is also the default logger configured
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEnvironmentConfiguration WithNoopLogger(this IEnvironmentConfiguration configuration)
        {
            return configuration.WithLogger(new NoopLoggerFacade(),LogLevel.Off);
        }

        /// <summary>
        /// Logs the message of level "level" to the logger.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="level">the level to log</param>
        /// <param name="message">the message to log</param>
        public static void Log(this IReadonlyEnvironmentConfiguration configuration, LogLevel level, object message)
        {
            if (configuration.HasValue(typeof(ILoggerFacade).FullName))
                configuration.GetValue<ILoggerFacade>(typeof(ILoggerFacade).FullName)?.Log(level, message);
        }

        /// <summary>
        /// Logs the trace message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogTrace(this IReadonlyEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Trace,message);
        }
        /// <summary>
        /// Logs the trace message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogTrace(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg=>cfg.Log(LogLevel.Trace, message));
        }

        /// <summary>
        /// Logs the debug message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogDebug(this IReadonlyEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Debug, message);
        }
        /// <summary>
        /// Logs the debug message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogDebug(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Debug, message));
        }


        /// <summary>
        /// Logs the information message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogInformation(this IReadonlyEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Information, message);
        }
        /// <summary>
        /// Logs the information message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogInformation(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Information, message));
        }

        /// <summary>
        /// Logs the warning message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogWarning(this IReadonlyEnvironmentConfiguration configuration, object message)
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
        /// <param name="message">the message to log</param>
        public static void LogError(this IReadonlyEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Error, message);
        }
        /// <summary>
        /// Logs the error message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogError(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Error, message));
        }

        /// <summary>
        /// Logs the fatal message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogFatal(this IReadonlyEnvironmentConfiguration configuration, object message)
        {
            configuration.Log(LogLevel.Fatal, message);
        }
        /// <summary>
        /// Logs the fatal message to the logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="message">the message to log</param>
        public static void LogFatal(this IEnvironmentBuilder configuration, object message)
        {
            configuration.WithConfiguration(cfg => cfg.Log(LogLevel.Fatal, message));
        }
    }
}
