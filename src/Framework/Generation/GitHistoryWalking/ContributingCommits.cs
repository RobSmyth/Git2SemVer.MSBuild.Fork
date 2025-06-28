namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class ContributingCommits
{
    public ContributingCommits(IReadOnlyList<GitSegment> segments)
    {
        Segments = segments;
    }

    public IReadOnlyList<GitSegment> Segments { get; }
}