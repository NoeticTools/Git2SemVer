using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Extensions;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Versioning.GitHistoryWalking;

internal class SegmentsAggregatorResult
{
    public SegmentsAggregatorResult(SemVersion priorVersion, SemVersion version, ApiChangeFlags changeFlags)
    {
        PriorVersion = priorVersion;
        Version = version;
        ChangeFlags = changeFlags;
    }

    /// <summary>
    ///     Aggregated change flags.
    /// </summary>
    public ApiChangeFlags ChangeFlags { get; }

    /// <summary>
    ///     The prior released version or 0.1.0 if starting from the repository root commit.
    /// </summary>
    public SemVersion PriorVersion { get; }

    /// <summary>
    ///     Calculated version.
    /// </summary>
    public SemVersion Version { get; }
}

internal sealed class SegmentsAggregator
{
    private readonly HashSet<LinkedSegment> _aggregatedSegments = [];
    private readonly ApiChangeFlags _changeFlags = new();

    public SegmentsAggregatorResult Aggregate(Commit head, LinkedSegment releaseLinkedSegment)
    {
        AggregateChangeFlags(releaseLinkedSegment);

        var releaseSegment = releaseLinkedSegment.Inner;
        var oldestCommit = releaseSegment.OldestCommit;
        var priorVersion = oldestCommit.IsRootCommit ? new SemVersion(0, 1, 0) : oldestCommit.TagMetadata.Version!;

        var version = (oldestCommit.IsRootCommit && !_changeFlags.Any) || oldestCommit.Equals(head)
            ? priorVersion
            : priorVersion.Bump(_changeFlags);

        return new SegmentsAggregatorResult(priorVersion, version, _changeFlags);
    }

    private void AggregateChangeFlags(LinkedSegment linkedSegment)
    {
        if (!_aggregatedSegments.Add(linkedSegment))
        {
            return;
        }

        _changeFlags.Aggregate(linkedSegment.ChangeFlags);
        foreach (var linkedChildSegment in linkedSegment.LinkedChildSegments)
        {
            AggregateChangeFlags(linkedChildSegment);
        }
    }
}