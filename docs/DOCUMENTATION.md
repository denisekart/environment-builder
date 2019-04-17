# Environment builder


EnvironmentBuilder is a utility for building the application environment using various configuration sources such as arguments, environment variables, json files, xml files etc. Its primary interface is a fluid API that is open to configuration and extension. The utility can easily be integrated with 3rd party tools and utilities.

We currently support the .NET framework 3.5 and greater and also the .NETStandard 2.0 spec.

This utility has no 3rd party dependencies.

---

**Table of contents:**
- [Environment builder](#environment-builder)
  - [Installation](#installation)
  - [Usage](#usage)
    - [Namespaces](#namespaces)
    - [Environment Manager](#environment-manager)
      - [Methodology and naming](#methodology-and-naming)
      - [Methods and Properties](#methods-and-properties)
  - [Example:](#example)
      - [Configuration](#configuration)
      - [Extensibility](#extensibility)
    - [Extensions](#extensions)
      - [Common Extensions](#common-extensions)
      - [Annotation Extensions](#annotation-extensions)
      - [Logging Extensions](#logging-extensions)
      - [Argument Extensions](#argument-extensions)
      - [Environment Variable Extensions](#environment-variable-extensions)
      - [Json File Extensions](#json-file-extensions)
      - [Xml File Extensions](#xml-file-extensions)
      - [Future Work](#future-work)
    - [Contribute](#contribute)

---

## Installation
You can implement this utility in your application using the [nuget.org feed](https://www.nuget.org/packages/EnvironmentBuilder/) or by cloning this repo and building the source code yourself.

Using the package manager:
```powershell
Install-Package EnvironmentBuilder
```

Using the .NET CLI:
```bash
dotnet add package EnvironmentBuilder
```

Using it in Cake scripts:
```csharp
#addin "nuget:?package=EnvironmentBuilder"
using EnvironmentBuilder.Extensions;
```

Building from source:
``` powershell
.\build.ps1 --target=Build --configuration=Release --output=bin 
```
(*our Cake scripts use our own environment builder utility for setting up the environment*).

---

## Usage

The main idea behind the environment builder is to use various application entry points and configuration sources and combine them into a single "pipe".

A simple use case example:
```csharp
var env = EnvironmentManager.Create();

var actionBundle=env
    .Arg("action")
    .Env("action")
    .Default("foo_action")
    .Bundle();

var action=actionBundle.Build()
```
The example does the following
1. Creates a lightweight EnvironmentManager
2. Creates a bundle (or a pipe) of configuration sources in order:
   - Program argument "action"
   - Environment variable "action"
   - Default value "foo_action"
3. Retrieves the value using the "Build" action
   - It checks the sources in order they were configured
   - It returns value "foo_action"

### Namespaces

The utility uses the following namespaces:
  - **EnvironmentBuilder** - the root namespace
  - **EnvironmentBuilder.Extensions** - this is where all of the candy is implemented
  - **EnvironmentBuilder.Abstractions** - this is where all of the interfaces and constants are located

### Environment Manager

*EnvironmentManager* is the main entry point for the utility. This is the place where all of the root functionality is located. Creating an environment manager object can be done by invoking one of the static functions

``` csharp
public static IEnvironmentBuilder Create()
public static IEnvironmentBuilder Create(Action<IEnvironmentConfiguration> configuration)
```
For example
``` csharp
var env=EnvironmentManager.Create();
//or
var env2=EnvironmentManager.Create(config=>{
    //configure the manager here
});
```
These methods return an object of type `IEnvironmentBuilder` that can be used to `Bundle` and `Build` the `Pipe`

#### Methodology and naming
`Bundle` - refers to a collection of sources that are groupped in a logical unit.

`Build` - refers to retreiving the value from the sources in the `Bundle`.

`Pipe` - refers to a collection of sources that will eventually be groupped to a `Bundle`.


#### Methods and Properties
The interface `IEnvironmentBuilder` returned via `EnvironmentManager.Create()` function exposes several methods and properties that can be used in constructing and configuring the utility.

```csharp
IReadonlyEnvironmentConfiguration Configuration { get; }
```
Returns the current read only version instance of the configuration.

---

```csharp
IEnumerable<IEnvironmentBundle> Bundles { get; }
```
Returns an enumeration of the bundles constructed in this instance of the environment manager.

---

```csharp
IEnvironmentBuilder WithConfiguration(Action<IEnvironmentConfiguration> configuration);
```
Configures the current instance of the environment manager with the configuration. 
This method can be called multiple times. Every time the method is called, the configuration of the current instance is updated. No previous configuration is lost.

---


```csharp
//[1]
IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source,
            Action<IEnvironmentConfiguration> configuration);
//[2]
IEnvironmentBuilder WithSource<T>(Func<IReadonlyEnvironmentConfiguration, T> source);
//[3]
IEnvironmentBuilder WithSource<T>(Func<T> source);
```
Adds a new source to the current pipe. The Func<> is the "source" being added to the pipe. As such, the Func<> will only be evaluated when the value is needed and not before.
- [1] - The configuration will be provided while evaluating the source. The configuration may also be updated using this overload(the same as calling `WithConfiguration(...)` before configuring the source).

Example:
``` csharp
//when evaluated, the value would be "foo"
env.WithSource(
  config=>config.GetValue("name"),
  config=>config.SetValue("name","foo")
  );
```
- [2] - The configuration will be provided while evaluating the source.

Example:
``` csharp
//when evaluated, the value would be "foo"
env.WithSource(
  config=>"foo"
  );
```
- [3] - The source will be evaluated without any state provided.

Example:
``` csharp
//when evaluated, the value would be "foo"
env.WithSource(
  ()=>"foo"
  );
```

---


```csharp
IEnvironmentBundle Bundle();
```
Bundles (constructs) the current pipe and returns the bundle. The bundle is also added to the `Bundles` property.

----


```csharp
string Build();
T Build<T>();
```
Builds the current pipe or bundle and returns the typed value.
Type of `T` can be an Enumerable, or any complex or simple reference or value type.
The utility will try to parse the built value and transform it to the requested type by any means. If the value can not be converted to the requested type, the default value of `T` is evaluated.

Example:
```csharp
//will convert the string "1" into an int
var value=env.WithSource(()=>"1").Build<int>();

class Foo{
  public string Bar{get;set;} 
  public int Baz{get;set;}
}
//Given the json file with content: {"Bar":"some value","Baz":2}
//will convert the json string into the type Foo
var value=env.Json("$").Build<Foo>();

```
---

#### Configuration
The configuration `IEnvironmentConfiguration` and its readonly counterpart `IReadonlyEnvironmentConfiguration` provide state and are responsible for configuring and maintaining configuration in the utility.

The type `IEnvironmentConfiguration` derives from `IReadonlyEnvironmentConfiguration` so a subset of functionality is shared.

``` csharp
//IReadonlyEnvironmentConfiguration
string GetValue(string key);
T GetValue<T>(string key);
```
Returns the value of the configuration item with the specified key or default(of T) if the key does not exist.


---


``` csharp
//IReadonlyEnvironmentConfiguration
T GetFactoryValue<T>(string key);
```
Gets the factory value it the factory exists or default (of T) if no factory with the key is provided. The value of the factory is resolved (as opposed to "GetValue) every time this method is called.

---

``` csharp
//IReadonlyEnvironmentConfiguration
bool HasValue(string key);
```
Returns the value indicating if the value with the specified key exists in the configuration.

---

``` csharp
IEnvironmentConfiguration SetValue<T>(string key, T value);
```
Sets the value to the configuration. The value with the specified key will be available in every access point from this point on.

---

``` csharp
IEnvironmentConfiguration SetFactoryValue<T>(string key, Func<T> value);
```
Sets the factory to the configuration. The factory with the specified key will be available in every access point from this point on.

---
The internal extensions rely heavily on the configuration to maintain state. For example, once the json file is parsed, an instance of the parser is kept inside the configuration so every new call requiring the file is optimized for performance. See the Extensibility section for more info.


#### Extensibility

The utility can easily be extended to provide extra functionality to the environment manager. The internal implementations extend the base functionality described in previous section using extension methods. 

To show a way of extending the base functionality of the utility let's write an example for a source that increments a number by a random interval each time the "Build" method is called and returns the previous value.

``` csharp
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;

namespace EnvironmentBuilderRandomContrib.Extensions
{
    // The static class that will hold the extension methods
    public static class RandomExtensions
    {
        // Unique keys to use in the configuration - they should be unique system wide, perhaps the assembly name and description
        public const string RngKey = "RandomExtensions.RngKey";
        public const string SaverKey = "RandomExtensions.NumberSaverKey";

        // This method will be used to add the RNG to the global configuation
        public static IEnvironmentConfiguration WithRandomNumberGenerator(this IEnvironmentConfiguration configuration,
            Random random)
        {
          //using a factory for random numbers
            return configuration.SetFactoryValue(RngKey, () => random.Next());
        }
        // this method will be used to get the next randomly incremented number
        public static IEnvironmentBuilder Random(this IEnvironmentBuilder builder)
        {
            if (!builder.Configuration.HasValue(SaverKey))
            {
              //using configuration values to store the "NumberSaver"
              // this configuration is "global"
                builder.WithConfiguration(cfg => cfg.SetValue(SaverKey, new NumberSaver()));
            }

            return builder.WithSource(config =>
            {
                //just a simple logic to increment and save values
                var nextValue = config.GetFactoryValue<int>(RngKey);
                var saver = config.GetValue<NumberSaver>(SaverKey);
                var oldKey = saver.Prev;
                saver.Prev += nextValue;
                return oldKey;

            }, config => 
            {
              //this configuration is "specific" to that source
              config.WithTrace("my random value","my random source")
            });
        }
        // this is just an example class that holds the previous value
        //a more suitable approach would be to save the value itself to the configuration
        // but this shows the ability to add extra logic and save it in the configuration
        private class NumberSaver
        {
            public int Prev { get; set; } = 0;
        }

    }
}

```
Such an extension could be implemeted like so:

``` csharp
//adds the RNG to the global configuration
var env = EnvironmentManager.Create(cfg => 
          cfg.WithRandomNumberGenerator(new Random(42)));
//calls the random 4 times each time incrementing the value randomly
int next1 = env.Random().Build<int>();
int next2 = env.Random().Build<int>();
int next3 = env.Random().Build<int>();
int next4 = env.Random().Build<int>();
//following assertions must hold true
Assert.True(next2>next1);
Assert.True(next3>next2);
Assert.True(next4>next3);
```

### Extensions

Extension methods provide a means to wrap various actions in a single logical extension. See chapter [Extensibility](#Extensibility) for an example.

There are quite a few such extensions implemented in the core package under the namespace `EnvironmentBuilder.Extensions`.

#### Common Extensions
Provide common core functionalities that are and can be useful in any other extension.

See [method descriptions](generated/_CommonExtensions.md).

Example:

``` csharp
Assert.Equal("Foo",EnvironmentManager.Create().Default("Foo").Build());
//is the same as
Assert.Equal("Foo",EnvironmentManager.Create().WithDefaultValue("Foo").Build());

Assert.Throws<ArgumentException>(() => EnvironmentManager.Create().Throw().Build());
//is the same as
Assert.Throws<ArgumentException>(() => EnvironmentManager.Create().WithException(null).Build());

//will search for values of env(Key) then arg(Key)
var builder=EnvironmentManager.Create().With("Key").Env().Arg();
Assert.Equal("Key",builder.Configuration.GetCommonKey());
```

#### Annotation Extensions
Provide functionality to annotate the sources, bundler and environament builders

See [method descriptions](generated/_AnnotationExtensions.md).

Example:

``` csharp
var env = EnvironmentManager.Create(config => config.WithDescription("Main description"));
Assert.Equal("Main description",env.GetDescription());

env.WithDescription("some source description").Default("foo").Bundle();
Assert.True(new[]{ "some source description" }.SequenceEqual(env.Bundles.GetDescriptions()));

env.WithDescription("d2").Throw("Throw").Bundle();

var help = env.GetHelp();
Assert.Equal(
    @"Main description
    
    - [default]foo
      some source description
    - [exception]Throw
      d2
      "
      ,
    help);
```

#### Logging Extensions
Provide functionalities related to logging.

See [method descriptions](generated/_LoggingExtensions.md).

Example:

``` csharp
var writer=new TestOutputHelperWriter(_outputHelper);
var env = EnvironmentManager.Create(config =>
    config
        .WithTextWriterLogger(writer)
        .WithLogLevel(EnvironmentBuilder.Abstractions.LogLevel.Trace));

writer.TextWritten += (s, e) => Assert.True(e.Text == "[TRACE] Foo");

env.LogTrace("Foo");
```

#### Argument Extensions
Provide the default command line argument parsers and sources.

See [method descriptions](generated/_CommandLineArgumentExtensions.md).

Example:

``` csharp
var env = EnvironmentManager.Create();
var var1 = env.Arg("longOption").Arg("l").Bundle();       
```

#### Environment Variable Extensions
Provide the default environment variable parsers and sources. The environment variables can be retrieved from the current process, the user or the machine.

See [method descriptions](generated/_EnvironmentVariableExtensions.md).

Example:

``` csharp
Environment.SetEnvironmentVariable("option","bar");
Environment.SetEnvironmentVariable("prefix_option","foo");
var env = EnvironmentManager.Create(config=>config.WithEnvironmentVariablePrefix("prefix_"));
Assert.Equal("foo",env.Env("option").Build());
Assert.Equal("bar",env.Env("option",c=>c.WithNoEnvironmentVariablePrefix()).Build());
```

#### Json File Extensions
Provide the default json file parsers and sources. Simple and complex types can be parsed. The values are parsed using the [JSON path](https://goessner.net/articles/JsonPath/) notation. Multiple json files are supported at once.

See [method descriptions](generated/_JsonFileExtensions.md).

Example:

``` csharp
var env = EnvironmentManager.Create(config => 
    config.WithJsonFile("json1.json").WithJsonFile("json2.json"));
Assert.Equal("bar",env.Json("$(json1).foo").Build());
Assert.Equal("baz",env.Json("$(json2).bar").Build());
```

#### Xml File Extensions
Provide the default xml file parsers and sources. Simple types and enumerations can be parsed. The values are parsed using the XPath expressions. Multiple xml files are supported at once.

See [method descriptions](generated/_XmlFileExtensions.md).

Example:

``` csharp
var env = EnvironmentManager.Create(config => config.WithXmlFile("xml1.xml"));
Assert.Equal("bar",env.Xml("/foo").Build());
```

#### Future Work
I have absolutely no idea when but...
- SQL database parsers and sources
- Redis KV store parsers and sources
- [Sky is the limit]

### Contribute
If you would like to contribute to this project just submit a PR.

Here are some basic sanity rules to follow:
- The added functionality should be unit tested extensively
- No external dependencies should be added
- The funcionality should be implemented in all of the supported frameworks
- The core (EnvironmentManager) should not be modified (contact me if you think it should be - i'm open to suggestion)
- The copied, referenced or otherwise borrowed code should have a license no more restrictive than the MIT license