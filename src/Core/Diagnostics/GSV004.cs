// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public sealed class GSV004 : DiagnosticCodeBase
{
    public GSV004(string buildScriptPath)
        : base(id: 4,
               subcategory: "Versioning",
               description: """
                            This occurs when the build property `BuildScriptPath` (`{0}`) is not a valid path 
                            and `RunScript` is `true`.
                            """,
               resolution: """
                           If there is a C# script to run, correct the property to script's path.
                           
                           Otherwise set RunScript to `false` by add the following
                           to the solution's `Directory.Build.props` (if it exists) or the project's file.
                           
                           ```xml
                           <PropertyGroup>
                             <Git2SemVer_RunScript>false</Git2SemVer_RunScript>
                           </PropertyGroup>
                           ```
                           """,
               message: "The script file not found and is required.",
               messageArgs: buildScriptPath)
    {
    }
}