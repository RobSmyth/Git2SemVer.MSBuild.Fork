using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class NextReleaseVersionFinder
{
    private readonly IReadOnlyList<VersionHistorySegment> _segments;
    private readonly ILogger _logger;
    private readonly Dictionary<CommitId, VersionHistorySegment> _segmentsByYoungestCommit;

    public NextReleaseVersionFinder(IReadOnlyList<VersionHistorySegment> segments, ILogger logger)
    {
        _segments = segments;
        _logger = logger;
        _segmentsByYoungestCommit = segments.ToDictionary(k => k.YoungestCommit.CommitId, v => v);
    }

    public SemVersion Find(Commit head)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Aggregating segments from prior releases.");

        var highestVersion = new SemVersion(0, 1, 0);
        using (_logger.EnterLogScope())
        {
            var segmentAggregators = new Dictionary<CommitId, SegmentAggregator>();
            AggregateParentSegments(segmentAggregators, CreateSegmentAggregator(segmentAggregators, head.CommitId));

            _logger.LogDebug("Commit      Bumps       From -> To");

            var releaseSegments = _segments.Where(x => x.ParentCommits.Count == 0 ||
                                                       x.TaggedReleasedVersion != null).ToList();
            foreach (var releaseSegment in releaseSegments)
            {
                var aggregator = segmentAggregators[releaseSegment.YoungestCommit.CommitId];
                var apiChanges = aggregator.ApiChanges;
                var startingVersion = releaseSegment.TaggedReleasedVersion ?? new SemVersion(0, 1, 0);
                var nextRelease = (releaseSegment.TaggedReleasedVersion == null && !apiChanges.Flags.Any) ?
                    startingVersion : startingVersion.Bump(apiChanges.Flags);

                _logger.LogDebug($"{aggregator.OldestCommitId.ShortSha,-12} {apiChanges.Flags}  {startingVersion,10} -> {nextRelease,-10}");

                if (nextRelease.ComparePrecedenceTo(highestVersion) > 0)
                {
                    highestVersion = nextRelease;
                }
            }
        }

        stopwatch.Stop();
        _logger.LogInfo("Found next release version will be {0} (in {1:F0} ms).", highestVersion, stopwatch.Elapsed.TotalMilliseconds);
        return highestVersion;
    }

    private void AggregateParentSegments(Dictionary<CommitId, SegmentAggregator> segmentAggregators, SegmentAggregator aggregator)
    {
        var segment = _segmentsByYoungestCommit[aggregator.YoungestCommitId];
        if (segment.IsAReleaseSegment)
        {
            return;
        }

        foreach (var parentCommitId in segment.ParentCommits)
        {
            if (segmentAggregators.ContainsKey(parentCommitId))
            {
                continue;
            }

            var parentAggregator = CreateSegmentAggregator(segmentAggregators, parentCommitId);
            parentAggregator.AddChild(aggregator);

            AggregateParentSegments(segmentAggregators, parentAggregator);
        }
    }

    private SegmentAggregator CreateSegmentAggregator(Dictionary<CommitId, SegmentAggregator> segmentAggregators, CommitId youngestCommitId)
    {
        var aggregator = new SegmentAggregator(_segmentsByYoungestCommit[youngestCommitId]);
        segmentAggregators.Add(youngestCommitId, aggregator);
        return aggregator;
    }
}