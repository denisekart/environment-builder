# Environment builder

EnvironmentBuilder is a utility for building the application environment using various configuration sources such as arguments, environment variables, json files, xml files etc. Its primary interface is a fluid API that is open to configuration and extension. The utility can easily be integrated with 3rd party tools and utilities.

We currently support the .NET framework 3.5 and greater and also the .NETStandard 2.0 spec.

This utility has no 3rd party dependencies.

## TL; DR;

THE DOCS ARE STILL UNDER CONSTRUCTION (\*\* insert a ..."working"... meme here \*\*)

Check out the [complete documentation](docs/DOCUMENTATION.md).

Use:
``` csharp
using EnvironmentBuilder;
using EnvironmentBuilder.Extensions;
...
var env=EnvironmenManager.Create();
var fooBundle=env.Arg("foo").Env("foo").Default("not_foo").Bundle();
var barBundle=env.Arg("bar").Throw("Missing bar argument").Bundle();
var foo=fooBundle.Build(); //argument then> env variable then> default value
var bar=barBundle.Build<int>() //argument of type int then> throws exception
```
Install:
```csharp
// package manager
Install-Package EnvironmentBuilder
// or dotnet cli
dotnet add package EnvironmentBuilder
// or cake build
#addin "nuget:?package=EnvironmentBuilder"
using EnvironmentBuilder.Extensions;
// or build sources
build.ps1 --target=Build
```

## How it works

Check out the [complete documentation](docs/DOCUMENTATION.md).

The main idea behind the environment builder is to combine various application entry points and configuration sources and combine them into a single "pipe".

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

A more complex use case example:
```csharp
//setup the manager utility
var env = EnvironmentManager.Create(config =>
    config
    .WithJsonFile("json1.json")
    .WithXmlFile("xml1.xml")
    .WithEnvironmentVariablePrefix("Foo_")
    .WithConsoleLogger(EnvironmentBuilder.Abstractions.LogLevel.Trace)
);
//create a simple pipe
var actionBundle = env
    .Arg("action")
    .Arg("a")
    .Env("environment_action")
    .Json("$.action")
    .Xml("/action")
    .Default("foo_action")
    .Bundle();
//retrieve the value
var action = actionBundle.Build();
//action="foo_action"
```
The example does the following
1. Creates a lightweight EnvironmentManager and configures it to:
   - Use a json file json1.json
   - Use an xml file xml1.xml
   - Prefix environment variables with the word Foo_
   - Use a simple console logger with the level of Trace
2. Creates a bundle (or a pipe) of configuration sources in order:
   - Program argument "action" then
   - program argument "a" then
   - environment variable "Foo_environment_action" then
   - json file field "action" then
   - xml file node "action" then
   - the default value "foo_action"
3. Retrieves the value using the "Build" action:
   - It checks the sources in order (2.) and returns the value of the first source that was specified(existed) at the time that this action was invoked
   - It returns the value "foo_action" because none of other sources were available


TODO: Missing complete documentation

## Using it
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

We use Cake build for build automation. Use the target "Build" for building the source. (*our Cake scripts use our own environment builder utility for setting up the environment*).

TODO:

This is a work in progress. Will be completed till May 1st 2019. Package is fully functional but not yet documented. Feel free to run unit tests using the default task in the Cake build.