using System.Diagnostics;
using System.Text;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class GitSegmentsWalker
{
    private readonly Commit _head;
    private readonly ILogger _logger;
    private readonly IReadOnlyList<GitSegment> _contributingCommits;
    private readonly Dictionary<CommitId, GitSegment> _segmentsByYoungestCommit;

    public GitSegmentsWalker(Commit head, ContributingCommits contributingCommits, ILogger logger)
    {
        _head = head;
        _contributingCommits = contributingCommits.Segments;
        _logger = logger;
        _segmentsByYoungestCommit = contributingCommits.Segments.ToDictionary(k => k.YoungestCommit.CommitId, v => v);
    }

    public SemanticVersionCalcResult CalculateSemVer()
    {
        var stopwatch = Stopwatch.StartNew();

        var linkedSegments = BuildLinkedSegments(_head);

        var result = new SemanticVersionCalcResult
        {
            HeadCommitId = _head.CommitId
        };

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("    Commit      Bumps       From -> To");
        var releasedSegments = _contributingCommits.Where(x => x.IsAReleaseSegment).ToList();
        foreach (var releaseSegment in releasedSegments)
        {
            var youngestLinkedSegment = linkedSegments[releaseSegment.YoungestCommit.CommitId];
            var aggregatedResult = new SegmentsAggregator().Aggregate(_head, youngestLinkedSegment);

            if (_logger.IsLogging(LoggingLevel.Debug))
            {
                stringBuilder.AppendLine($"    {youngestLinkedSegment.OldestCommitId.ShortSha,-12} {aggregatedResult.ChangeFlags}  {aggregatedResult.PriorVersion,10} -> {aggregatedResult.Version,-10}");
            }

            if (aggregatedResult.Version.ComparePrecedenceTo(result.Version) <= 0)
            {
                continue;
            }

            result.Version = aggregatedResult.Version;
            result.PriorReleaseCommitId = releaseSegment.OldestCommit.CommitId;
            result.PriorReleaseVersion = aggregatedResult.PriorVersion;
            result.ChangeFlags = aggregatedResult.ChangeFlags;
        }

        stopwatch.Stop();
        _logger.LogDebug("Found {0} prior releases reachable from head {1} (in {2:F0} ms):\n{3}",
                         releasedSegments.Count.ToString(),
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