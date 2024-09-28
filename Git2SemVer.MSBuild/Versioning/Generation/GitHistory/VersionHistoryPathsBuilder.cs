namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistory;

#pragma warning disable CS1591
internal sealed class VersionHistoryPathsBuilder
{
    private readonly IReadOnlyList<VersionHistorySegment> _segments;

    public VersionHistoryPathsBuilder(IReadOnlyList<VersionHistorySegment> segments)
    {
        _segments = segments;
    }

    public HistoryPaths Build()
    {
        var paths = FindPaths();
        return new HistoryPaths(paths, _segments);
    }

    private IReadOnlyList<VersionHistorySegment> StartSegments => _segments.Where(x => !x.From.Any()).ToList();

    private List<VersionHistoryPath> FindPaths()
    {
        var paths = new List<VersionHistoryPath>();
        foreach (var segment in StartSegments)
        {
            paths.AddRange(new VersionHistoryPath(segment).With(segment.To));
        }

        var nextPathId = 1;
        foreach (var path in paths)
        {
            path.Id = nextPathId++;
        }

        return paths;
    }
}