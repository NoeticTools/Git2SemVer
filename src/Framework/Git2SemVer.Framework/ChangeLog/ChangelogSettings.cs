using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using Directory = NoeticTools.Git2SemVer.Core.FileSystem.Directory;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

/// <summary>
///     Git2SemVer changelog generation settings.
/// </summary>
/// <remarks>
///     Git repository (project) settings. To be located with the git repository.
/// </remarks>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ChangelogSettings : JsonSettingsFileBase<ChangelogSettings>, IEquatable<ChangelogSettings>, IChangelogSettings
{
    /// <summary>
    ///     Optional url to a version's artifacts. May contain version placeholder '%VERSION%'.
    /// </summary>
    [JsonPropertyOrder(50)]
    public string ArtifactLinkPattern { get; set; } = ChangelogConstants.DefaultArtifactLinkPattern;

    /// <summary>
    ///     Categories to include in the changelog.
    /// </summary>
    [JsonPropertyOrder(120)]
    public CategorySettings[] Categories { get; set; } =
    [
        new(1, "Added", "feat"),
        new(2, "Changed", "change"),
        new(3, "Depreciated", "deprecate"),
        new(4, "Removed", "remove"),
        new(5, "Fixed", "fix"),
        new(6, "Security", "security"),
        new(7, "Other", "^(?!dev|Dev|refactor).*$")
    ];

    [JsonPropertyOrder(110)]
    public ConventionalCommitsSettings ConvCommits { get; set; } = new();

    /// <summary>
    ///     Path to generator's data and configuration files directory. It may be a relative or absolute path.
    /// </summary>
    [JsonPropertyOrder(20)]
    public Directory DataDirectory { get; set; } = ChangelogConstants.DefaultDataDirectory;

    /// <summary>
    ///     Changelog generation enabled flag.
    /// </summary>
    [JsonPropertyOrder(-10)]
    public bool Enabled { get; set; } = true;

    // ReSharper disable once GrammarMistakeInComment
    /// <summary>
    ///     Issue link format with issue ID as argument ({0}).
    /// </summary>
    /// <remarks>
    ///     Example:
    ///     <example>
    ///         "https://organisation-name/project-name/issues/{0}"
    ///     </example>
    /// </remarks>
    [JsonPropertyOrder(60)]
    public string IssueLinkFormat { get; set; } = ChangelogConstants.IssueLinkFormat;

    /// <summary>
    ///     Generated changelog file path. It may be a relative or absolute path. Set to empty string to disable file write.
    /// </summary>
    [JsonPropertyOrder(10)]
    public File OutputFilePath { get; set; } = ChangelogConstants.DefaultFilename;

    /// <summary>
    ///     Configuration file schema revision.
    /// </summary>
    [JsonPropertyOrder(-20)]
    // ReSharper disable once MemberCanBePrivate.Global
    public string Rev { get; set; } = "1";

    public bool Equals(ChangelogSettings? other)
    {
        if (other is null)
        {
            return false;
        }

        return GetHashCode() == other.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is ChangelogSettings other && Equals(other));
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        var categoriesCode = Categories.Aggregate(17, (current, item) => current * 23 + item.GetHashCode());
        return HashCode.Combine(ArtifactLinkPattern, categoriesCode, ConvCommits.GetHashCode(), DataDirectory, IssueLinkFormat, OutputFilePath, Rev);
    }

    public static ChangelogSettings FromJson(string json)
    {
        return Git2SemVerJsonSerializer.Deserialise<ChangelogSettings>(json);
    }
}