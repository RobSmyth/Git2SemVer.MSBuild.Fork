// ReSharper disable InconsistentNaming
namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public sealed class GSV002 : DiagnosticCodeBase
{
    public GSV002()
        : base(id: 2,
               subcategory: "Versioning",
               description: $"""
                             This occurs when the build property `Git2SemVer_ScriptPath` is null or whitespaces and the property `Git2SemVer_RunScript` is not `false`.
                             """,
               resolution: $"""
                            If there is a C# script to run, set the property to script's path build property `Git2SemVer_ScriptPath`.
                            
                            Otherwise set `Git2SemVer_RunScript` to `false` by add the following
                            to the solution's `Directory.Build.props` (if it exists) or the project's file.
                            
                            ```xml
                            <PropertyGroup>
                              <Git2SemVer_RunScript>false</Git2SemVer_RunScript>
                            </PropertyGroup>
                            ```
                            """,
               message: $"The script file path build property BuildScriptPath is required.")
    {
    }
}