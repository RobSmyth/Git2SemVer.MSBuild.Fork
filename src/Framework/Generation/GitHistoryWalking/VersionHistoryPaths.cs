using System.Collections.Immutable;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class HistoryPaths : IHistoryPaths
{
    public HistoryPaths(IReadOnlyList<VersionHistoryPath> paths)
    {
        Paths = paths.ToSortedSet();
        BestPath = Paths.First();
    }

    public IVersionHistoryPath BestPath { get; }

    public ImmutableSortedSet<IVersionHistoryPath> Paths { get; }
}