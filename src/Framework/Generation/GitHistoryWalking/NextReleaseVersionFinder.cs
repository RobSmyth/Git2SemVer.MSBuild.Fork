using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class NextReleaseVersionFinder
{
    private readonly ILogger _logger;
    private readonly IReadOnlyList<VersionHistorySegment> _segments;
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
            var linkedSegments = BuildLinkedSegments(head);

            _logger.LogDebug("Commit      Bumps       From -> To");
            var releaseSegments = _segments.Where(x => x.IsAReleaseSegment).ToList();
            foreach (var releaseSegment in releaseSegments)
            {
                var changeAggregator = new ChangeFlagsAggregator();

                var linkedReleaseSegment = linkedSegments[releaseSegment.YoungestCommit.CommitId];
                var changeFlags = changeAggregator.Aggregate(linkedReleaseSegment);

                var startingVersion = releaseSegment.TaggedReleasedVersion ?? new SemVersion(0, 1, 0);
                var nextRelease = releaseSegment.TaggedReleasedVersion == null && !changeFlags.Any
                    ? startingVersion
                    : startingVersion.Bump(changeFlags);

                _logger.LogDebug($"{linkedReleaseSegment.OldestCommitId.ShortSha,-12} {changeAggregator}  {startingVersion,10} -> {nextRelease,-10}");

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

    private Dictionary<CommitId, LinkedSegment> BuildLinkedSegments(Commit head)
    {
        var linkedSegments = new Dictionary<CommitId, LinkedSegment>();
        BuildLinkedSegments(linkedSegments, CreateLinkedSegment(linkedSegments, head.CommitId));
        return linkedSegments;
    }

    private void BuildLinkedSegments(Dictionary<CommitId, LinkedSegment> linkedSegments, LinkedSegment linkedSegment)
    {
        var segment = _segmentsByYoungestCommit[linkedSegment.YoungestCommitId];
        if (segment.IsAReleaseSegment)
        {
            return;
        }

        foreach (var parentCommitId in segment.ParentCommits)
        {
            if (linkedSegments.ContainsKey(parentCommitId))
            {
                continue;
            }

            var linkedParentSegment = CreateLinkedSegment(linkedSegments, parentCommitId);
            linkedParentSegment.AddChild(linkedSegment);

            BuildLinkedSegments(linkedSegments, linkedParentSegment);
        }
    }

    private LinkedSegment CreateLinkedSegment(Dictionary<CommitId, LinkedSegment> linkedSegments, CommitId youngestCommitId)
    {
        var linkedSegment = new LinkedSegment(_segmentsByYoungestCommit[youngestCommitId]);
        linkedSegments.Add(youngestCommitId, linkedSegment);
        return linkedSegment;
    }
}