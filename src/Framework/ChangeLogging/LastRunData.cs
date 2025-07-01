namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public sealed class LastRunData
{
    public string CommitSha { get; set; } = "";

    public DateTimeOffset CommitWhen { get; set; } = DateTimeOffset.MinValue;

    public string SemVersion { get; set; } = "";
}