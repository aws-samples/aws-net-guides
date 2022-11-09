namespace AspNetCoreWebApiRds;

public class ApplicationOptions
{
    public Secret Secret { get; set; }
    public Database Database { get; set; }    
}

public class Secret
{
    public string Name { get; set; } 
    public string VersionStage { get; set; }    
    public string Region { get; set; }      
}

public class Database
{
    public string Name { get; set; }
    public bool MultipleActiveResultSets { get; set; }
}