﻿using Microsoft.Build.Framework;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;
using NoeticTools.MSBuild.Tasking.Logging;


// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NoeticTools.Git2SemVer.MSBuild.Tasks;

/// <summary>
///     The Git2SemVer MSBuild task to get the package version from the properties file.
/// </summary>
public class Git2SemVerRestoreVersionPropertiesTask : Git2SemVerTaskBase
{
    /// <summary>
    ///     Path to the projects intermediate files directory (ob/).
    ///     Defaults to the MSBuild
    ///     <see
    ///         href="https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2022#list-of-common-properties-and-parameters">
    ///         BaseIntermediateOutputPath
    ///     </see>
    ///     property.
    /// </summary>
    [Required]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string Input_VersionCacheDirectory { get; set; } = "";

    /// <summary>
    ///     Called by MSBuild to execute the task.
    /// </summary>
    public override bool Execute()
    {
        var logger = new MSBuildTaskLogger(Log);
        try
        {
            logger.LogDebug("Restoring version properties.");
            var cache = new GeneratedVersionsJsonFile().Load(Input_VersionCacheDirectory);
            SetOutputs(cache);
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