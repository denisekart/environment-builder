# CommonExtensions



`public static void SetBuildType<T>(this IEnvironmentConfiguration configuration)`

  Sets the type for the T in the configuration.
  This may be accessed with the GetBuildType or with the value of
  Abstractions.Constants.SourceRequiredTypeKey

Parameter | Description 
 --------|--------
T | the type to set
configuration | the configuration to use as source or target



`public static Type GetBuildType(this IReadonlyEnvironmentConfiguration configuration)`

  Gets the value of the current type of value requested from the build method

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target 

**Returns** the type requested


`public static IEnvironmentBuilder WithDefaultValue<T>(this IEnvironmentBuilder builder, T value)`

  Adds the default value source to the pipe

Parameter | Description 
 --------|--------
T | the type of value
builder | the environment builder instance to use as source or target 
value | the value to add

**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder Default<T>(this IEnvironmentBuilder builder, T value)`

  Shorthand alias for WithDefaultValue{T}

Parameter | Description 
 --------|--------
T | the type of value
builder | the environment builder instance to use as source or target 
value | the value to add

**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder WithException(this IEnvironmentBuilder builder, string message)`

  Adds the throwable source to the pipe. Throws an ArgumentException if hit

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target 
message | the message to throw

**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder Throw(this IEnvironmentBuilder builder, string message=null)`

  Shorthand alias for WithException

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target 
message | the message to throw

**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder WithCommonKey(this IEnvironmentBuilder builder, string key)`

  Adds the common key to the configuration to be consumed by other types

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target 
key | the key name

**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder With(this IEnvironmentBuilder builder, string key)`
  This is a shorthand for WithCommonKey

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target 
key | the key name

**Returns** (this is a fluid extension method)


`public static string GetCommonKey(this IReadonlyEnvironmentConfiguration configuration)`
  Gets the common key for the source or environment.See also WithCommonKey

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target 

**Returns** the common key or null