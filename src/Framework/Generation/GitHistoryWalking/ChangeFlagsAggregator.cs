using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class ChangeFlagsAggregator
{
    private readonly HashSet<LinkedSegment> _aggregatedSegments = [];
    private ApiChangeFlags _flags = new();

    public ApiChangeFlags Aggregate(LinkedSegment linkedSegment)
    {
        if (!_aggregatedSegments.Add(linkedSegment))
        {
            return _flags;
        }

        _flags = _flags.Aggregate(linkedSegment.ChangeFlags);
        foreach (var linkedChildSegment in linkedSegment.LinkedChildSegments)
        {
            Aggregate(linkedChildSegment);
        }

        return _flags;
    }
}