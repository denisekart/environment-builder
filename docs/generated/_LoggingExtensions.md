# LoggingExtensions



`public static IEnvironmentConfiguration WithLogLevel(this IEnvironmentConfiguration configuration,
            LogLevel level)`

  Sets the current active log level for the configured logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
level | the log level to set

**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder WithLogLevel(this IEnvironmentBuilder configuration,
            LogLevel level)`

  Sets the current active log level for the configured logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
level | the log level to set

**Returns** (this is a fluid extension method)


`public static IEnvironmentConfiguration WithLogger(this IEnvironmentConfiguration configuration,
            ILoggerFacade loggerFacade, LogLevel level)`

  Adds a logger to the pipeline

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
loggerFacade | the logger to use
level | the logging level to use

**Returns** (this is a fluid extension method)


`public static IEnvironmentConfiguration WithConsoleLogger(this IEnvironmentConfiguration configuration, LogLevel level=LogLevel.Information)`

  Adds the default console logger to the pipeline

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
level | the logging level to use

**Returns** (this is a fluid extension method)


`public static IEnvironmentConfiguration WithTextWriterLogger(this IEnvironmentConfiguration configuration,TextWriter writer, LogLevel level = LogLevel.Information)`

  Adds a text writer logger to the pipeline

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
writer | the text writer
level | the logging level to use

**Returns** (this is a fluid extension method)


`public static IEnvironmentConfiguration WithNoopLogger(this IEnvironmentConfiguration configuration)`

  Adds a No-Op logger to the pipeline. This logger does nothing.
  This is also the default logger configured

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  

**Returns** (this is a fluid extension method)


`public static void Log(this IReadonlyEnvironmentConfiguration configuration, LogLevel level, object message)`

  Logs the message of level "level" to the logger.

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
level | the level to log
message | the message to log


`public static void LogTrace(this IReadonlyEnvironmentConfiguration configuration, object message)`

  Logs the trace message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogTrace(this IEnvironmentBuilder configuration, object message)`

  Logs the trace message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogDebug(this IReadonlyEnvironmentConfiguration configuration, object message)`

  Logs the debug message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogDebug(this IEnvironmentBuilder configuration, object message)`

  Logs the debug message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogInformation(this IReadonlyEnvironmentConfiguration configuration, object message)`

  Logs the information message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogInformation(this IEnvironmentBuilder configuration, object message)`

  Logs the information message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogWarning(this IReadonlyEnvironmentConfiguration configuration, object message)`

  Logs the warning message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogWarning(this IEnvironmentBuilder configuration, object message)`

  Logs the warning message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | 


`public static void LogError(this IReadonlyEnvironmentConfiguration configuration, object message)`

  Logs the error message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogError(this IEnvironmentBuilder configuration, object message)`

  Logs the error message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogFatal(this IReadonlyEnvironmentConfiguration configuration, object message)`

  Logs the fatal message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log


`public static void LogFatal(this IEnvironmentBuilder configuration, object message)`

  Logs the fatal message to the logger

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
message | the message to log