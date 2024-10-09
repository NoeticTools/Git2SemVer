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

        _logger.LogDebug("Finding git path segments to last releases.");
        BuildSegmentsTo(commit);

        stopwatch.Stop();
        _logger.LogDebug($"Found {_segments.Count} segments ({stopwatch.ElapsedMilliseconds}ms)");

        using (_logger.EnterLogScope())
        {
            foreach (var segment in _segments)
            {
                _logger.LogDebug(segment.Value.ToString());
            }
        }

        return _segments.Values.ToList();
    }

    private void BuildSegmentsTo(Commit commit)
    {
        _logger.LogTrace("Finding segments to: {0}", commit.CommitId.ObfuscatedSha);
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
        _logger.LogDebug("Branch commit: {0}", commit.CommitId.ObfuscatedSha);
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
        _logger.LogTrace($"Commit: {commit.CommitId.ObfuscatedSha}  {commit.ReleasedVersion?.ToString() ?? ""}");

        if (_commitsCache.ContainsKey(commit.CommitId))
        {
            _logger.LogTrace("Commit {0} exists in another segment.");
            OnBranchCommit(commit);
            return SegmentWalkResult.FoundStart;
        }

        _segment.Append(commit);
        _commitsCache.Add(commit.CommitId, _segment);

        if (commit.ReleasedVersion != null)
        {
            _logger.LogTrace("Commit {0} has release tag", commit.CommitId.ObfuscatedSha);
            _segment.TaggedReleasedVersion = commit.ReleasedVersion;
            return SegmentWalkResult.FoundStart;
        }

        var parents = commit.Parents.ToList();

        if (!parents.Any())
        {
            // First commit in repository
            return SegmentWalkResult.FoundStart;
        }

        if (parents.Count == 2)
        {
            OnMergeCommit(commit);
            return SegmentWalkResult.FoundStart;
        }

        return SegmentWalkResult.Continue;
    }

    /*
     *        Merge commit: 99f9d75
       1>            Merged from commit: aaab759
       1>            Finding segments to: aaab759
       1>              Commit: aaab759
       1>              Commit: 8576c20
       1>              Commit: 501fe22
       1>              Commit: 14eca22
       1>              Commit: f538a02
       1>              Commit: 28aa374
       1>              Commit: d77cae7  0.3.1
       1>            Merged from commit: e228a8b <<<< A
       1>            Finding segments to: e228a8b
       1>              Commit: e228a8b
       1>              Merge commit: e228a8b <<<<<< SUSPECT this merge immediately after A -- PROBLEM IS LATER
       1>                Merged from commit: 8576c20
       1>                Finding segments to: 8576c20
       1>                  Commit: 8576c20
       1>                  Branch commit: 8576c20
       1>                    Branched from commit: 8576c20
       1>                Merged from commit: 5158acd
       1>                Finding segments to: 5158acd
       1>                  Commit: 5158acd
       1>                  Commit: f538a02
       1>                  Branch commit: f538a02
       1>                    Branched from commit: f538a02
       1>        Merged from commit: daf530d
       
     */

    private void OnMergeCommit(Commit commit)
    {
        _logger.LogDebug($"Merge commit: {commit.CommitId.ObfuscatedSha}");
        using (_logger.EnterLogScope())
        {
            foreach (var parent in commit.Parents.ToList())
            {
                _logger.LogDebug($"Merged from commit: {parent.ObfuscatedSha}");
                using (_logger.EnterLogScope())
                {
                    var newSegmentVisitor = new VersionHistorySegmentsBuilder(_segment.CreateMergedSegment(), this);
                    newSegmentVisitor.BuildSegmentsTo(_commits.Get(parent));
                }
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