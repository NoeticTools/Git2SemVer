using Microsoft.Build.Framework;
using NoeticTools.Git2SemVer.Framework.Generation;
using ILogger = NoeticTools.Git2SemVer.Core.Logging.ILogger;


// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace NoeticTools.Git2SemVer.IntegrationTests.Framework;

public class VersionGeneratorInputsStub : IVersionGeneratorInputs
{
    public string BranchMaturityPattern { get; set; } = "";

    public string BuildContext { get; set; } = string.Empty;

    public IBuildEngine5 BuildEngine9 { get; set; } = new BuildEngine9Stub(new Dictionary<string, string>());

    public string BuildIdFormat { get; } = "";

    public string BuildNumber { get; set; } = "";

    public string BuildScriptPath { get; set; } = "";

    public string HostType { get; set; } = "";

    public string IntermediateOutputDirectory { get; } = "";

    public string ReleaseTagFormat { get; set; } = "";

    public bool? RunScript { get; set; }

    public string ScriptArgs { get; set; } = "";

    public string SolutionSharedDirectory { get; set; } = "";

    public string SolutionSharedVersioningPropsFile { get; set; } = "";

    public bool UpdateHostBuildLabel { get; set; }

    public string Version { get; set; } = "";

    public VersioningMode VersioningMode { get; set; }

    public string VersionSuffix { get; set; } = "";

    public string WorkingDirectory { get; set; } = "";

    public bool WriteConventionalCommitsInfo { get; } = false;

    public bool ValidateScriptInputs(ILogger logger)
    {
        return true;
    }
}