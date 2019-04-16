# CommandLineArgumentExtensions



` public static IEnvironmentBuilder WithArgument(this IEnvironmentBuilder builder,string name,Action<IEnvironmentConfiguration> configuration=null)`

  Adds the command line argument source to the pipe

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  
name | the argument name
configuration | additional configurration scoped to the argument
**Returns** 


` public static IEnvironmentBuilder Arg(this IEnvironmentBuilder builder, string name,Action<IEnvironmentConfiguration> configuration)`

  Shorthand alias for "WithArgument"

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  
name | the argument name
configuration | the scoped configuration to add
**Returns** 


` public static IEnvironmentBuilder Arg(this IEnvironmentBuilder builder, string name)`

  Shorthand alias for "WithArgument"

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  
name | the argument name
**Returns** 


` public static IEnvironmentBuilder Arg(this IEnvironmentBuilder builder)`

  Shorthand alias for "WithArgument" using the common key set beforehand
  See also "CommonExtensions.WithCommonKey"

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target  
**Returns** 