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
      - [Methods](#methods)
      - [Configuration](#configuration)
      - [Extensibility](#extensibility)
    - [Extensions](#extensions)
      - [Common Extensions](#common-extensions)
      - [Logging Extensions](#logging-extensions)
      - [Argument Extensions](#argument-extensions)
      - [Environment Variable Extensions](#environment-variable-extensions)
      - [Json File Extensions](#json-file-extensions)
      - [Xml File Extensions](#xml-file-extensions)
      - [Future Work](#future-work)
    - [Contributions](#contributions)

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

#### Methods

TODO: Describe available methods

#### Configuration

TODO: Describe configuration scenarios

#### Extensibility

TODO: Describe a way to extend this

### Extensions

TODO: Describe the purpose of the extensions - just the basics

#### Common Extensions

TODO

#### Logging Extensions

TODO

#### Argument Extensions

TODO

#### Environment Variable Extensions

TODO

#### Json File Extensions

TODO

#### Xml File Extensions

TODO

#### Future Work

TODO: Describe future work

### Contributions

TODO: Describe how to contribute