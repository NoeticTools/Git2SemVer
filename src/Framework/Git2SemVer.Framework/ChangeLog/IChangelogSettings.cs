using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

public interface IChangelogSettings
{
    /// <summary>
    ///     Changelog generation enabled flag.
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    ///     Optional url to a version's artifacts. May contain version placeholder '%VERSION%'.
    /// </summary>
    string ArtifactLinkPattern { get; set; }

    /// <summary>
    ///     Categories to include in the changelog.
    /// </summary>
    CategorySettings[] Categories { get; set; }

    ConventionalCommitsSettings ConvCommits { get; set; }

    /// <summary>
    ///     Path to generator's data and configuration files directory. It may be a relative or absolute path.
    /// </summary>
    string DataDirectory { get; set; }

    /// <summary>
    ///     Issue link format with issue ID as argument ({0}).
    /// </summary>
    /// <remarks>
    ///     Example:
    ///     <example>
    ///         "https://organisation-name/project-name/issues/{0}"
    ///     </example>
    /// </remarks>
    string IssueLinkFormat { get; set; }

    /// <summary>
    ///     Generated changelog file path. It may be a relative or absolute path. Set to empty string to disable file write.
    /// </summary>
    string OutputFilePath { get; set; }
}