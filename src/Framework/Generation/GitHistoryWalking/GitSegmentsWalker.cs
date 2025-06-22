using System.Diagnostics;
using System.Text;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class GitSegmentsWalker
{
    private readonly Commit _head;
    private readonly ILogger _logger;
    private readonly IReadOnlyList<GitSegment> _segments;
    private readonly Dictionary<CommitId, GitSegment> _segmentsByYoungestCommit;

    public GitSegmentsWalker(Commit head, IReadOnlyList<GitSegment> segments, ILogger logger)
    {
        _head = head;
        _segments = segments;
        _logger = logger;
        _segmentsByYoungestCommit = segments.ToDictionary(k => k.YoungestCommit.CommitId, v => v);
    }

    public SemanticVersionCalcResult CalculateSemVer()
    {
        var stopwatch = Stopwatch.StartNew();

        if (_head.HasReleaseTag)
        {
            _logger.LogDebug("Head has a release tag. Tagged version will be used.");
            var headReleaseVersion = _head.ReleasedVersion ?? new SemVersion(0, 1, 0);
            return new SemanticVersionCalcResult
            {
                HeadCommitId = _head.CommitId,
                Version = headReleaseVersion,
                PriorReleaseCommitId = _head.CommitId,
                PriorReleaseVersion = headReleaseVersion
            };
        }

        var result = new SemanticVersionCalcResult
        {
            HeadCommitId = _head.CommitId
        };

        var linkedSegments = BuildLinkedSegments(_head);

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("    Commit      Bumps       From -> To");
        var releaseSegments = _segments.Where(x => x.IsAReleaseSegment).ToList();
        foreach (var releaseSegment in releaseSegments)
        {
            var changesAggregator = new ChangeFlagsAggregator();

            var linkedReleaseSegment = linkedSegments[releaseSegment.YoungestCommit.CommitId];
            var changeFlags = changesAggregator.Aggregate(linkedReleaseSegment);

            var priorReleaseVersion = releaseSegment.TaggedReleasedVersion ?? new SemVersion(0, 1, 0);
            var nextRelease = releaseSegment.TaggedReleasedVersion == null && !changeFlags.Any
                ? priorReleaseVersion
                : priorReleaseVersion.Bump(changeFlags);

            stringBuilder.AppendLine($"    {linkedReleaseSegment.OldestCommitId.ShortSha,-12} {changeFlags}  {priorReleaseVersion,10} -> {nextRelease,-10}");

            if (nextRelease.ComparePrecedenceTo(result.Version) > 0)
            {
                result.Version = nextRelease;
                result.PriorReleaseCommitId = releaseSegment.OldestCommit.CommitId;
                result.PriorReleaseVersion = priorReleaseVersion;
                result.ChangeFlags = changeFlags;
            }
        }

        stopwatch.Stop();
        _logger.LogDebug("Found {0} prior released commits reachable from head {1} ((in {2:F0} ms):\n{3}",
                         releaseSegments.Count.ToString(),
                         _head.CommitId.ShortSha,
                         stopwatch.ElapsedMilliseconds,
                         stringBuilder.ToString().TrimEnd());

        return result;
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