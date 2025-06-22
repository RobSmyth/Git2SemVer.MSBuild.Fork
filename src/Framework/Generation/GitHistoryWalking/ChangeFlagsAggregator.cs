using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class ChangeFlagsAggregator
{
    private readonly ApiChangeFlags Flags = new();
    private readonly HashSet<LinkedSegment> _aggregatedSegments = [];

    public ApiChangeFlags Aggregate(LinkedSegment linkedSegment)
    {
        if (!_aggregatedSegments.Add(linkedSegment))
        {
            return Flags;
        }

        Flags.Aggregate(linkedSegment.ChangeFlags);
        foreach (var linkedChildSegment in linkedSegment.LinkedChildSegments)
        {
            Aggregate(linkedChildSegment);
        }

        return Flags;
    }
}