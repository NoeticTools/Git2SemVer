﻿using Microsoft.Build.Framework;
using NoeticTools.Common.Exceptions;
using ILogger = NoeticTools.Common.Logging.ILogger;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;

internal class VersionGeneratorInputs : IVersionGeneratorInputs
{
    public VersionGeneratorInputs(string mode,
                                  string version,
                                  string versionSuffix,
                                  string buildNumber,
                                  string buildContext,
                                  string inputBuildIdFormat,
                                  bool updateHostBuildLabel,
                                  string hostType,
                                  bool? runScript,
                                  string buildScriptPath,
                                  string scriptArgs,
                                  string branchMaturityPattern,
                                  string gitDirectory,
                                  string intermediateOutputPath,
                                  string solutionSharedDirectory,
                                  string solutionSharedVersioningPropsFile,
                                  IBuildEngine buildEngine,
                                  IBuildEngine9 buildEngine9)
    {
        BuildEngine = buildEngine;
        BuildEngine9 = buildEngine9;
        BuildContext = buildContext;
        BuildIdFormat = inputBuildIdFormat;
        BuildNumber = buildNumber;
        UpdateHostBuildLabel = updateHostBuildLabel;
        HostType = hostType;
        BuildScriptPath = buildScriptPath;
        ScriptArgs = scriptArgs;
        WorkingDirectory = gitDirectory;
        Version = version;
        VersionSuffix = versionSuffix;
        IntermediateOutputDirectory = intermediateOutputPath;
        SolutionSharedDirectory = solutionSharedDirectory;
        SolutionSharedVersioningPropsFile = solutionSharedVersioningPropsFile;
        RunScript = runScript ?? true;
        BranchMaturityPattern = branchMaturityPattern;
        try
        {
            Mode = (VersioningModeEnum)Enum.Parse(typeof(VersioningModeEnum), mode);
        }
        catch (Exception exception)
        {
            throw new Git2SemVerConfigurationException($"Invalid Git2SemVer_Mode value '{mode}'.", exception);
        }
    }

    /// <inheritdoc />
    public string BranchMaturityPattern { get; }

    /// <inheritdoc />
    public string BuildContext { get; }

    /// <inheritdoc />
    public IBuildEngine BuildEngine { get; }

    /// <inheritdoc />
    public IBuildEngine9 BuildEngine9 { get; }

    public string BuildIdFormat { get; }

    public string BuildNumber { get; }

    public string BuildScriptPath { get; }

    public string HostType { get; }

    public string IntermediateOutputDirectory { get; }

    public VersioningModeEnum Mode { get; set; }

    public bool? RunScript { get; }

    public string ScriptArgs { get; }

    public string SolutionSharedDirectory { get; }

    public string SolutionSharedVersioningPropsFile { get; }

    public bool UpdateHostBuildLabel { get; }

    public string Version { get; }

    public string VersionSuffix { get; }

    public string WorkingDirectory { get; }

    public bool Validate(ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(BuildScriptPath))
        {
            logger.LogError($"The script file path (property {nameof(BuildScriptPath)}) is required.");
            return false;
        }

        if (RunScript is true && !File.Exists(BuildScriptPath))
        {
            logger.LogError($"The required build script file '{BuildScriptPath}' was not found.");
            return false;
        }

        return true;
    }
}