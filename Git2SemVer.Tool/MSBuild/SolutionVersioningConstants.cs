using Spectre.Console;


namespace NoeticTools.Git2SemVer.Tool.MSBuild;

internal static class SolutionVersioningConstants
{
    public const string DefaultVersioningProjectName = "SolutionVersioningProject";
    public const string DirectoryBuildPropsFilename = "Directory.Build.props";
    public const string DirectoryVersionPropsFilename = "Directory.Versioning.Build.props";
}