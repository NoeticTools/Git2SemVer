using Microsoft.Build.Framework;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Framework.Config;
using NoeticTools.Git2SemVer.MSBuild.Tools.CI;
using NoeticTools.Git2SemVer.MSBuild.Versioning;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;
using NoeticTools.MSBuild.TaskLogging;
using ILogger = NoeticTools.Common.Logging.ILogger;


// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NoeticTools.Git2SemVer.MSBuild.Tasks;

/// <summary>
///     The Git2SemVer MSBuild task.
/// </summary>
/// <remarks>
///     <para>
///         This class exposed properties for inputs from the MSBuild environment and outputs to the environment.
///     </para>
/// </remarks>
public class Git2SemVerGenerateVersionTask : Git2SemVerTaskBase
{
    /// <summary>
    ///     Optional case-insensitive regular expression that maps branch name to build maturity such as "release" or "beta".
    /// </summary>
    public string Input_BranchMaturityPattern { get; set; } = "";

    /// <summary>
    ///     The optional MSBuild <c>Git2SemVer_BuildContext</c> property value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If set, used to override the <see cref="IBuildHost.BuildContext" /> property.
    ///     </para>
    /// </remarks>
    public string Input_BuildContext { get; set; } = "";

    /// <summary>
    ///     Optional build ID format.
    /// </summary>
    /// <remarks>
    ///     If set, overrides the host object's <see cref="IBuildHost.BuildIdFormat">BuildIdFormat</see> property to set it
    ///     formats the value of <see cref="IBuildHost.BuildId">BuildId</see>.
    ///     Intended to be used with <c>Custom</c> build host type but can be used with any host.
    /// </remarks>
    public string Input_BuildIDFormat { get; set; } = "";

    /// <summary>
    ///     Optional input MSBuild <c>Git2SemVer_BuildContext</c> property value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If set, used to override the <see cref="IBuildHost.BuildNumber" /> property.
    ///     </para>
    ///     <para>
    ///         <b>TeamCity</b>
    ///     </para>
    ///     <para>
    ///         TeamCity provides the build number in an environment variable <b>BUILD_NUMBER</b> which is is automatically
    ///         read.
    ///     </para>
    ///     <para>
    ///         <b>GitHub</b>
    ///     </para>
    ///     <para>
    ///         Currently GitHub does not provide a build number. However, a unique build ID can be constructed from
    ///         <c>github.run_number</c> and <c>github.run_attempt</c>:
    ///     </para>
    ///     <code>
    ///     Unique build ID = "&lt;github.run_number&gt;-&lt;github.run_attempt&gt;"
    /// </code>
    ///     This can be passed to the build in the GitHub workflow yml like this:
    ///     <code>
    ///    - name: Build NetVersionBuilder
    ///      env:
    ///        BUILD_NUMBER: ${{ github.run_number }}-${{ github.run_attempt }}
    ///      run: |
    ///        dotnet build -p:BuildNumber=${{ env.BUILD_NUMBER }}
    /// </code>
    /// </remarks>
    public string Input_BuildNumber { get; set; } = "";

    /// <summary>
    ///     Required path the C# script file to run.
    /// </summary>
    [Required]
    public string Input_BuildScriptPath { get; set; } = "";

    /// <summary>
    ///     Path to directory to write versioning report to.
    ///     Defaults to the MSBuild
    ///     <see
    ///         href="https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2022#list-of-common-properties-and-parameters">
    ///         BaseIntermediateOutputPath
    ///     </see>
    ///     property.
    /// </summary>
    [Required]
    public string Input_Env_IntermediateOutputDirectory { get; set; } = "";

    /// <summary>
    ///     This project's versioning mode. For internal use only.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If Git2SemVer was installed using the dotnet tool then solution versioning
    ///         is being used and this property is set to either <c>SolutionClientProject</c> or
    ///         <c>SolutionVersioningProject</c>.
    ///     </para>
    ///     <para>
    ///         Otherwise, this project is versioning itself independent of any other solution projects and
    ///         this property is set to <c>StandAloneProject</c>.
    ///     </para>
    ///     <para>
    ///         See also: <seealso cref="VersioningModeEnum" />
    ///     </para>
    /// </remarks>
    public string Input_Env_Mode { get; set; } = "";

    /// <summary>
    ///     When using solution versioning the shared directory that holds generated version properties.
    /// </summary>
    /// <remarks>
    ///     Not used if the <see cref="Input_Env_Mode" /> is <c>"StandAloneProject"</c>. Otherwise, required.
    /// </remarks>
    public string Input_Env_SharedDirectory { get; set; } = "";

    /// <summary>
    ///     When using solution versioning the path to the shared generated version properties file.
    /// </summary>
    /// <remarks>
    ///     Not used if the <see cref="Input_Env_Mode" /> is <c>"StandAloneProject"</c>. Otherwise, required.
    /// </remarks>
    public string Input_Env_SharedVersioningPropsFile { get; set; } = "";

