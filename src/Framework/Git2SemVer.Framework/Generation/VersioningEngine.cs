using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.ChangeLogging;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Generation.Builders;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.Framework.Persistence;


namespace NoeticTools.Git2SemVer.Framework.Generation;

internal sealed class VersioningEngine(
    IVersionGeneratorInputs inputs,
    IBuildHost host,
    IOutputsJsonIO generatedOutputsJsonFile,
    IGitTool gitTool,
    IGitHistoryWalker gitWalker,
    IDefaultVersionBuilderFactory defaultVersionBuilderFactory,
    ILogger logger)
    : IVersioningEngine
{
    public void Dispose()
    {
        gitTool.Dispose();
    }

    public ConventionalCommitsVersionInfo GetConventionalCommitsInfo()
    {
        var (outputs, results) = GetVersionOutputs();
        return new ConventionalCommitsVersionInfo(outputs, results.Contributing);
    }

    public IVersionOutputs PrebuildRun()
    {
        var stopwatch = Stopwatch.StartNew();

        host.BumpBuildNumber();
        var (outputs, _) = GetVersionOutputs();

        SaveGeneratedVersions(outputs);

        stopwatch.Stop();

        logger.LogInfo($"Informational version: {outputs.InformationalVersion}");
        logger.LogDebug($"Version generation completed (in {stopwatch.Elapsed.TotalSeconds:F1} seconds).");
        host.ReportBuildStatistic("git2semver.runtime.seconds", stopwatch.Elapsed.TotalSeconds);

        return outputs;
    }

    private (VersionOutputs Outputs, SemanticVersionCalcResult Results) GetVersionOutputs()
    {
        var results = gitWalker.CalculateSemanticVersion();
        var outputs = new VersionOutputs(new GitOutputs(gitTool,
                                                        results.PriorReleaseVersion,
                                                        results.PriorReleaseCommitId,
                                                        results.PriorVersions),
                                         results.Version);

        using (logger.EnterLogScope())
        {
            var stopwatch = Stopwatch.StartNew();

            var defaultBuilder = defaultVersionBuilderFactory.Create(outputs.Version!);
            defaultBuilder.Build(host, gitTool, inputs, outputs);

            stopwatch.Stop();
            logger.LogDebug($"Version building completed (in {stopwatch.Elapsed.TotalSeconds:F1} sec).");
        }

        if (inputs.WriteConventionalCommitsInfo)
        {
            SaveConventionalCommitsInfo(outputs, results.Contributing);
        }

        return (outputs, results);
    }

    private void SaveConventionalCommitsInfo(VersionOutputs outputs, ContributingCommits contributing)
    {
        var conventionalCommitsInfo = new ConventionalCommitsVersionInfo(outputs, contributing);
        const string commitsInfoFilename = ChangelogConstants.DefaultConvCommitsInfoFilename;
        var filePath = Path.Combine(inputs.IntermediateOutputDirectory, commitsInfoFilename);
        conventionalCommitsInfo.Write(filePath);
        if (inputs.VersioningMode != VersioningMode.StandAloneProject)
        {
            logger.LogDebug("Saving conventional commits info file to '{0}'.", filePath);
            conventionalCommitsInfo.Write(Path.Combine(inputs.SolutionSharedDirectory, commitsInfoFilename));
        }
    }

    private void SaveGeneratedVersions(VersionOutputs outputs)
    {
        generatedOutputsJsonFile.Write(inputs.IntermediateOutputDirectory, outputs);
        if (inputs.VersioningMode != VersioningMode.StandAloneProject)
        {
            generatedOutputsJsonFile.Write(inputs.SolutionSharedDirectory, outputs);
        }
    }
}