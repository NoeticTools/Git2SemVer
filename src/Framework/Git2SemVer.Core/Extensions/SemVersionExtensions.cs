using NoeticTools.Git2SemVer.Core.ConventionCommits;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Extensions;

public static class SemVersionExtensions
{
    /// <summary>
    ///     Bump version according to Semantic Versioning specification.
    /// </summary>
    public static SemVersion Bump(this SemVersion lastReleased, ApiChangeFlags changeFlags)
    {
        if (changeFlags.BreakingChange)
        {
            return new SemVersion(lastReleased.Major + 1, 0, 0);
        }

        if (changeFlags.FunctionalityChange)
        {
            return new SemVersion(lastReleased.Major, lastReleased.Minor + 1, 0);
        }

        return new SemVersion(lastReleased.Major, lastReleased.Minor, lastReleased.Patch + 1);
    }

    /// <summary>
    ///     Retrieves the Git SHA from the metadata identifiers of the specified semantic version.
    /// </summary>
    /// <remarks>
    ///     This method assumes that the Git SHA, if present, is stored as the last metadata identifier
    ///     in the semantic version. The returned SHA must be exactly 40 characters long to be considered valid.
    /// </remarks>
    /// <param name="semVersion">The semantic version instance from which to extract the Git SHA.</param>
    /// <returns>
    ///     The Git SHA as a 40-character string if the last metadata identifier is a valid SHA;  otherwise, an empty
    ///     string.
    /// </returns>
    public static string GetGitFullSha(this SemVersion semVersion)
    {
        if (semVersion.MetadataIdentifiers.Count == 0)
        {
            return string.Empty;
        }

        var shaIdentifier = semVersion.MetadataIdentifiers.Last();
        return shaIdentifier.Value.Length != 40 ? string.Empty : shaIdentifier.Value;
    }

    public static SemVersion WithAbbreviatedGitSha(this SemVersion semver)
    {
        var sha = semver.GetGitFullSha();
        if (string.IsNullOrEmpty(sha))
        {
            return semver;
        }

        // Abbreviate the SHA to 7 characters.
        // ReSharper disable once ReplaceSubstringWithRangeIndexer
        var abbreviatedSha = sha.Length > 7 ? sha.Substring(0, 7) : sha;

        var newMetadata = semver.MetadataIdentifiers
                                .Take(semver.MetadataIdentifiers.Count - 1)
                                .Append(new MetadataIdentifier(abbreviatedSha));
        return semver.WithMetadata(newMetadata);
    }
}