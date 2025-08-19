using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.FileSystem;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using NoeticTools.Git2SemVer.Framework.Versioning;
using NoeticTools.Git2SemVer.Framework.Versioning.GitHistoryWalking;
using Semver;


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
        ConventionalCommits = contributing.Commits.Where(HasConventionalCommitInfo).Select(x => new ConventionalCommit(x)).ToList();
        HeadCommitSha = contributing.Head.CommitId.Sha;
        HeadCommitWhen = contributing.Head.When;
        BranchName = contributing.BranchName;
        Version = outputs.Version!;
        InformationalVersion = outputs.InformationalVersion!;
    }

    [JsonRequired]
    public string BranchName { get; set; } = string.Empty;

    [JsonPropertyOrder(100)]
    [JsonRequired]
    public string[] ContributingReleases { get; set; } = [];

    [JsonPropertyOrder(200)]
    [JsonRequired]
    public IReadOnlyList<ConventionalCommit> ConventionalCommits { get; set; } = [];

    [JsonRequired]
    public string HeadCommitSha { get; set; } = string.Empty;

    [JsonRequired]
    public DateTimeOffset HeadCommitWhen { get; set; } = DateTimeOffset.MinValue;

    [JsonRequired]
    [JsonConverter(typeof(SemVersionJsonConverter))]
    public SemVersion InformationalVersion { get; set; } = new(0, 0, 0);

    [JsonPropertyOrder(-100)]
    [JsonRequired]
    [JsonConverter(typeof(SemVersionJsonConverter))]
    public SemVersion Version { get; set; } = new(0, 0, 0);

    public void Save(FilePath filePath)
    {
        var directory = filePath.GetDirectory();
        if (!directory.Exists())
        {
            directory.Create();
        }

        Git2SemVerJsonSerializer.Write(filePath, this);
    }

    private static bool HasConventionalCommitInfo(Commit x)
    {
        var changeType = x.MessageMetadata.ChangeType;
        return changeType.Length > 0 &&
               x.MessageMetadata.ApiChangeFlags.Any;
    }
}