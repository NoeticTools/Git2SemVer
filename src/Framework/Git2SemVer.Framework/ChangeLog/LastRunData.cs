using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.Diagnostics;
using NoeticTools.Git2SemVer.Core.FileSystem;
using NoeticTools.Git2SemVer.Core.Logging;
using Semver;
using Directory = NoeticTools.Git2SemVer.Core.FileSystem.Directory;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

/// <summary>
///     Data from the last run when generating a specific changelog.
/// </summary>
public sealed class LastRunData
{
    [JsonPropertyOrder(40)]
    // ReSharper disable once MemberCanBePrivate.Global
    public IReadOnlyList<string> ContributingReleases { get; set; } = [];

    [JsonPropertyOrder(10)]
    // ReSharper disable once MemberCanBePrivate.Global
    public string ForcedReleasedTitle { get; set; } = "";

    [JsonPropertyOrder(50)]
    public List<HandledChange> HandledChanges { get; set; } = [];

    [JsonIgnore]
    public bool NoData => string.IsNullOrEmpty(Rev);

    /// <summary>
    ///     This file's schema revision. To allow for file migration.
    /// </summary>
    [JsonPropertyOrder(-10)]
    // ReSharper disable once MemberCanBePrivate.Global
    public string Rev { get; set; } = "";

    public bool ContributingReleasesChanged(SemVersion[] priorContributingReleases)
    {
        if (ContributingReleases.Count == 0)
        {
            return false; // no data
        }

        if (ContributingReleases.Count != priorContributingReleases.Length)
        {
            return true;
        }

        return !priorContributingReleases.All(ver => ContributingReleases.Contains(ver.ToString()));
    }

    public static LastRunData Load(Directory directory, File filename, ILogger logger)
    {
        var data = Git2SemVerJsonSerializer.Read<LastRunData>(GetFilePath(directory, filename).FullName);
        if (data.NoData)
        {
            logger.LogWarning(new GSV201(directory, filename));
        }

        return data;
    }

    public void Save(Directory directory, File file)
    {
        Rev = "1";
        var path = GetFilePath(directory, file).FullName;
        Git2SemVerJsonSerializer.Write(path, this);
    }

    public void Update(ConventionalCommitsVersionInfo outputs)
    {
        ContributingReleases = outputs.ContributingReleases.Select(x => x.ToString()).ToReadOnlyList();
    }

    private static FileInfo GetFilePath(Directory dataDirectory, File targetFile)
    {
        var targetFilename = targetFile.IsEmptyPath ? "no_target" : targetFile.FileName;
        return new FileInfo(Path.Combine(dataDirectory, targetFilename + ChangelogConstants.LastRunDataFileSuffix));
    }
}