using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.Versioning;


// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;

[ExcludeFromCodeCoverage(Justification = "Self evident")]
internal sealed class VersionGeneratorInputs : IVersionGeneratorInputs
{
    public string BranchMaturityPattern { get; set; } = "";

    public string BuildContext { get; } = "";

    public string BuildIdFormat { get; } = "";

    public string BuildNumber { get; } = "";

    public string BuildScriptPath { get; set; } = "";

    public string HostType { get; set; } = "";

    public string IntermediateOutputDirectory { get; set; } = "";

    public string ReleaseTagFormat { get; set; } = "";

    public bool? RunScript { get; set; }

    public string ScriptArgs { get; set; } = "";

    public string SolutionSharedDirectory { get; } = "";

    public string SolutionSharedVersioningPropsFile { get; } = "";

    public bool UpdateHostBuildLabel { get; set; }

    public string Version { get; } = "";

    public string VersionSuffix { get; } = "";

    public string WorkingDirectory { get; } = "";

    public bool WriteConventionalCommitsInfo { get; set; }

    public bool ValidateScriptInputs(ILogger logger)
    {
        return true;
    }
}