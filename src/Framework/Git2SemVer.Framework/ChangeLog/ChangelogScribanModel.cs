// ReSharper disable UnusedMember.Global

namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

internal sealed class ChangelogScribanModel
{
    public ChangelogScribanModel(ConventionalCommitsVersionInfo inputs,
                                 IReadOnlyList<ChangeCategory> categories,
                                 string releaseUrl,
                                 string forceReleaseAs)
    {
        Categories = categories;
        IsPrerelease = forceReleaseAs.Length == 0 && inputs.Version!.IsPrerelease;
        IsRelease = forceReleaseAs.Length > 0 || inputs.Version.IsRelease;
        var version = forceReleaseAs.Length > 0 ? forceReleaseAs : inputs.Version.ToString();
        SemVersion = version;
        ReleaseUrl = releaseUrl.Contains(ChangelogConstants.VersionPlaceholder)
            ? releaseUrl.Replace(ChangelogConstants.VersionPlaceholder, version)
            : releaseUrl;
    }

    /// <summary>
    ///     Categories to group changes into.
    /// </summary>
    public IReadOnlyList<ChangeCategory> Categories { get; }

    /// <summary>
    ///     True if generating a changelog for a prerelease version.
    /// </summary>
    public bool IsPrerelease { get; }

    /// <summary>
    ///     True if generating a changelog for a release version.
    /// </summary>
    public bool IsRelease { get; }

    /// <summary>
    ///     Format string to build URL to artifact.
    /// </summary>
    public string ReleaseUrl { get; }

    /// <summary>
    ///     Current semantic versioning version.
    /// </summary>
    public string SemVersion { get; }
}