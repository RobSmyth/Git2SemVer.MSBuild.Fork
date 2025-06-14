// ReSharper disable ClassNeverInstantiated.Global
namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public class GSV004 : DiagnosticCodeBase
{
    public GSV004(string buildScriptPath)
        : base(id: 4,
               description: $"""
                             This occurs when the build property `BuildScriptPath` (`{buildScriptPath}`) is not a valid path 
                             and the property RunScript is `true`.
                             """,
               resolution: $"""
                            If there is a C# script to run, correct the property to script's path.
                            Otherwise set RunScript to `false`.
                            """,
               message: $"The script file not found and is required.")
    {
    }
}