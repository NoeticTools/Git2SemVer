using Microsoft.Build.Utilities;
using NoeticTools.Common.Exceptions;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using Semver;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistory;

internal sealed class VersionHistorySegment
{
    private static int _nextId = 1;
    private readonly List<Commit> _commits = [];
    private readonly ILogger _logger;
    private readonly IList<VersionHistorySegment> _older = [];
    private readonly IList<VersionHistorySegment> _younger = [];
    private ApiChanges? _bumps;

    private VersionHistorySegment(List<Commit> commits, ILogger logger) : this(logger)
    {
        _commits.AddRange(commits);
    }

    private VersionHistorySegment(ILogger logger)
    {
        _logger = logger;
        Id = _nextId++;
        logger.LogTrace("Created git path segment {0}", Id);
    }

    public ApiChanges Bumps => GetVersionBumps();

    public IReadOnlyList<Commit> Commits => _commits.ToList();

    public Commit FirstCommit => _commits.Last();

    public IReadOnlyList<VersionHistorySegment> From => _older.ToList();

    public int Id { get; }

    public Commit LastCommit => _commits[0];

    public SemVersion? TaggedReleasedVersion { get; set; }

    public IReadOnlyList<VersionHistorySegment> To => _younger.ToList();

    public void Append(Commit commit)
    {
        if (_commits.Count > 0 && FirstCommit.Parents.All(x => x.Id != commit.CommitId.Id))
        {
            throw new InvalidOperationException($"Cannot append {commit.CommitId.ShortSha} as it is not connected to segment's first (oldest) commit.");
        }

        _bumps = null;
        _commits.Add(commit);
        _logger.LogTrace("Appended commit {0} to segment {1}.", commit.CommitId.ShortSha, Id);
    }

    public VersionHistorySegment? BranchedFrom(VersionHistorySegment branchSegment, Commit commit)
    {
        _logger.LogDebug("Commit {0} in segment {1} branches to segment {2}.", commit.CommitId.ShortSha, Id, branchSegment.Id);

        if (commit.CommitId.Equals(LastCommit.CommitId))
        {
            _logger.LogTrace("Commit {0} is last (youngest) commit in segment {1}. Link segments.", commit.CommitId.ShortSha, Id);
            LinkToYounger(branchSegment);
            return null;
        }

        _bumps = null;
        var fromSegment = SplitSegmentAt(commit);
        fromSegment.LinkToYounger(branchSegment);
        return fromSegment;
    }

    public static VersionHistorySegment CreateHeadSegment(ILogger logger)
    {
        return new VersionHistorySegment(logger);
    }

    /// <summary>
    ///     Create a (younger) segment that this segment links to.
    /// </summary>
    public VersionHistorySegment CreateMergedSegment()
    {
        var fromBranch = new VersionHistorySegment(_logger);
        fromBranch.LinkToYounger(this);
        return fromBranch;
    }

    public override string ToString()
    {
        var toSegments = !To.Any()
            ? "To: none (head)"
            : $"To: {string.Join(",", To.Select(x => x.Id))}";

        var fromSegments = !From.Any()
            ? "From: none"
            : $"From: {string.Join(",", From.Select(x => x.Id))}";

        var commitsCount = $"({_commits.Count})";

        return
            $"Segment {Id}: {LastCommit.CommitId.ShortSha} -> {FirstCommit.CommitId.ShortSha}  {commitsCount,5}  {Bumps.ToString() ?? "???"}  {toSegments,-16}  {fromSegments,-16}";
    }

    private ApiChanges GetVersionBumps()
    {
        if (_bumps != null)
        {
            return _bumps.Value;
        }

        var bumps = new ApiChanges();

        var parser = new ConventionalCommitParser(_logger);
        foreach (var commit in _commits)
        {
            var commitBumps = parser.Parse(commit);
            bumps.Aggregate(commitBumps);
        }

        _bumps = bumps;
        return bumps;
    }

    private void LinkToYounger(VersionHistorySegment toSegment)
    {
        _younger.Add(toSegment);
        toSegment._older.Add(this);
    }

    private VersionHistorySegment SplitSegmentAt(Commit commit)
    {
        var index = _commits.IndexOf(commit);
        if (index < 0)
        {
            throw new Git2SemVerInvalidOperationException("Cannot split a segment that does not contain the commit.");
        }

        _logger.LogTrace("Splitting segment {0} at commit {1}", Id, commit.CommitId.ShortSha);

        var keepCommits = _commits.Take(index).ToList();
        var newSegmentCommits = _commits.Skip(index).Take(_commits.Count - index).ToList();
        _commits.Clear();
        _commits.AddRange(keepCommits);

        var fromSegment = new VersionHistorySegment(newSegmentCommits, _logger);
        fromSegment.LinkToYounger(this);
        return fromSegment;
    }
}