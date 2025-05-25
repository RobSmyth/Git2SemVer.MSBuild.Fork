using System.Collections.Immutable;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

public interface IHistoryPaths
{
    IVersionHistoryPath BestPath { get; }

    ImmutableSortedSet<IVersionHistoryPath> Paths { get; }
}