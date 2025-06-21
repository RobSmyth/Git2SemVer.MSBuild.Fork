using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

#pragma warning disable CS1591
internal sealed class SegmentAggregator(VersionHistorySegment segment)
{
    private readonly List<SegmentAggregator> _childAggregators = [];
    private ApiChanges? _apiChanges;

    /// <summary>
    ///     Aggregated API changes from this segment up to all younger segments.
    /// </summary>
    public ApiChanges ApiChanges => GetApiChanges();

    public CommitId YoungestCommitId => segment.YoungestCommit.CommitId;

    public CommitId OldestCommitId => segment.OldestCommit.CommitId;

    /// <summary>
    ///     Link to child (younger) segment.
    /// </summary>
    public void AddChild(SegmentAggregator childSegment)
    {
        if (_childAggregators.Contains(childSegment))
        {
            return;
        }

        if (_childAggregators.Count == 2)
        {
            throw new Git2SemVerInvalidOperationException("A history segment may not have more than 2 child segments.");
        }

        _childAggregators.Add(childSegment);
    }

    private ApiChanges GetApiChanges()
    {
        if (_apiChanges != null)
        {
            return _apiChanges;
        }

        _apiChanges = new ApiChanges();
        _apiChanges.Aggregate(segment.ApiChanges);

        foreach (var childAggregator in _childAggregators)
        {
            _apiChanges.Aggregate(childAggregator.ApiChanges);
        }

        return _apiChanges;
    }
}