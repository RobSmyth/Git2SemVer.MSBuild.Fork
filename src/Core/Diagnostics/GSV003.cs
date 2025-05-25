// ReSharper disable InconsistentNaming
namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public class GSV003 : DiagnosticCodeBase
{
    public GSV003(string buildScriptPath)
        : base(id: 3,
               description: $"""
                             This occurs when ...
                             """,
               resolution: $"""
                            If there ...
                            """,
               message: $"The required build script file '{buildScriptPath}' was not found.")
    {
    }
}