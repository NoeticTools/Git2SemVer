using Microsoft.Build.Framework;
using NoeticTools.Git2SemVer.Framework.Persistence;


// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NoeticTools.Git2SemVer.MSBuild.Tasks;

/// <summary>
///     The Git2SemVer MSBuild task to get the package version from the properties file.
/// </summary>
// ReSharper disable once UnusedType.Global
public class Git2SemVerRestoreVersionPropertiesTask : Git2SemVerTaskBase
{
    /// <summary>
    ///     Path to the projects intermediate files directory (ob/).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         MSBuild task input.
    ///     </para>
    ///     <para>
    ///         Defaults to the MSBuild
    ///         <see
    ///             href="https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2022#list-of-common-properties-and-parameters">
    ///             BaseIntermediateOutputPath
    ///         </see>
    ///         property.
    ///     </para>
    /// </remarks>
    [Required]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string VersionCacheDirectory { get; set; } = "";

    /// <summary>
    ///     Called by MSBuild to execute the task.
    /// </summary>
    public override bool Execute()
    {
        var logger = new MSBuildTaskLogger(Log);
        try
        {
            logger.LogTrace("Restoring version properties.");
            var cache = new VersioningOutputsJsonFileIO().Read(VersionCacheDirectory);
            SetOutputs(cache);
            logger.LogDebug("Restored PackageVersion: {0}", PackageVersion);
            return !Log.HasLoggedErrors;
        }
#pragma warning disable CA1031
        catch (Exception exception)
#pragma warning restore CA1031
        {
            Log.LogErrorFromException(exception);
            return false;
        }
        finally
        {
            logger.Dispose();
        }
    }
}