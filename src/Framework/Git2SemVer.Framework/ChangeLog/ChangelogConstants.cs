using NoeticTools.Git2SemVer.Core.FileSystem;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

public static class ChangelogConstants
{
    /// <summary>
    ///     The default url to a version's artifacts using version placeholder '%VERSION%'. No link is generated if an empty
    ///     string.
    /// </summary>
    public const string DefaultArtifactLinkPattern = "";

    public const string DefaultConvCommitsInfoFilename = "conventionalcommits.data.g.json";

    public const string DefaultDataDirectory = ".git2semver";

    public const string DefaultFilename = "CHANGELOG.md";

    public const string DefaultLogLevel = "info";

    public static readonly File DefaultMarkdownTemplateFilename = "changelog.markdown.template.scriban";

    public const string IssueLinkFormat = "{0}";

    public const string LastRunDataFileSuffix = ".data.g.json";

    public const string ProjectSettingsFilename = "git2semver.changelog.settings.json";

    public const string VersionPlaceholder = TagParsingConstants.VersionPlaceholder;
}