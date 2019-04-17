# JsonFileExtensions



`public static IEnvironmentConfiguration WithJsonFile(this IEnvironmentConfiguration configuration, string file, bool eagerLoad=false)
        `


  Adds the json file to the configuration. Multiple different files can be added.
         Use root expression syntax for file selection "$(filename).some.path"

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
file | the file to use
eagerLoad | if the file should be eagerly loaded rather than lazily loaded

**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder WithJson(this IEnvironmentBuilder builder, string jPath)
        `

  Adds the json file to the pipeline

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target 
jPath | the json path to retrieve

**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder Json(this IEnvironmentBuilder builder, string jPath)
        `

  This is a shorthand for the "WithJson"

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target 
jPath | the json path to retrieve

**Returns** (this is a fluid extension method)