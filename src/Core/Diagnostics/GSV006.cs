namespace NoeticTools.Git2SemVer.Core.Diagnostics;

[DiagnosticCode]
public sealed class GSV006 : DiagnosticCodeBase
{
    public GSV006(string tagFormat)
        : base(6,
               "Versioning",
               """
               The occurs when the build property `Git2SemVer_ReleaseTagFormat`'s value is missing the required placeholder text: ``%VERSION%``.

               The placeholder text will be replaced with the regular expression `(?<version>\d+\.\d+\.\d+)` to read any tag version number.

               For example:
               ```xml
               <PropertyGroup>
                 <Git2SemVer_ReleaseTagFormat>MyRelease %VERSION%</Git2SemVer_ReleaseTagFormat>
               </PropertyGroup>
               ```
               """,
               "The build property `Git2SemVer_ReleaseTagFormat` value `{0}` must include the placeholder `%VERSION%` text.",
               tagFormat)

    {
    }
}