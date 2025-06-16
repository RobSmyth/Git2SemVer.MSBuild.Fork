using System.Text;
using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public sealed class GSV005 : DiagnosticCodeBase
{
    public GSV005(string tagFormat, string reservedPrefix)
        : base(5,
               "Versioning",
               "SET IN CONSTRUCTOR",
               """
               Correct the `Git2SemVer_ScriptPath` build property value to not use the reported prefix.

               The `Git2SemVer_ScriptPath` build property is set the project file or in a directory build properties file like `Directory.Build.props`.

               For example:
               ```xml
               <PropertyGroup>
                 <Git2SemVer_ReleaseTagFormat>MyRelease %VERSION%</Git2SemVer_ReleaseTagFormat>
               </PropertyGroup>
               ```
               """,
               "'{1}' is a reserved release tag format prefix an may not be used in the build property `Git2SemVer_ReleaseTagFormat` value `{0}`.",
               tagFormat, reservedPrefix)

    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("""
                             The occurs when the build property `Git2SemVer_ReleaseTagFormat` has a value that starts with a reserved prefix.

                             Reserved prefixes:
                             """);

        foreach (var keyValue in TagParser.ReservedPatternPrefixes)
        {
            stringBuilder.AppendLine($"* `{keyValue.Key}` - {keyValue.Value}.");
        }

        Description = stringBuilder.ToString();
    }
}