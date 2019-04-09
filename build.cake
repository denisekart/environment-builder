#load "build/scripts/args.cake"


#region ARGUMENTS
var args=new ArgBuilder(Context,"cake_","build.json")
.Help("This is an utility for setting up the environment")
.Help("-------------------------------------------------");

var target = args
   .Arg("target").Arg("t")
   .Env("target").File("build.target")
   .Default("Default")
   .Help(null,true)
   .Help("\tThe target to execute")
   .Bundle();
var configuration=args
   .Arg("configuration").Arg("c")
   .Env("configuration").File("build.configuration")
   .Default("Debug")
   .Help(null, true)
   .Help("\tThe configuration to use")
   .Bundle();

var output=args
    .Arg("output").Arg("o")
    .Env("output").File("build.output")
    .Default("./artifacts")
    .Help(null,true)
    .Help("\tThe relative output folder")
    .Bundle();

var package=args
    .Arg("packageDirectory").Arg("p")
    .Env("packageDirectory").File("build.packageDirectory")
    .Default("./packages")
    .Help(null,true)
    .Help("\tThe relative output folder for deployments")
    .Bundle();

var mainProjectFile=args
   .Default("./src/EnvironmentBuilder/EnvironmentBuilder.csproj")
   .Bundle();
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
   });
Task("Restore")
   .Description("Runs the nuget restore task")
   .Does(()=>{
      Information("Running restore for all projects...");
      DotNetCoreRestore();
   });

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
.IsDependentOn("Version")
// .IsDependentOn("Build")
.Does(()=>{
   DotNetCorePack(mainProjectFile.Build(),new DotNetCorePackSettings{
      OutputDirectory=package.Build(),
      Configuration=configuration.Build()
   });
});

Task("Test")
.IsDependentOn("Build")
.Does(()=>{
   Information("Running test task...");
   var settings = new DotNetCoreTestSettings
     {
         Configuration = configuration.Build(),
         NoBuild=true
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
   XmlPoke(mainProjectFile.Build(),"/Project/PropertyGroup/AssemblyVersion",version.AssemblySemFileVer);
   XmlPoke(mainProjectFile.Build(),"/Project/PropertyGroup/AssemblyVersion",version.MajorMinorPatch);

});
Task("Default")
.Does(()=>{
   Information("Running default task...");
});

#endregion //TASKS

#region MISC
Task("Help")
.Does(()=>{
   Information(args.Help());
   });

#endregion //MISC




RunTarget(target.Build());