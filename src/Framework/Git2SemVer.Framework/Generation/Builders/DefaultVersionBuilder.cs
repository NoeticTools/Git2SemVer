using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation.Builders;

/// <summary>
///     Git2SemVer's default outputs builder. This builder sets all MSBuild output properties.
/// </summary>
internal sealed class DefaultVersionBuilder(ILogger logger) : IVersionBuilder
{
    /// <summary>
    ///     Build versioning outputs from found git history path to prior releases.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="gitTool"></param>
    /// <param name="inputs"></param>
    /// <param name="outputs"></param>
    public void Build(IBuildHost host, IGitTool gitTool, IVersionGeneratorInputs inputs, IVersionOutputs outputs)
    {
        logger.LogDebug("Running default (built-in) version builder.");
        using (logger.EnterLogScope())
        {
            var prereleaseLabel = GetPrereleaseLabel(inputs, outputs);

            var version = GetVersion(outputs, prereleaseLabel, host);
            var informationalVersion = GetInformationalVersion(version, host, outputs);
            outputs.SetAllVersionPropertiesFrom(informationalVersion,
                                                host.BuildNumber,
                                                host.BuildContext);

            outputs.BuildSystemVersion = GetBuildSystemLabel(host, prereleaseLabel, version);

            if (logger.Level >= LoggingLevel.Trace)
            {
                logger.LogTrace(outputs.GetReport());
            }
        }
    }

    private static SemVersion GetBuildSystemLabel(IBuildHost host, string prereleaseLabel, SemVersion version)
    {
        var buildSystemLabel = version.IsRelease
            ? version.WithMetadata(host.BuildNumber)
            : version.WithPrerelease(prereleaseLabel, host.BuildId.ToArray());
        return buildSystemLabel;
    }

    private static SemVersion GetInformationalVersion(SemVersion version, IBuildHost host, IVersionOutputs outputs)
    {
        var commitId = outputs.Git.HeadCommit.CommitId.Sha;
        var branchName = outputs.Git.BranchName.ToNormalisedSemVerIdentifier();
        var metadata = new List<string>();
        if (version.IsRelease)
        {
            metadata.AddRange(host.BuildId);
        }

        metadata.AddRange([branchName, commitId]);
        return version.WithMetadata(metadata.ToArray());
    }

    private string GetPrereleaseLabel(IVersionGeneratorInputs inputs, IVersionOutputs outputs)
    {
        var initialDevSuffix = "";
        if (outputs.Version!.Major == 0)
        {
            initialDevSuffix = VersioningConstants.InitialDevelopmentLabel;
        }

        if (VersioningConstants.ReleaseGroupName.Equals(inputs.VersionSuffix,
                                                        StringComparison.Ordinal))
        {
            return initialDevSuffix;
        }

        var prereleaseLabel = string.IsNullOrWhiteSpace(inputs.VersionSuffix)
            ? GetPrereleaseLabelFromBranchName(inputs, outputs)
            : inputs.VersionSuffix;
        if (!string.IsNullOrWhiteSpace(prereleaseLabel) &&
            !string.IsNullOrWhiteSpace(initialDevSuffix))
        {
            initialDevSuffix = "-" + initialDevSuffix;
        }

        return prereleaseLabel + initialDevSuffix;
    }

    private static string GetPrereleaseLabelFromBranchName(IVersionGeneratorInputs inputs, IVersionOutputs outputs)
    {
        var branchName = outputs.Git.BranchName;
        var pattern = string.IsNullOrWhiteSpace(inputs.BranchMaturityPattern)
            ? VersioningConstants.DefaultBranchMaturityPattern
            : inputs.BranchMaturityPattern;
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        var match = regex.Match(branchName);

        var groupNames = regex.GetGroupNames();
        foreach (var groupName in groupNames)
        {
            if (char.IsDigit(groupName[0]))
            {
                continue;
            }

            var group = match.Groups[groupName];
            if (group.Success)
            {
                return "release".Equals(groupName, StringComparison.Ordinal) ? "" : groupName.ToNormalisedSemVerIdentifier();
            }
        }

        return "UNKNOWN_BRANCH";
    }

    private SemVersion GetVersion(IVersionOutputs outputs, string prereleaseLabel, IBuildHost host)
    {
        var isARelease = string.IsNullOrWhiteSpace(prereleaseLabel);
        if (isARelease)
        {
            return outputs.Version!;
        }

        var prereleaseIdentifiers = new List<string> { prereleaseLabel };
        prereleaseIdentifiers.AddRange(host.BuildId);
        return outputs.Version!.WithPrerelease(prereleaseIdentifiers.ToArray());
    }
}