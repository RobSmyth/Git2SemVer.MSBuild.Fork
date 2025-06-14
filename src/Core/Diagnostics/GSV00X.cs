namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public class GSV00X : DiagnosticCodeBase
{
    public GSV00X(string buildScriptPath)
        : base(id: 9999,
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