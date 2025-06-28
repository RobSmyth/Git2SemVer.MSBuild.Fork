﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class ContributingCommits
{
    private readonly LoadOnDemand<IReadOnlyList<LinkedSegment>> _leafSegments;
    private readonly LoadOnDemand<IReadOnlyList<LinkedSegment>> _segments;

    public ContributingCommits(IReadOnlyList<GitSegment> gitSegments, Commit head)
    {
        _segments = new LoadOnDemand<IReadOnlyList<LinkedSegment>>(() => BuildLinkedSegments(head, gitSegments));
        _leafSegments = new LoadOnDemand<IReadOnlyList<LinkedSegment>>(() => Segments.Where(x => x.Inner.IsAReleaseSegment).ToList());
        Commits = gitSegments.SelectMany(x => x.Commits).ToList();
    }

    /// <summary>
    ///     All commits reachable from head commit to all immediately prior releases.
    /// </summary>
    public IReadOnlyList<Commit> Commits { get; }

    /// <summary>
    ///     Segments where the oldest commit is a prior release (or root commit).
    /// </summary>
    public IReadOnlyList<LinkedSegment> LeafSegments => _leafSegments.Value;

    /// <summary>
    ///     Linked git segments containing the commits. Use for navigating to prior releases.
    /// </summary>
    public IReadOnlyList<LinkedSegment> Segments => _segments.Value;

    private static IReadOnlyList<LinkedSegment> BuildLinkedSegments(Commit head, IReadOnlyList<GitSegment> gitSegments)
    {
        var segmentsByYoungestCommit = gitSegments.ToDictionary(k => k.YoungestCommit.CommitId, v => v);
        var linkedSegments = new Dictionary<CommitId, LinkedSegment>();
        BuildLinkedSegments(linkedSegments,
                            CreateLinkedSegment(linkedSegments, head.CommitId, segmentsByYoungestCommit),
                            segmentsByYoungestCommit);
        return linkedSegments.Values.ToList();
    }

    private static void BuildLinkedSegments(Dictionary<CommitId, LinkedSegment> linkedSegments,
                                            LinkedSegment linkedSegment,
                                            Dictionary<CommitId, GitSegment> segmentsByYoungestCommit)
    {
        var segment = linkedSegment.Inner;
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

            var linkedParentSegment = CreateLinkedSegment(linkedSegments, parentCommitId, segmentsByYoungestCommit);
            linkedParentSegment.AddChild(linkedSegment);

            BuildLinkedSegments(linkedSegments, linkedParentSegment, segmentsByYoungestCommit);
        }
    }

    private static LinkedSegment CreateLinkedSegment(Dictionary<CommitId, LinkedSegment> linkedSegments,
                                                     CommitId youngestCommitId,
                                                     Dictionary<CommitId, GitSegment> segmentsByYoungestCommit)
    {
        var linkedSegment = new LinkedSegment(segmentsByYoungestCommit[youngestCommitId]);
        linkedSegments.Add(youngestCommitId, linkedSegment);
        return linkedSegment;
    }
}