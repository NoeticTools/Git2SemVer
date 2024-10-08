using System.Diagnostics;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistory;

internal sealed class VersionHistorySegmentsBuilder
{
    private readonly ICommitsRepository _commits;
    private readonly Dictionary<CommitId, VersionHistorySegment> _commitsCache = new();
    private readonly ILogger _logger;
    private readonly VersionHistorySegment _segment;
    private readonly Dictionary<int, VersionHistorySegment> _segments = [];

    private VersionHistorySegmentsBuilder(VersionHistorySegment segment, VersionHistorySegmentsBuilder parent)
    {
        _logger = parent._logger;
        _segments = parent._segments;
        _commits = parent._commits;
        _commitsCache = parent._commitsCache;
        _segment = segment;
        _segments.Add(segment.Id, segment);
    }

    public VersionHistorySegmentsBuilder(ICommitsRepository commits, ILogger logger)
    {
        _commits = commits;
        _logger = logger;
        _segment = VersionHistorySegment.CreateHeadSegment(logger);
        _segments.Add(_segment.Id, _segment);
    }

    public IReadOnlyList<VersionHistorySegment> BuildTo(Commit commit)
    {
        var stopwatch = Stopwatch.StartNew();

        BuildSegmentsTo(commit);

        stopwatch.Stop();
        _logger.LogDebug($"Found {_segments.Count} segments ({stopwatch.ElapsedMilliseconds}ms)");

        using (_logger.EnterLogScope())
        {
            foreach (var segment in _segments)
            {
                _logger.LogDebug(segment.ToString());
            }
        }

        return _segments.Values.ToList();
    }

    private void BuildSegmentsTo(Commit commit)
    {
        _logger.LogTrace($"Finding segments to: {commit.CommitId.ShortSha}");
        using (_logger.EnterLogScope())
        {
            while (OnCommit(commit) == SegmentWalkResult.Continue)
            {
                commit = _commits.Get(commit.Parents.First());
            }
        }
    }

    private void OnBranchCommit(Commit commit)
    {
        _logger.LogDebug($"Branch commit: {commit.CommitId.ShortSha}");
        using (_logger.EnterLogScope())
        {
            var intersectingSegment = _commitsCache[commit.CommitId];
            var branchedFromSegment = intersectingSegment.BranchedFrom(_segment, commit);
            if (branchedFromSegment == null)
            {
                return;
            }

            _segments.Add(branchedFromSegment.Id, branchedFromSegment);
            foreach (var segmentCommit in branchedFromSegment.Commits)
            {
                _commitsCache[segmentCommit.CommitId] = branchedFromSegment;
            }
        }
    }

    private SegmentWalkResult OnCommit(Commit commit)
    {
        _logger.LogTrace($"Commit: {commit.CommitId.ShortSha}  {commit.ReleasedVersion?.ToString() ?? ""}");

        if (_commitsCache.ContainsKey(commit.CommitId))
        {
            OnBranchCommit(commit);
            return SegmentWalkResult.FoundStart;
        }

        _segment.Append(commit);
        _commitsCache.Add(commit.CommitId, _segment);

        if (commit.ReleasedVersion != null)
        {
            _segment.TaggedReleasedVersion = commit.ReleasedVersion;
            return SegmentWalkResult.FoundStart;
        }

        var parents = commit.Parents.ToList();

        if (!parents.Any())
        {
            return SegmentWalkResult.FoundStart;
        }

        if (parents.Count != 2)
        {
            return SegmentWalkResult.Continue;
        }

        OnMergeCommit(commit);
        return SegmentWalkResult.FoundStart;
    }

    private void OnMergeCommit(Commit commit)
    {
        _logger.LogDebug($"Merge commit: {commit.CommitId.ShortSha}");
        using (_logger.EnterLogScope())
        {
            foreach (var parent in commit.Parents.ToList())
            {
                _logger.LogDebug($"Merged from commit: {parent.ShortSha}");
                var newSegmentVisitor = new VersionHistorySegmentsBuilder(_segment.CreateToSegment(), this);
                newSegmentVisitor.BuildSegmentsTo(_commits.Get(parent));
            }
        }
    }

    private enum SegmentWalkResult
    {
        Unknown = 0,
        Continue,
        FoundStart
    }
}