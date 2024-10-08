using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistory;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

// ReSharper disable CanSimplifyDictionaryLookupWithTryAdd

namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;

internal sealed class PathsFromLastReleasesFinder : IGitHistoryPathsFinder
{
    private readonly ICommitsRepository _commitsRepo;
    private readonly IGitTool _gitTool;
    private readonly ILogger _logger;

    public PathsFromLastReleasesFinder(ICommitsRepository commitsRepo, IGitTool gitTool, ILogger logger)
    {
        _commitsRepo = commitsRepo;
        _gitTool = gitTool;
        _logger = logger;
    }

    public HistoryPaths FindPathsToHead()
    {
        _logger.LogDebug($"Current branch: {_gitTool.BranchName}");
        var segments = new VersionHistorySegmentsBuilder(_commitsRepo, _logger).BuildTo(_commitsRepo.Head);
        return new VersionHistoryPathsBuilder(segments, _logger).Build();
    }
}