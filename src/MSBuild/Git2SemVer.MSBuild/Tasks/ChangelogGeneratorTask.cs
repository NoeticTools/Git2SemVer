using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework;
using NoeticTools.Git2SemVer.Framework.ChangeLogging;
using NoeticTools.Git2SemVer.Framework.ChangeLogging.Task;


namespace NoeticTools.Git2SemVer.MSBuild.Tasks;

[RegisterTransient]
public sealed class ChangelogGeneratorTask(IChangeLogGeneratorTaskOptions taskOptions, ILogger logger)
{
    public void Execute(VersioningOutputs versioningOutput)
    {
        Git2SemVerArgumentException.ThrowIfNull(versioningOutput, nameof(versioningOutput));

        if (!taskOptions.ChangelogEnable || !versioningOutput.Metadata.CalculationPerformed)
        {
            return;
        }

        logger.LogInfo("Generating changelog.");

        var projectSettings = ChangelogSettings.Load(taskOptions.ChangelogDataDirectory, ChangelogConstants.ProjectSettingsFilename);

        new ChangelogGenerator(projectSettings, logger).Execute(versioningOutput,
                                                                taskOptions.ChangelogArtifactLinkPattern,
                                                                taskOptions.ChangelogReleaseAs,
                                                                taskOptions.ChangelogDataDirectory,
                                                                taskOptions.ChangelogOutputFilePath, 
                                                                taskOptions.WorkingDirectory);
    }
}