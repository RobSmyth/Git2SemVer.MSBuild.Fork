using System.Security.Cryptography.X509Certificates;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using Spectre.Console.Rendering;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistoryWalking;

#pragma warning disable CS1591
internal sealed class VersionHistoryPathsBuilder
{
    private readonly ILogger _logger;
    private readonly IReadOnlyList<VersionHistorySegment> _segments;
    private readonly ILookup<VersionHistorySegment, VersionHistorySegment> _childSegmentsLookup;

    public VersionHistoryPathsBuilder(IReadOnlyList<VersionHistorySegment> segments, ILogger logger)
    {
        _segments = segments;
        _logger = logger;

        var segmentsByYoungestCommit = _segments.ToDictionary(k => k.YoungestCommit.CommitId, v => v);
        _childSegmentsLookup = GetChildSegmentsLookup(segments, segmentsByYoungestCommit);
        //var segmentsByOldestCommit = _segments.ToDictionary(k => k.OldestCommit.CommitId, v => v);
        _startSegments = segments.Where(x => x.ParentCommits.Count == 0 || 
                                             x.TaggedReleasedVersion != null).ToList();
    }

    private static ILookup<VersionHistorySegment, VersionHistorySegment> GetChildSegmentsLookup(IReadOnlyList<VersionHistorySegment> segments, Dictionary<CommitId, VersionHistorySegment> segmentsByYoungestCommit)
    {
        var childLinks = new List<(VersionHistorySegment parent, VersionHistorySegment child)>();
        foreach (var segment in segments)
        {
            foreach (var parentCommit in segment.ParentCommits)
            {
                if (segmentsByYoungestCommit.TryGetValue(parentCommit, out var parentSegment))
                {
                    childLinks.Add((parent:parentSegment, child:segment));
                }
            }
        }

        var childSegmentsLookup = childLinks.ToLookup(k => k.parent, v => v.child);
        return childSegmentsLookup;
    }

    public HistoryPaths Build()
    {
        _logger.LogDebug("Building git paths to last releases from segments.");

        var paths = FindPaths();
        return new HistoryPaths(paths, _segments);
    }

    private readonly IReadOnlyList<VersionHistorySegment> _startSegments;

    private List<VersionHistoryPath> GetChildPaths(VersionHistorySegment parentSegment, VersionHistoryPath path)
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

    private List<VersionHistoryPath> FindPaths()
    {
        var paths = new List<VersionHistoryPath>();
        foreach (var startSegment in _startSegments)
        {
            paths.AddRange(GetChildPaths(startSegment, new VersionHistoryPath(startSegment)));
        }

        var nextPathId = 1;
        foreach (var path in paths)
        {
            path.Id = nextPathId++;
        }

        _logger.LogDebug($"Found {paths.Count} paths.");
        using (_logger.EnterLogScope())
        {
            _logger.LogDebug("Path #   Segments (count)            Bumps    Ver from/to");
            foreach (var path in paths)
            {
                _logger.LogDebug(path.ToString());
            }
        }

        return paths;
    }
}