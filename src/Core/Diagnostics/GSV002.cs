// ReSharper disable InconsistentNaming
namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public class GSV002 : DiagnosticCodeBase
{
    public GSV002()
        : base(id: 2,
               description: $"""
                             This occurs when the build property BuildScriptPath is null or whitespaces and the property RunScript is not `false`.
                             """,
               resolution: $"""
                            If there is a C# script to run, set the property to script's path build property BuildScriptPath.
                            Otherwise set RunScript to `false`.
                            """,
               message: $"The script file path build property BuildScriptPath is required.")
    {
    }
}