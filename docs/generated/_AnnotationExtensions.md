# AnnotationExtensions


`public static IEnvironmentConfiguration WithDescription(this IEnvironmentConfiguration configuration, string description)`

  Sets the global description. This value will be used to print out the info/help

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  the configuration to modify
description | the description to set
**Returns** the configuration


`public static IEnvironmentBuilder WithDescription(this IEnvironmentBuilder builder, string description)`

  Sets the scoped description for the current bundle. This value will be used to print out the info for the current bundle.

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  
description | the description to use
**Returns** (this is a fluid extension method)


`public static string GetDescription(this IEnvironmentBuilder builder)`

  Gets the global description or null if none exist

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  the builder
**Returns** the description


`public static IEnumerable<string> GetDescriptions(this IEnumerable<IEnvironmentBundle> bundles)`

  Gets a list of descriptions for the bundles in the correct order the bundles were added

Parameter | Description 
 --------|--------
bundles | the bundles to get the descriptions for 
**Returns** (this is a fluid extension method)


`public static string GetHelp(this IEnvironmentBuilder builder)`

  Gets the formatted string containing the descriptions of the bundles and the utility.

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  
**Returns** the formatted help string


`public static IEnvironmentConfiguration WithTrace(this IEnvironmentConfiguration configuration, string value, string sourceType=null)`

  Adds a trace to the current value. Useful for tracing the surces and source types

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
value | the trace value
sourceType | the optional source type (will ne displayed in brackets before the value)
**Returns** (this is a fluid extension method)


`public static string GetTrace(this IReadonlyEnvironmentConfiguration configuration)`

  Gets the trace value

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
**Returns** the trace value or null if not provided