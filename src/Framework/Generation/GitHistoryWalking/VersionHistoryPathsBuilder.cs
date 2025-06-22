using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using Spectre.Console.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

#pragma warning disable CS1591
internal sealed class VersionHistoryPathsBuilder
{
    private const string LogPathListIndent = "    ";
    private const int LogPathsLimit = 500;
    private readonly ILookup<GitSegment, GitSegment> _childSegmentsLookup;
    private readonly IReadOnlyList<GitSegment> _segments;
    private readonly ILogger _logger;
    private readonly Dictionary<CommitId, GitSegment> _segmentsByYoungestCommit;

    public VersionHistoryPathsBuilder(IReadOnlyList<GitSegment> segments, ILogger logger)
    {
        _segments = segments;
        _logger = logger;
        _segmentsByYoungestCommit = segments.ToDictionary(k => k.YoungestCommit.CommitId, v => v);
        _childSegmentsLookup = GetChildSegmentsLookup(segments, _segmentsByYoungestCommit);
    }

    /// <summary>
    /// Build a collection of commit history paths to preceding releases from found segments.
    /// </summary>
    public HistoryPaths BuildTo()
    {
        var stopwatch = Stopwatch.StartNew();
        var foundPaths = FindPaths();
        stopwatch.Stop();
        var paths = new HistoryPaths(foundPaths);
        LogFoundPaths(paths, stopwatch.Elapsed);
        return paths;
    }

    private List<VersionHistoryPath> FindPaths()
    {
        var releaseSegments = _segments.Where(x => x.ParentCommits.Count == 0 ||
                                             x.TaggedReleasedVersion != null).ToList();
        var paths = new List<VersionHistoryPath>();
        foreach (var startSegment in releaseSegments)
        {
            paths.AddRange(GetChildPaths(startSegment, new VersionHistoryPath(startSegment)));
        }

        var nextPathId = 1;
        foreach (var path in paths)
        {
            path.Id = nextPathId++;
        }

        return paths;
    }

    private List<VersionHistoryPath> GetChildPaths(GitSegment parentSegment, VersionHistoryPath path)
    {
        var childSegments = _childSegmentsLookup[parentSegment].ToList();
        if (childSegments.Count == 0)
        {
            return [path];
        }
        
        var pathSegments = new List<VersionHistoryPath>();
        foreach (var childSegment in childSegments)
        {
            pathSegments.AddRange(GetChildPaths(childSegment, path.With(childSegment)));
        }

        return pathSegments;
    }

    private static ILookup<GitSegment, GitSegment> GetChildSegmentsLookup(IReadOnlyList<GitSegment> segments,
                                                                                                Dictionary<CommitId, GitSegment>
                                                                                                    segmentsByYoungestCommit)
    {
        var childLinks = new List<(GitSegment parent, GitSegment child)>();
        foreach (var segment in segments)
        {
            foreach (var parentCommit in segment.ParentCommits)
            {
                if (segmentsByYoungestCommit.TryGetValue(parentCommit, out var parentSegment))
                {
                    childLinks.Add((parent: parentSegment, child: segment));
                }
            }
        }

        var childSegmentsLookup = childLinks.ToLookup(k => k.parent, v => v.child);
        return childSegmentsLookup;
    }

    private void LogFoundPaths(IHistoryPaths paths, TimeSpan timeTaken)
    {
        var stringBuilder = new StringBuilder();

        var bestPath = paths.BestPath;
        var pathsCount = paths.Paths.Count;
        stringBuilder.AppendLine($"Found {pathsCount} path{(pathsCount == 1 ? "" : "s")} (in {timeTaken.Milliseconds:F0} ms):");
        if (pathsCount < LogPathsLimit)
        {
            LogAllPaths(paths, stringBuilder);
        }
        else
        {
            LogPath(stringBuilder, bestPath);
        }

        _logger.LogDebug(stringBuilder.ToString().TrimEnd());

        _logger.LogDebug($"Path {bestPath.Id} is the shortest path resulting in the highest version {bestPath.Version}.");

        _logger.LogDebug(bestPath.GetVersioningReport());
    }

    private void LogPath(StringBuilder stringBuilder, IVersionHistoryPath bestPath)
    {
        using (_logger.EnterLogScope())
        {
            stringBuilder.AppendLine(LogPathListIndent + VersionHistoryPath.ListHeader);
            stringBuilder.AppendLine(LogPathListIndent + bestPath.ToString());
        }
    }

    private void LogAllPaths(IHistoryPaths paths, StringBuilder stringBuilder)
    {
        using (_logger.EnterLogScope())
        {
            stringBuilder.AppendLine(LogPathListIndent + VersionHistoryPath.ListHeader);
            foreach (var path in paths.Paths)
            {
                stringBuilder.AppendLine(LogPathListIndent + path.ToString());
            }
        }
    }
}