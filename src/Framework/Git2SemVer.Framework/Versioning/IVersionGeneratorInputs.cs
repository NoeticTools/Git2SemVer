using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;


namespace NoeticTools.Git2SemVer.Framework.Versioning;

/// <summary>
///     Inputs into the MSBuild task read from MSBuild properties.
/// </summary>
public interface IVersionGeneratorInputs : ICommonTaskOptions
{
    /// <summary>
    ///     Optional input  MSBuild <c>Git2SemVer_BranchMaturityPattern</c> property.
    ///     Sets the regular expression pattern that determines build maturity (prerelease label).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The default pattern is: <c>^((?&lt;release>main|release)|(?&lt;beta>feature)|(?&lt;alpha>.+))[\\/_]?</c>.
    ///     </para>
    ///     <para>
    ///         The groups are evaluated left to right, the first match is used.
    ///     </para>
    ///     <para>
    ///         The named group <c>release</c> is required. A match to this group results in a release version.
    ///         The named groups to the right can use any group name.
    ///         The group's name is the prerelease label to use.
    ///     </para>
    ///     <para>
    ///         A catch-all pattern at the end (e.g: <c>alpha</c>) is required.
    ///         If no match is found a prerelease label of "UNKNOWN_BRANCH" is used.
    ///     </para>
    /// </remarks>
    string BranchMaturityPattern { get; }

    /// <summary>
    ///     Build context. Provides context or extension to build number.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         MSBuild property: <c>Git2SemVer_BuildContext</c>.
    ///     </para>
    /// </remarks>
    string BuildContext { get; }

    string BuildIdFormat { get; }

    /// <summary>
    ///     Optional input MSBuild <c>Git2SemVer_BuildContext</c> property.
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
    string BuildNumber { get; }

    /// <summary>
    ///     Required path the C# script file to run.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         MSBuild property: <c>Git2SemVer_ScriptPath</c>.
    ///     </para>
    /// </remarks>
    string BuildScriptPath { get; }

    /// <summary>
    ///     Optional input MSBuild <c>Git2SemVer_HostType</c> property.
    ///     If set overrides automatic host type detection.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See the build object's <see cref="IBuildHost.HostTypeId">HostTypeId</see> property.
    ///     </para>
    /// </remarks>
    string HostType { get; }

    /// <summary>
    ///     The intermediate output directory (usually 'obj') that the build log and generated versions file are written to.
    /// </summary>
    string IntermediateOutputDirectory { get; }

    string ReleaseTagFormat { get; }

    /// <summary>
    ///     Optional MSBuild <c>Git2SemVer_RunScript</c> property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If set to <c>false</c> prevents the script from being executed.
    ///         If set to <c>true</c> ensure the script is executed and fail the build if the script is not present.
    ///         If <c>null</c> (not set) and the script will execute if present but the build will not fail if not present.
    ///         The default is <c>null</c>.
    ///     </para>
    /// </remarks>
    bool? RunScript { get; }

    /// <summary>
    ///     Optional arguments for script use.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         MSBuild property: <c>Git2SemVer_ScriptArg</c>.
    ///     </para>
    /// </remarks>
    string ScriptArgs { get; }

    string SolutionSharedDirectory { get; }

    string SolutionSharedVersioningPropsFile { get; }

    /// <summary>
    ///     Option passed to the script to request the script to update a build system's build label.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         MSBuild property: <c>Git2SemVer_UpdateHostBuildLabel</c>.
    ///     </para>
    /// </remarks>
    bool UpdateHostBuildLabel { get; }

    /// <summary>
    ///     Optional input from MSBuild
    ///     <see href="https://gist.github.com/jonlabelle/34993ee032c26420a0943b1c9d106cdc#version">Version</see> property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Not used by Git2SemVer but include for optional use in C# script.
    ///     </para>
    /// </remarks>
    // ReSharper disable once UnusedMember.Global
    string Version { get; }

    /// <summary>
    ///     Optional input from MSBuild
    ///     <see href="https://gist.github.com/jonlabelle/34993ee032c26420a0943b1c9d106cdc#versionsuffix">VersionSuffix</see>
    ///     property.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If set to <c>release</c> (case-insensitive) forces the version to be a release.
    ///         Otherwise, if set, forces a prerelease with this as the prerelease label.
    ///     </para>
    ///     <para>
    ///         If set, <see cref="BranchMaturityPattern" /> is not used.
    ///     </para>
    /// </remarks>
    string VersionSuffix { get; }

    /// <summary>
    ///     If true conventional commits info file is created. This can be used for changelog generation.
    /// </summary>
    /// <remarks>Default is false.</remarks>
    bool WriteConventionalCommitsInfo { get; }

    bool ValidateScriptInputs(ILogger logger);
}