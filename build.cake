#addin "nuget:?package=EnvironmentBuilder&version=1.0.0"
using EnvironmentBuilder.Extensions;

#region ARGUMENTS
var env=EnvironmentBuilder.EnvironmentManager.Create(config=>
   config.WithJsonFile("build.json")
   .WithEnvironmentVariablePrefix("cake_"));

var target=env.Arg("target").Arg("t").Env("target").Json("$.build.target").Default("Default").Bundle();
var configuration=env.Arg("configuration").Arg("c").Env("configuration").Json("build.configuration").Default("Debug").Bundle();
var output=env.Arg("output").Arg("o").Env("output").Json("build.output").Default("./artifacts").Bundle();
var package=env.Arg("packageDirectory").Arg("p").Env("packageDirectory").Json("build.packageDirectory").Default("./packages").Bundle();
var mainProjectFile=env.Default("./src/EnvironmentBuilder/EnvironmentBuilder.csproj").Bundle();
var nugetApiKey=env.WithEnvironmentVariable("NUGET_API_KEY",config=>
config.WithNoEnvironmentVariablePrefix()
.SetEnvironmmentTarget(EnvironmentVariableTarget.Machine))
.Throw("Missing nuget api key").Bundle();

#endregion //ARGUMENTS

#region VARIABLES


#endregion //VARIABLES

#region TASKS
Task("Clean")
   .Description("Runs the clean task")
   .Does(()=>{
      Information("Running clean...");
      if(DirectoryExists(output.Build()))
         DeleteDirectory(output.Build(),new DeleteDirectorySettings{Recursive=true});
      if(DirectoryExists(package.Build()))
         DeleteDirectory(package.Build(),new DeleteDirectorySettings{Recursive=true});

         var projectFiles = GetFiles("./tests/**/*.csproj")
         .Union(GetFiles("./src/**/*.csproj"));
      foreach(var file in projectFiles)
      {
         DotNetCoreClean(file.FullPath);
      }
   });
Task("Restore")
   .Description("Runs the nuget restore task")
   .Does(()=>{
      Information("Running restore for all projects...");
      DotNetCoreRestore();
   });

//not used at the moment
void BuildTarget(string target){
   var relativeOutputRoot="./../../"+output.Build()+"/"+target;
   Information("Path for target "+target+": "+relativeOutputRoot);
   MSBuild(File(mainProjectFile.Build()),
   new MSBuildSettings{
      Configuration=configuration.Build(),
      Restore=false,
      PlatformTarget=PlatformTarget.MSIL,
      Verbosity=Verbosity.Minimal
   }
   .WithProperty("OutDir",relativeOutputRoot)
   .WithProperty("TargetFramework",target));
}
Task("Build")
   .Description("Runs the build task")
   .IsDependentOn("Restore")
   .Does(()=>{
      Information("Running build...");
      // BuildTarget("net35");
      DotNetCoreBuild(".",
       new DotNetCoreBuildSettings{
          Configuration=configuration.Build(),
          OutputDirectory=output.Build(),
          NoRestore=true,
          EnvironmentVariables=new Dictionary<string,string>{
             {"TargetFramework","net46"}
          }
       });
   });
Task("Nuget-Pack")
.IsDependentOn("Clean")
.IsDependentOn("Version")
// .IsDependentOn("Build")
.Does(()=>{
   DotNetCorePack(mainProjectFile.Build(),new DotNetCorePackSettings{
      OutputDirectory=package.Build(),
      Configuration="Release"
   });
});
Task("Nuget-Push")
.IsDependentOn("Nuget-Pack")
.Does(()=>{
   DotNetCoreNuGetPush("EnvironmentBuilder.*.nupkg", new DotNetCoreNuGetPushSettings{
      Source = "https://api.nuget.org/v3/index.json",
      ApiKey = nugetApiKey.Build(),
      WorkingDirectory=package.Build()
   });

});
Task("Test")
.IsDependentOn("Build")
.Does(()=>{
   Information("Running test task...");
   var settings = new DotNetCoreTestSettings
     {
         Configuration = configuration.Build(),
         NoBuild=true,
     };
   var projectFiles = GetFiles("./tests/**/*.csproj");
   foreach(var file in projectFiles)
   {
      DotNetCoreTest(file.FullPath, settings);
   }
});
Task("Version")
.Does(()=>{
   var version=GitVersion(new GitVersionSettings{
      UpdateAssemblyInfo=false
   });
   var asmVer=XmlPeek(mainProjectFile.Build(),"/Project/PropertyGroup/AssemblyVersion");
   var fileVer=XmlPeek(mainProjectFile.Build(),"/Project/PropertyGroup/FileVersion");
   var ver=XmlPeek(mainProjectFile.Build(),"/Project/PropertyGroup/Version");
   Information($"Got versions {asmVer}, {fileVer} and {ver}");
   Information($"Replacing with {version.AssemblySemVer}, {version.AssemblySemFileVer} and {version.MajorMinorPatch}");
   XmlPoke(mainProjectFile.Build(),"/Project/PropertyGroup/AssemblyVersion",version.AssemblySemVer);
   XmlPoke(mainProjectFile.Build(),"/Project/PropertyGroup/FileVersion",version.AssemblySemFileVer);
   XmlPoke(mainProjectFile.Build(),"/Project/PropertyGroup/Version",version.MajorMinorPatch);
});
Task("Default")
.IsDependentOn("Test")
.Does(()=>{
   Information("Running default task...");
});


#endregion //TASKS

#region MISC
Task("Help")
.Does(()=>{
   Information($"Showing info for version {GitVersion().FullSemVer}");
   Information(env.GetHelp());
   });

#endregion //MISC




RunTarget(target.Build());