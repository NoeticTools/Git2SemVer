using NoeticTools.Git2SemVer.Framework.Versioning;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging.Task;

public interface IChangeLogGeneratorTaskOptions : ICommonTaskOptions
{
    /// <summary>
    ///     Optional changelog url to a version's artifacts. May contain version placeholder '%VERSION%'.
    /// </summary>
    string ChangelogArtifactLinkPattern { get; }

    /// <summary>
    ///     Path to changelog generator's data and configuration files directory. It may be a relative or absolute path.
    /// </summary>
    string ChangelogDataDirectory { get; } // todo - constant

    /// <summary>
    ///     Optional changelog generate/update enable.
    /// </summary>
    bool ChangelogEnable { get; }

    /// <summary>
    ///     Generated changelog file path. It may be a relative or absolute path. Set to empty string to disable file write.
    /// </summary>
    string ChangelogOutputFilePath { get; }

    /// <summary>
    ///     If not an empty string, sets the changelog's changes version (normally version or 'Unreleased'). Any text
    ///     permitted.
    /// </summary>
    string ChangelogReleaseAs { get; }
}