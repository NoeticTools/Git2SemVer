using NoeticTools.Common.Tools.Git;
using Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistory;

#pragma warning disable CS1591
internal sealed class VersionHistoryPath
{
    private readonly List<VersionHistorySegment> _segments = [];
    private ApiChanges _bumps;
    private int? _commitsCount;

    public VersionHistoryPath(params VersionHistorySegment[] segments)
    {
        _segments.AddRange(segments);
        LastReleasedVersion = segments.First().TaggedReleasedVersion;
        Version = GetNextReleaseVersion();
    }

    public int CommitsSinceLastRelease
    {
        get
        {
            _commitsCount ??= _segments.Sum(x => x.Commits.Count);
            return _commitsCount.Value;
        }
    }

    public Commit FirstCommit => _segments.First().FirstCommit;

    public Commit HeadCommit => _segments.Last().LastCommit;

    public int Id { get; internal set; }

    public SemVersion? LastReleasedVersion { get; }

    public SemVersion Version { get; }

    public override string ToString()
    {
        var commitsCount = $"({CommitsSinceLastRelease})";
        var segmentIdsString = string.Join("-", _segments.Select(x => x.Id));
        return
            $"Path {Id,3}: Segment{(_segments.Count > 1 ? "s" : " ")} {segmentIdsString,-20} {commitsCount,5}  {_bumps.ToString()}  {LastReleasedVersion?.ToString() ?? " none"} -> {GetNextReleaseVersion()}";
    }

    public IReadOnlyList<VersionHistoryPath> With(IReadOnlyList<VersionHistorySegment> toSegments)
    {
        if (!toSegments.Any())
        {
            return new List<VersionHistoryPath> { this };
        }

        var paths = new List<VersionHistoryPath>();
        foreach (var segment in toSegments)
        {
            paths.AddRange(With(segment).With(segment.To));
        }

        return paths;
    }

    private ApiChanges AggregateBumps()
    {
        var bumps = new ApiChanges();
        foreach (var segmentBumps in _segments.Select(segment => segment.Bumps))
        {
            bumps.BreakingChange |= segmentBumps.BreakingChange;
            bumps.FunctionalityChange |= segmentBumps.FunctionalityChange;
            bumps.Patch |= segmentBumps.Patch;
        }

        return bumps;
    }

    private SemVersion GetNextReleaseVersion()
    {
        _bumps = AggregateBumps();

        var version = _segments.First().TaggedReleasedVersion;
        if (version == null)
        {
            version = new SemVersion(0, 1, 0);
        }
        else
        {
            if (_bumps.BreakingChange)
            {
                return new SemVersion(version.Major + 1, 0, 0);
            }

            if (_bumps.FunctionalityChange)
            {
                return new SemVersion(version.Major, version.Minor + 1, 0);
            }

            return new SemVersion(version.Major, version.Minor, version.Patch + 1);
        }

        return version;
    }

    private VersionHistoryPath With(VersionHistorySegment segment)
    {
        var segmentIds = new List<VersionHistorySegment>(_segments) { segment };
        return new VersionHistoryPath(segmentIds.ToArray());
    }
}