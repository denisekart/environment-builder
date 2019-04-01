//#addin nuget:?package=Cake.Json&version=3.0.1
#addin nuget:?package=Newtonsoft.Json&version=11.0.1
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class ArgBuilder : IArgBuilderBundle{

    private bool isBundled=false;    
    private readonly string environmentVariablePrefix;
    private readonly string configurationFileName;
    private readonly JObject configurationContent;
    private readonly ICakeContext context;
    private string latestValue=string.Empty;
    private readonly IList<string> argumentParserDescriptions=new List<string>();
    private readonly IList<Func<object>> argumentParsers=new List<Func<object>>();
    private readonly IList<string> helpLines=new List<string>();
    private readonly IList<IArgBuilderBundle> bundles=new List<IArgBuilderBundle>();
    private bool bundledResultSet=false;
    private object bundledResult=null;

    private string description=string.Empty;

    private void CreateEntry(Func<object> parser, string description){
        argumentParsers.Add(parser);
        argumentParserDescriptions.Add(description??"");
    }

    private void Clear(){
        argumentParsers.Clear();
        argumentParserDescriptions.Clear();
        helpLines.Clear();
        latestValue=string.Empty;
    }

    private ArgBuilder(ArgBuilder bundle){
        isBundled=true;
        environmentVariablePrefix=bundle.environmentVariablePrefix;
        configurationFileName=bundle.configurationFileName;
        configurationContent=bundle.configurationContent;
        context=bundle.context;
        argumentParserDescriptions=new List<string>(bundle.argumentParserDescriptions);
        argumentParsers=new List<Func<object>>(bundle.argumentParsers);
        helpLines=new List<string>(bundle.helpLines);
    }
    public ArgBuilder(ICakeContext context,string environmentVariablePrefix):this(context){
        this.environmentVariablePrefix = environmentVariablePrefix;
        context.Verbose($"With environmentVariablePrefix '{environmentVariablePrefix}'");
    }
    public ArgBuilder(ICakeContext context,string environmentVariablePrefix,string configurationFileName):this(context,environmentVariablePrefix){
        if(context.FileExists(context.File(configurationFileName))){
            this.configurationFileName = configurationFileName;
            context.Verbose($"With configurationFileName '{configurationFileName}'");
            
            configurationContent=JObject.Parse(System.IO.File.ReadAllText(context.File(configurationFileName)));
        }else{
            context.Verbose($"File with configurationFileName '{configurationFileName}' was not found. Skipping setup.");
        }
    }
    public ArgBuilder(ICakeContext context){
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        this.context = context;
        context.Verbose($"Building ArgBuilder");
    }
    
    public ArgBuilder With(string name){
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("message", nameof(name));
        }
        latestValue=name;
        return this;
    }
    public ArgBuilder Arg(){
        return Arg(latestValue);
    }
    public ArgBuilder Arg(string argumentName){
        if (string.IsNullOrWhiteSpace(argumentName))
        {
            throw new ArgumentException("message", nameof(argumentName));
        }
        context.Debug($"With argument '{argumentName}' at priority {argumentParsers.Count}");

        CreateEntry(()=>context.HasArgument(argumentName)?context.Argument<string>(argumentName):null,
        $"(arg '{argumentName}')");

        latestValue=argumentName;
        return this;
    }
    public ArgBuilder Env(){
        return Env(latestValue);
    }
    public ArgBuilder Env(bool usePrefix){
        return Env(latestValue,usePrefix);
    }
    public ArgBuilder Env(string environmentVariableName,bool usePrefix=true){
        if (string.IsNullOrWhiteSpace(environmentVariableName))
        {
            throw new ArgumentException("message", nameof(environmentVariableName));
        }
        context.Debug($"With environment variable '{environmentVariableName}' (with prefix '{environmentVariablePrefix??"none"}') at priority {argumentParsers.Count}");
        var variableName=environmentVariableName;
        if(usePrefix)
            variableName=(environmentVariablePrefix??"")+variableName;

        CreateEntry(()=>{
            context.Verbose($"Found environment variable with name {variableName}={context.HasEnvironmentVariable(variableName)}");
            return context.HasEnvironmentVariable(variableName)?(context.EnvironmentVariable(variableName)):null;
            },
            $"(env '{variableName}')");
        latestValue=environmentVariableName;
        return this;
    }
    public ArgBuilder File(){
        return File(latestValue);
    }
    public ArgBuilder File(string configurationFileEntry){
        if (string.IsNullOrWhiteSpace(configurationFileEntry))
        {
            throw new ArgumentException("message", nameof(configurationFileEntry));
        }

        CreateEntry(()=>{
            if(string.IsNullOrWhiteSpace(configurationFileName) || configurationContent == null){
                context.Verbose("Skipping configuration entry because the file was not initialized");
                return null;
            }
            context.Verbose($"Parsing entry '{configurationFileEntry}' from file '{configurationFileName}'");
            var parts=configurationFileEntry.Split(new []{'.'},StringSplitOptions.RemoveEmptyEntries);
            context.Verbose($"Traversing {parts.Length} JSON nodes");
            JToken root=configurationContent;
            foreach (var part in parts)
            {
                if(root==null)
                    break;
                context.Verbose($"At part '{part}' in JSON tree");
                root=root?[part];
                
            }
            context.Verbose($"Found value {root??"NULL"} at leaf node");
            return root?.ToString();
        },
        $"(file '{configurationFileEntry}')");

        context.Debug($"With configuration file entry '{configurationFileEntry}' (with configuration file '{configurationFileName}') at priority {argumentParsers.Count}");
        latestValue=configurationFileEntry;
        return this;
    }
    public ArgBuilder Func<T>(Func<T> function,string description=null){
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }
        CreateEntry(
            ()=>function(),
            description??"(custom)");

        return this;
    }
    public ArgBuilder Default<T>(T defaultValue){

        CreateEntry(()=>defaultValue,
        $"(default '{defaultValue}')");

        context.Debug($"With default value '{defaultValue}' at priority {argumentParsers.Count}");
        return this;
    }
    public ArgBuilder Throw(string message=null,bool trace=true){
        var traceMessage=trace?("["+Trace()+"]"):string.Empty;
        CreateEntry(
            ()=>throw new Exception((message??$"Configuration value is missing")+traceMessage),
            $"(throw '{message??"empty"}')");

        context.Debug($"With throw '{message??"NO MESSAGE"}' at priority {argumentParsers.Count}");
        return this;
    }

    public ArgBuilder Help(string line=null, bool lineIsTrace=false){
        helpLines.Add((line??string.Empty)+" "+(lineIsTrace?Trace():string.Empty));
        return this;
    }
    public string Help(){
        if(isBundled)
            return helpLines.Count==0?string.Empty:string.Join(Environment.NewLine,helpLines);
        else
            return (string.IsNullOrWhiteSpace(description)
            ?string.Empty
            :string.Format("{0}{1}",description,Environment.NewLine)) 
            +string.Join(Environment.NewLine+Environment.NewLine,bundles.Select(b=>b.Help()).Where(s=>!string.IsNullOrWhiteSpace(s)));
    }

    public T Build<T>(){
        if(isBundled && bundledResultSet)
            if(bundledResult==null)
                return default(T);
            else
                return (T)bundledResult;

        T ret=default(T);
        int index=0;

        foreach (var parser in argumentParsers)
        {
            context.Verbose($"Parsing value at position {index}");
            var value=parser();
            if(value is  T v){
                ret=v;
                break;
            }
            else if(Convert.ChangeType(value, typeof(T)) is T v2){
                ret=v2;
                break;
            }
            index++;
        }
        if(!isBundled)
            Clear();

        context.Verbose($"Built argument tree for type '{typeof(T).FullName}' with result '{ret}' from parser at position {index}");
        bundledResultSet=true;
        bundledResult=ret;
        return ret;
    }
    public string Build(){
        return Build<string>()?.ToString();
    }
    public ArgBuilder WithDescription(string description){
        if(string.IsNullOrWhiteSpace(this.description))
            this.description=description;
        else
            this.description=string.Format("{0}{1}{2}",
            this.description,
            Environment.NewLine,
            description);
        return this;
    }
    public ArgBuilder WithTaskDescriptions(params ICakeTaskInfo[] tasks){
        context.Verbose($"Building descriptions for {tasks?.Count()??0} tasks");
        var member=string.Join(Environment.NewLine,tasks.Select(x=>$"{x.Name}\t-{x.Description}\r\n\t\tDependsOn:[{string.Join(", ",x?.Dependencies.Select(d=>d.Name+$"[{(d.Required?"Required":"Optional")}]"))}]"));
        WithDescription(member);
        return this;
    }
    public string Trace(){
        if(argumentParserDescriptions.Count==0) return string.Empty;
        return string.Join(", ",argumentParserDescriptions);
    }

    public IArgBuilderBundle Bundle(){
        
        var bundle= new ArgBuilder(this);
        bundles.Add(bundle);
        Clear();
        return bundle;
    }
}

public interface IArgBuilderBundle{
    string Trace();
    string Build();
    string Help();
    T Build<T>();

}