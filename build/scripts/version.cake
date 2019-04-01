#tool nuget:?package=GitVersion.CommandLine&version=4.0.0
#addin nuget:?package=Cake.FileHelpers&version=3.1.0
//https://gitversion.readthedocs.io
public class VersionBuilder : IVersionBuilder {
    private readonly ICakeContext context;

    private GitVersion version;

    public VersionBuilder(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        this.context = context;

    }

    public VersionBuilder Build(){
        version=version??context.GitVersion(new GitVersionSettings{UpdateAssemblyInfo=false});
        return this;
    }

    public GitVersion Version(){
        if(version==null)
            throw new ArgumentNullException(nameof(version));
        return version;
    }

    public IVersionBuilder UpdateAssemblyInfo(){
        var assemblyVersion=version.AssemblySemVer;
        var assemblyFileVersion=version.AssemblySemFileVer;
        var assemblyInformationalVersion=version.InformationalVersion;
        
        context.ReplaceRegexInFiles("**AssemblyInfo.cs",
            "(?<=AssemblyVersion\\(\")(.+?)(?=\"\\))",
            assemblyVersion);

        context.ReplaceRegexInFiles("**AssemblyInfo.cs",
            "(?<=AssemblyInformationalVersion\\(\")(.+?)(?=\"\\))",
            assemblyInformationalVersion);

        context.ReplaceRegexInFiles("**AssemblyInfo.cs",
            "(?<=AssemblyFileVersion\\(\")(.+?)(?=\"\\))",
            assemblyFileVersion);

        return this;
    }
    public IVersionBuilder UpdateProjectManifest(){

        return this;
    }
    public IVersionBuilder UpdateNuspec(){

        return this;
    }

}

public interface IVersionBuilder{
    GitVersion Version();
    IVersionBuilder UpdateAssemblyInfo();
    IVersionBuilder UpdateProjectManifest();
    IVersionBuilder UpdateNuspec();
}