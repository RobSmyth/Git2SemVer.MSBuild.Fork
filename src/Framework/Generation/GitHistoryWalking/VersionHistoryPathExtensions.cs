﻿using System.Collections.Immutable;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

#pragma warning disable CS1591

internal static class VersionHistoryPathExtensions
{
    internal static ImmutableSortedSet<IVersionHistoryPath> ToSortedSet(this IReadOnlyList<IVersionHistoryPath> paths)
    {
        return paths.ToImmutableSortedSet(new PathComparer());
    }

    private sealed class PathComparer : IComparer<IVersionHistoryPath>
    {
        public int Compare(IVersionHistoryPath? left, IVersionHistoryPath? right)
        {
            var versionPrecedence = right!.Version.ComparePrecedenceTo(left!.Version);
            if (versionPrecedence == 0)
            {
                var sizePrecedence = left.CommitsSinceLastRelease - right.CommitsSinceLastRelease;
                return sizePrecedence != 0 ? sizePrecedence : left.Id - right.Id;
            }

            return versionPrecedence;
        }
    }
}