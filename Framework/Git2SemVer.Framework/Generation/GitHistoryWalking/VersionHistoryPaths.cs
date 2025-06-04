using System.Collections.Immutable;

namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class HistoryPaths : IHistoryPaths
{
    private readonly IReadOnlyList<VersionHistorySegment> _segments;

    public HistoryPaths(IReadOnlyList<VersionHistoryPath> paths,
        IReadOnlyList<VersionHistorySegment> segments)
    {
        _segments = segments;
        Paths = paths.ToSortedSet();
        BestPath = Paths.First();
    }

    public IVersionHistoryPath BestPath { get; }

    public ImmutableSortedSet<IVersionHistoryPath> Paths { get; }
}