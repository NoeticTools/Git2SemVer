using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using NoeticTools.Git2SemVer.Framework.Versioning;
using NoeticTools.Git2SemVer.Framework.Versioning.GitHistoryWalking;
using Semver;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

public class ConventionalCommitsVersionInfo
{
    [JsonConstructor]
    public ConventionalCommitsVersionInfo()
    {
    }

    public ConventionalCommitsVersionInfo(IVersionOutputs outputs, ContributingCommits contributing)
    {
        ContributingReleases = outputs.Git.ContributingReleases.Select(x => x.ToString()).ToArray();
        ContributingCommits = contributing.Commits.Where(IsAContributingCommit).Select(x => new ConventionalCommit(x)).ToList();
        HeadCommitSha = contributing.Head.CommitId.Sha;
        HeadCommitWhen = contributing.Head.When;
        BranchName = contributing.BranchName;
        Version = outputs.Version!;
        InformationalVersion = outputs.InformationalVersion!;
    }

    /// <summary>
    ///     Name of the branch the head is on.
    /// </summary>
    [JsonRequired]
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    ///     Conventional commits that contribute to the version calculation.
    /// </summary>
    [JsonPropertyOrder(200)]
    [JsonRequired]
    public IReadOnlyList<ConventionalCommit> ContributingCommits { get; set; } = [];

    /// <summary>
    ///     Releases (tags) that contribute to the version calculation.
    /// </summary>
    [JsonPropertyOrder(100)]
    [JsonRequired]
    public string[] ContributingReleases { get; set; } = [];

    /// <summary>
    ///     The SHA-1 hash of the head commit in the repository.
    /// </summary>
    [JsonRequired]
    public string HeadCommitSha { get; set; } = string.Empty;

    /// <summary>
    ///     When the head commit was made.
    /// </summary>
    [JsonRequired]
    public DateTimeOffset HeadCommitWhen { get; set; } = DateTimeOffset.MinValue;

    /// <summary>
    ///     The calculated informational version.
    /// </summary>
    [JsonRequired]
    [JsonConverter(typeof(SemVersionJsonConverter))]
    public SemVersion InformationalVersion { get; set; } = new(0, 0, 0);

    /// <summary>
    ///     The calculated semantic version.
    /// </summary>
    [JsonPropertyOrder(-100)]
    [JsonRequired]
    [JsonConverter(typeof(SemVersionJsonConverter))]
    public SemVersion Version { get; set; } = new(0, 0, 0);

    public void Save(File file)
    {
        var directory = file.GetDirectory();
        if (!directory.Exists())
        {
            directory.Create();
        }

        Git2SemVerJsonSerializer.Write(file, this);
    }

    /// <summary>
    ///     Determines whether the specified commit is a contributing commit based on its metadata.
    /// </summary>
    /// <param name="x">The commit to evaluate. Must contain valid message metadata.</param>
    /// <returns>
    ///     <see langword="true" /> if the commit has a non-empty change type and indicates any API change flags; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    private static bool IsAContributingCommit(Commit x)
    {
        var changeType = x.MessageMetadata.ChangeType;
        return changeType.Length > 0 &&
               x.MessageMetadata.ApiChangeFlags.Any;
    }
}