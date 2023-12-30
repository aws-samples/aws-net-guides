using System.Collections.Generic;

namespace ServerlessDocProcessing.Lambda;

public class CustomFunctionProps
{

    // Static Properties used across functions
    public static int GlobalTimeout { get; set; } = 30;

    public static int GlobalMemory { get; set; } = 1024;

    public string ResourcePrefix { get; set; } = "docProcessing";

    // Default to using FunctionHandler as the Annotations Handler Environment. We will override this as needed
    public static Dictionary<string, string> GlobalEnvironment { get; } = new()
    {
        { "ANNOTATIONS_HANDLER", "FunctionHandler" }
    };

    public static string EnvironmentName { get; set; }

    public static string FunctionBaseDirectory { get; set; }

    // Per-Instance properties

    public int? Memory { get; set; }
    public int? Timeout { get; set; }
    public string BuildMethod { get; set; }
    public string FunctionNameBase { get; set; }
    public string CodeBaseDirectory { get; set; }
    public string FunctionCodeDirectory { get; set; }
    public string Description { get; set; }
    public string FunctionName => $"{ResourcePrefix}{FunctionNameBase}{EnvironmentName}";

    public Amazon.CDK.AWS.Lambda.FunctionProps FunctionProps => new()
    {
        Tracing = Tracing.ACTIVE,
        Handler = "bootstrap",
        FunctionName = FunctionName,
        Architecture = Architecture.X86_64,
        Runtime = Runtime.PROVIDED_AL2,
        Timeout = Duration.Seconds(Timeout ?? GlobalTimeout),
        MemorySize = Memory ?? GlobalMemory,
        Code = Code.FromAsset($"{FunctionBaseDirectory}/{FunctionCodeDirectory ?? FunctionNameBase}.zip"),
        Description = Description,
    };

    public static void AddGlobalEnvironment(string key, string value) => GlobalEnvironment[key] = value;



}
