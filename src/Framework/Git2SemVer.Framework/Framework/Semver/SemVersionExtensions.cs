using NoeticTools.Git2SemVer.Core.ConventionCommits;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Framework.Semver;

// ReSharper disable once ConvertToExtensionBlock

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
    /// Return an assembly version string for the given semantic version. The assembly version is in the format "major.minor.patch.0" as per .NET conventions, where the fourth segment is typically reserved for build or revision numbers and is set to 0 in this case.
    /// </summary>
    public static string ToAssemblyVersion(this SemVersion version)
    {
        return $"{version.ToFileVersion()}.0";
    }

    /// <summary>
    /// Converts the specified semantic version to a file version string in the format "Major.Minor.Patch".
    /// </summary>
    public static string ToFileVersion(this SemVersion version)
    {
        return $"{version.Major}.{version.Minor}.{version.Patch}";
    }
}