# XmlFileExtensions



`public static IEnvironmentConfiguration WithXmlFile(this IEnvironmentConfiguration configuration, string file,
            IDictionary<string, string> namespaces = null, bool eagerLoad = false)`

  Adds the xml file to the configuration. Multiple different files can be added.

Parameter | Description 
 --------|--------
configuration | the configuration to use as source or target  
file | the file to use
namespaces | The xml namespaces to load
eagerLoad | if the file should be eagerly loaded rather than lazily loaded
**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder WithXml(this IEnvironmentBuilder builder, string xPath, string file=null)`

  Adds the xml file to the pipe

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target 
xPath | the xpath to retrieve
file | the optional file to use
**Returns** (this is a fluid extension method)


`public static IEnvironmentBuilder Xml(this IEnvironmentBuilder builder, string xPath, string file = null)`

  This is a shorthand for the "WithXml"

Parameter | Description 
 --------|--------
builder | the environment builder instance to use as source or target 
xPath | the xpath to retrieve
file | the optional file to use
**Returns** (this is a fluid extension method)