using NoeticTools.Common.Logging;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistory;

#pragma warning disable CS1591
internal sealed class VersionHistoryPathsBuilder
{
    private readonly IReadOnlyList<VersionHistorySegment> _segments;
    private readonly ILogger _logger;

    public VersionHistoryPathsBuilder(IReadOnlyList<VersionHistorySegment> segments, ILogger logger)
    {
        _segments = segments;
        _logger = logger;
    }

    public HistoryPaths Build()
    {
        _logger.LogDebug($"Building git paths to last releases from segments.");

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

        _logger.LogDebug($"Found {paths.Count} paths.");
        using (_logger.EnterLogScope())
        {
            foreach (var path in paths)
            {
                _logger.LogDebug(path.ToString());
            }
        }

        return paths;
    }
}