    /// <summary>
    ///     The working directory.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is the project's directory.
    ///     </para>
    /// </remarks>
    [Required]
    public string Input_Env_WorkingDirectory { get; set; } = "";

    /// <summary>
    ///     Optional input MSBuild <c>Git2SemVer_HostType</c> property.
    ///     If set overrides automatic host type detection.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IBuildHost.HostTypeId" />.
    ///     </para>
    /// </remarks>
    public string Input_HostType { get; set; } = "";

    /// <summary>
    ///     Optional MSBuild <c>Git2SemVer_RunScript</c> property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If set to <c>false</c> prevents the script from being executed.
    ///         If set to <c>true</c> ensure the script is executed and fail the build if the script is not present.
    ///         If <c>null</c> (not set) the script will execute if present but the build will not fail if not present.
    ///         The default is <c>null</c>.
    ///     </para>
    /// </remarks>
    public bool? Input_RunScript { get; set; }

    /// <summary>
    ///     The optional MSBuild <c>Git2SemVer_ScriptArg</c> property.
    /// </summary>
    public string Input_ScriptArgs { get; set; } = "";

    /// <summary>
    ///     The optional MSBuild <c>Git2SemVer_UpdateHostBuildLabel</c> property.
    /// </summary>
    public bool Input_UpdateHostBuildLabel { get; set; }

    /// <summary>
    ///     MSBuild's <see href="https://gist.github.com/jonlabelle/34993ee032c26420a0943b1c9d106cdc#version">Version</see>
    ///     property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Not used by Git2SemVer but include for optional use in C# script.
    ///     </para>
    /// </remarks>
    public string Input_Version { get; set; } = "";

    /// <summary>
    ///     Optional MSBuild
    ///     <see href="https://gist.github.com/jonlabelle/34993ee032c26420a0943b1c9d106cdc#versionsuffix">VersionSuffix</see>
    ///     property value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If set to <c>release</c> (case-insensitive) forces the version to be a release.
    ///         Otherwise, if set, forces a prerelease with this as the prerelease label.
    ///     </para>
    /// </remarks>
    public string Input_VersionSuffix { get; set; } = "";

    /// <summary>
    ///     Called by MSBuild to execute the task.
    /// </summary>
    public override bool Execute()
    {
        var logger = new CompositeLogger() { Level = LoggingLevel.Trace };
        logger.Add(new MSBuildTaskLogger(Log) {Level = LoggingLevel.Trace});
        var logFilePath = Path.Combine(Input_Env_IntermediateOutputDirectory, "Git2SemVer.MSBuild.log");
        logger.Add(new FileLogger(logFilePath) {Level = LoggingLevel.Trace});

        try
        {
            logger.LogDebug("Executing Git2SemVer.MSBuild task to generate version.");

            var config = Git2SemVerConfiguration.Load();
            var inputs = GetGeneratorInputs();
            var host = new BuildHostFactory(config, logger).Create(inputs.HostType,
                                                                   inputs.BuildNumber,
                                                                   inputs.BuildContext,
                                                                   inputs.BuildIdFormat);
            var gitTool = new GitTool(logger)
            {
                WorkingDirectory = inputs.WorkingDirectory
            };
            var commitsRepo = new CommitsRepository(gitTool);
            var gitPathsFinder = new PathsFromLastReleasesFinder(commitsRepo, gitTool, logger);

            var defaultBuilderFactory = new DefaultVersionBuilderFactory(logger);
            var scriptBuilder = new ScriptVersionBuilder();
            var versionGenerator = new VersionGenerator(inputs, host, new GeneratedOutputsFile(), gitTool, gitPathsFinder, defaultBuilderFactory,
                                                        scriptBuilder, logger);
            SetOutputs(versionGenerator.Run());
            return !Log.HasLoggedErrors;
        }
        catch (Exception exception)
        {
            logger.LogError(exception);
            //Log.LogErrorFromException(exception);
            return false;
        }
        finally
        {
            logger.Dispose();
        }
    }

    private VersionGeneratorInputs GetGeneratorInputs()
    {
        return new VersionGeneratorInputs(Input_Env_Mode,
                                          Input_Version, Input_VersionSuffix,
                                          Input_BuildNumber, Input_BuildContext,
                                          Input_BuildIDFormat,
                                          Input_UpdateHostBuildLabel,
                                          Input_HostType,
                                          Input_RunScript,
                                          Input_BuildScriptPath,
                                          Input_ScriptArgs,
                                          Input_BranchMaturityPattern,
                                          Input_Env_WorkingDirectory,
                                          Input_Env_IntermediateOutputDirectory,
                                          Input_Env_SharedDirectory,
                                          Input_Env_SharedVersioningPropsFile,
                                          BuildEngine,
                                          BuildEngine9);
    }
}