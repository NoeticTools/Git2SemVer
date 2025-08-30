using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.ChangeLog;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Versioning.Builders;
using NoeticTools.Git2SemVer.Framework.Versioning.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Versioning.GitHistoryWalking;


namespace NoeticTools.Git2SemVer.Framework.Versioning;

internal sealed class VersioningEngine(
    IVersionGeneratorInputs inputs,
    IBuildHost host,
    IOutputsJsonIO generatedOutputsJsonFile,
    IGitTool gitTool,
    IGitHistoryWalker gitWalker,
    IDefaultVersionBuilderFactory defaultVersionBuilderFactory,
    IVersionBuilder scriptBuilder,
    IMSBuildGlobalProperties msBuildGlobalProperties,
    ILogger logger)
    : IVersioningEngine
{
    public void Dispose()
    {
        gitTool.Dispose();
    }

    public VersioningOutputs OutsideOfBuildRun(VersioningMode versioningMode)
    {
        return GetVersionOutputs(versioningMode);
    }

    public VersioningOutputs PrebuildRun(VersioningMode versioningMode)
    {
        var stopwatch = Stopwatch.StartNew();

        host.BumpBuildNumber();
        var outputs = GetVersionOutputs(versioningMode);

        SaveGeneratedVersions(outputs.Versions, versioningMode);

        stopwatch.Stop();

        logger.LogInfo($"Informational version: {outputs.Versions.InformationalVersion}");
        logger.LogDebug($"Version generation completed (in {stopwatch.Elapsed.TotalSeconds:F1} seconds).");
        host.ReportBuildStatistic("git2semver.runtime.seconds", stopwatch.Elapsed.TotalSeconds);

        return outputs;
    }

    private VersioningOutputs GetVersionOutputs(VersioningMode versioningMode)
    {
        var calcResult = gitWalker.CalculateSemanticVersion();
        var outputs = new VersionOutputs(new GitOutputs(gitTool,
                                                        calcResult.PriorReleaseVersion,
                                                        calcResult.PriorReleaseCommitId,
                                                        calcResult.PriorVersions),
                                         calcResult.Version);
        RunBuilders(outputs);

        if (inputs.WriteConventionalCommitsInfo)
        {
            SaveConventionalCommitsInfo(outputs, calcResult.Contributing, versioningMode);
        }

        return new VersioningOutputs(outputs, calcResult);
    }

    private void RunBuilders(VersionOutputs outputs)
    {
        logger.LogDebug("Running version builders.");
        using (logger.EnterLogScope())
        {
            var stopwatch = Stopwatch.StartNew();

            var defaultBuilder = defaultVersionBuilderFactory.Create(outputs.Version!);
            defaultBuilder.Build(host, gitTool, inputs, outputs, msBuildGlobalProperties);

            scriptBuilder.Build(host, gitTool, inputs, outputs, msBuildGlobalProperties);

            stopwatch.Stop();
            logger.LogDebug($"Version building completed (in {stopwatch.Elapsed.TotalSeconds:F1} sec).");
        }
    }

    private void SaveConventionalCommitsInfo(VersionOutputs outputs, ContributingCommits contributing, VersioningMode versioningMode)
    {
        var conventionalCommitsInfo = new ConventionalCommitsVersionInfo(outputs, contributing);
        var filePath = Path.Combine(inputs.IntermediateOutputDirectory, ChangelogConstants.DefaultConvCommitsInfoFilename);
        conventionalCommitsInfo.Save(filePath);
        if (versioningMode == VersioningMode.StandAloneProject)
        {
            return;
        }

        logger.LogDebug("Saving conventional commits info file to '{0}'.", filePath);
        conventionalCommitsInfo.Save(Path.Combine(inputs.SolutionSharedDirectory, ChangelogConstants.DefaultConvCommitsInfoFilename));
    }

    private void SaveGeneratedVersions(IVersionOutputs outputs, VersioningMode versioningMode)
    {
        generatedOutputsJsonFile.Write(inputs.IntermediateOutputDirectory, outputs);
        if (versioningMode == VersioningMode.StandAloneProject)
        {
            return;
        }

        generatedOutputsJsonFile.Write(inputs.SolutionSharedDirectory, outputs);
    }
}