namespace NoeticTools.Git2SemVer.Framework.Versioning;

public interface IVersioningEngine : IDisposable
{
    /// <summary>
    ///     Get information including contributing conventional commits since last (direct) releases.
    /// </summary>
    /// <param name="versioningMode"></param>
    VersioningOutputs OutsideOfBuildRun(VersioningMode versioningMode);

    /// <summary>
    ///     Perform a prebuild versioning run. Depending on the host may bump the build number.
    /// </summary>
    /// <param name="versioningMode"></param>
    VersioningOutputs PrebuildRun(VersioningMode versioningMode);
}