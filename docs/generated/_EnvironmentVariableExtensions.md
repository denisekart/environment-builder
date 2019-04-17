# EnvironmentVariableExtensions



` public static void SetEnvironmentTarget(this IEnvironmentConfiguration configuration, EnvironmentVariableTarget target)`

  Sets the target for the environment variable resolution

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
target | the target to use


` public static EnvironmentVariableTarget GetEnvironmentTarget(this IReadonlyEnvironmentConfiguration configuration)`

  Gets the value of the environment variable store used

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  

**Returns** the target currently used

 
`public static IEnvironmentConfiguration WithEnvironmentVariablePrefix(
            this IEnvironmentConfiguration configuration, string prefix)`

  Sets the environment variable prefix to the source or environment

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
prefix | the prefix to use

**Returns** (this is a fluid extension method)


`public static IEnvironmentConfiguration WithNoEnvironmentVariablePrefix(
            this IEnvironmentConfiguration configuration)`

  Clears the environment variable prefix for the source or the environment

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  

**Returns** (this is a fluid extension method)


` public static string GetEnvironmentVariablePrefix(this IReadonlyEnvironmentConfiguration configuration)`

  Gets the environment variable prefix. Defaults to null

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  

**Returns** the prefix currently being used or null


` public static IEnvironmentBuilder WithEnvironmentVariable(this IEnvironmentBuilder builder, string name, Action<IEnvironmentConfiguration> configuration=null)`

  Adds the environment variable source to te pipe

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  
name | the name of the variable
configuration | the configuration to use as source or target  the scoped configuration to add

**Returns** (this is a fluid extension method)


` public static IEnvironmentBuilder Env(this IEnvironmentBuilder builder, string name, Action<IEnvironmentConfiguration> configuration = null)`

  Shorthand for "WithEnvironmentVariable"

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  
name | the name of the variable
configuration | the configuration to use as source or target  the configuration to use

**Returns** (this is a fluid extension method)


` public static IEnvironmentBuilder Env(this IEnvironmentBuilder builder, Action<IEnvironmentConfiguration> configuration = null)`

  Shorthand alias for "WithEnvironmentVariable" using the common key set beforehand
  "CommonExtensions.WithCommonKey"

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  

**Returns** (this is a fluid extension method)