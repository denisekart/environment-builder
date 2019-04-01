#load "build/scripts/args.cake"


#region ARGUMENTS
var args=new ArgBuilder(Context,"cake_","build.json")
.Help("This is an utility for setting up the environment")
.Help("-------------------------------------------------");

var target = args
   .Arg("target").Arg("t")
   .Env("target").File("build.target")
   .Default("Default")
   .Bundle().Build();



#endregion //ARGUMENTS

#region VARIABLES


#endregion //VARIABLES

#region TASKS
Task("Default")
.Does(()=>{
   Information("Default task...");
});

#endregion //TASKS

#region MISC
Task("Help")
.Does(()=>{
   Information(args.Help());
   });

#endregion //MISC




RunTarget(target);