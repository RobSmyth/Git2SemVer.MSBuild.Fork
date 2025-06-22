using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

#pragma warning disable CS1591
internal sealed class LinkedSegment(VersionHistorySegment segment)
{
    private readonly List<LinkedSegment> _linkedChildSegments = [];

    public bool IsAReleaseSegment => segment.IsAReleaseSegment;

    public IReadOnlyList<LinkedSegment> LinkedChildSegments => _linkedChildSegments;

    /// <summary>
    ///     Aggregated API changes from this segment up to all younger segments.
    /// </summary>
    public ApiChangeFlags ChangeFlags => segment.ApiChanges.Flags;

    public CommitId YoungestCommitId => segment.YoungestCommit.CommitId;

    public CommitId OldestCommitId => segment.OldestCommit.CommitId;

    /// <summary>
    ///     Link to child (younger) segment.
    /// </summary>
    public void AddChild(LinkedSegment childSegment)
    {
        if (_linkedChildSegments.Contains(childSegment))
        {
            return;
        }

        if (_linkedChildSegments.Count == 2)
        {
            throw new Git2SemVerInvalidOperationException("A history segment may not have more than 2 child segments.");
        }

        _linkedChildSegments.Add(childSegment);
    }
}