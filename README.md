##Available target frameworks
https://docs.microsoft.com/en-us/dotnet/standard/frameworks

##Setup
https://weblog.west-wind.com/posts/2017/jun/22/multitargeting-and-porting-a-net-library-to-net-core-20
https://github.com/RickStrahl/Westwind.Utilities/blob/master/Westwind.Utilities/Westwind.Utilities.csproj



.NET project directory structure guidelines

https://gist.github.com/davidfowl/ed7564297c61fe9ab814

--GIST
-------------------------------------------
$/
  artifacts/
  build/
  docs/
  lib/
  packages/
  samples/
  src/
  tests/
  .editorconfig
  .gitignore
  .gitattributes
  build.cmd
  build.sh
  LICENSE
  NuGet.Config
  README.md
  {solution}.sln
---------------------------------------------

src - Main projects (the product code)
tests - Test projects
docs - Documentation stuff, markdown files, help files etc.
samples (optional) - Sample projects
lib - Things that can NEVER exist in a nuget package
artifacts - Build outputs go here. Doing a build.cmd/build.sh generates artifacts here (nupkgs, dlls, pdbs, etc.)
packages - NuGet packages
build - Build customizations (custom msbuild files/psake/fake/albacore/etc) scripts
build.cmd - Bootstrap the build for windows
build.sh - Bootstrap the build for *nix
global.json - ASP.NET vNext only

.gitignore
also see : 
https://github.com/github/gitignore 
https://github.com/github/gitignore/blob/master/VisualStudio.gitignore
https://github.com/angular/angular/blob/master/.gitignore

-------------------------------------------------
[Oo]bj/
[Bb]in/
.nuget/
_ReSharper.*
packages/
artifacts/
*.user
*.suo
*.userprefs
*DS_Store
*.sln.ide
-------------------------------------------------
There's probably more things that go in the ignore file.

Update: Added docs folder
Added README.md and LICENSE - Critical if you're OSS, if not ignore it
Renamed test to tests
Added lib for things that CANNOT exist in nuget packages
Removed NuGet.config for people using packet :)
Added global.json for ASP.NET vnext
Added .editorconfig file in the root (x-plat IDE settings)
Added NuGet.config back because people were confused about it missing