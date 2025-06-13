using System.Diagnostics;
using System.Text;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class VersionHistorySegmentsBuilder
{
    private readonly Dictionary<CommitId, VersionHistorySegment> _commitsCache = new();
    private readonly IGitTool _gitTool;
    private readonly ILogger _logger;
    private readonly VersionHistorySegment _segment;
    private readonly IVersionHistorySegmentFactory _segmentFactory;
    private readonly Dictionary<int, VersionHistorySegment> _segments = [];

    private VersionHistorySegmentsBuilder(VersionHistorySegment segment, VersionHistorySegmentsBuilder parent)
    {
        _logger = parent._logger;
        _segments = parent._segments;
        _segmentFactory = parent._segmentFactory;
        _gitTool = parent._gitTool;
        _commitsCache = parent._commitsCache;
        _segment = segment;
        _segments.Add(segment.Id, segment);
    }

    public VersionHistorySegmentsBuilder(IGitTool gitTool, ILogger logger)
    {
        _gitTool = gitTool;
        _logger = logger;
        _segmentFactory = new VersionHistorySegmentFactory(logger);
        _segment = _segmentFactory.Create();
        _segments.Add(_segment.Id, _segment);
    }

    public IReadOnlyList<VersionHistorySegment> BuildTo(Commit commit)
    {
        FindPathSegmentsReachableFrom(commit);
        return _segments.Values.ToList();
    }

    private void FindPathSegmentsReachableFrom(Commit commit)
    {
        var stopwatch = Stopwatch.StartNew();
        while (NextCommit(commit) == SegmentWalkResult.Continue)
        {
            commit = _gitTool.Get(commit.Parents.First());
        }

        stopwatch.Stop();
        _logger.LogDebug(GetFoundSegmentsReport(stopwatch.Elapsed));
    }

    private string GetFoundSegmentsReport(TimeSpan timeTaken)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Found {_segments.Count} segment{(_segments.Count == 1 ? "" : "s")} (in {timeTaken.TotalMilliseconds:F0} ms):");

        stringBuilder.AppendLine("  Segment #      From -> To      Commits    Bumps  Release");
        foreach (var segment in _segments)
        {
            stringBuilder.AppendLine("  " + segment.Value);
        }

        return stringBuilder.ToString().TrimEnd();
    }

    private SegmentWalkResult NextCommit(Commit commit)
    {
        if (_commitsCache.ContainsKey(commit.CommitId))
        {
            OnBranchFromExistingSegment(commit);
            return SegmentWalkResult.FoundStart;
        }

        _segment.Append(commit);
        _commitsCache.Add(commit.CommitId, _segment);

        if (commit.ReleasedVersion != null)
        {
            _logger.LogTrace("Found release {1} at Commit {0}.",
                             commit.CommitId.ShortSha,
                             commit.ReleasedVersion.ToString());

            return SegmentWalkResult.FoundStart;
        }

        var parents = commit.Parents.ToList();
        if (parents.Count == 0)
        {
            _logger.LogTrace("Found commits path to the repository's first commit ({0}) that is reachable from the head commit without a release.",
                             commit.CommitId.ShortSha);
            return SegmentWalkResult.FoundStart;
        }

        if (parents.Count != 2)
        {
            return SegmentWalkResult.Continue;
        }

        OnMergeCommit(commit);
        return SegmentWalkResult.FoundStart;
    }

    private void NextCommitBeforeMerge(CommitId branchCommitId)
    {
        var parentCommit = _gitTool.Get(branchCommitId);

        if (_commitsCache.ContainsKey(parentCommit.CommitId))
        {
            OnBranchFromExistingSegment(parentCommit);
        }
        else
        {
            using (_logger.EnterLogScope())
            {
                var newSegmentVisitor = new VersionHistorySegmentsBuilder(_segmentFactory.Create([]), this);
                newSegmentVisitor.FindPathSegmentsReachableFrom(parentCommit);
            }
        }
    }

    private void OnBranchFromExistingSegment(Commit commit)
    {
        var intersectingSegment = _commitsCache[commit.CommitId];
        var branchedFromSegment = intersectingSegment.BranchesFrom(_segment, commit, _segmentFactory);
        if (branchedFromSegment == null)
        {
            return;
        }

        _segments.Add(branchedFromSegment.Id, branchedFromSegment);
        foreach (var segmentCommit in branchedFromSegment.Commits)
        {
            _commitsCache[segmentCommit.CommitId] = branchedFromSegment;
        }
    }

    private void OnMergeCommit(Commit mergeCommit)
    {
        var mergedBranchCommit = mergeCommit.Parents[0];
        var continuingBranchCommit = mergeCommit.Parents[1];
        using (_logger.EnterLogScope())
        {
            _logger.LogTrace($"Commit {mergeCommit.CommitId.ShortSha} is a merge commit.");
        }

        _logger.LogTrace("Continuing branch:");
        NextCommitBeforeMerge(continuingBranchCommit);

        _logger.LogTrace($"Commit {mergeCommit.CommitId.ShortSha} is a merge commit from branch commit {mergedBranchCommit.ShortSha}:");
        using (_logger.EnterLogScope())
        {
            NextCommitBeforeMerge(mergedBranchCommit);
        }
    }

    private enum SegmentWalkResult
    {
        Continue,
        FoundStart
    }
}