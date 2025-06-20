namespace NoeticTools.Git2SemVer.Tool.Commands.Add;

internal readonly struct UserOptions(string leadingProjectName)
{
    public string VersioningProjectName { get; } = leadingProjectName;
}