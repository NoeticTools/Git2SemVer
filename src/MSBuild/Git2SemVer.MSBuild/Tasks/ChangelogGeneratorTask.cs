using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.FileSystem;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework;
using NoeticTools.Git2SemVer.Framework.ChangeLog;


namespace NoeticTools.Git2SemVer.MSBuild.Tasks;

[RegisterTransient]
public sealed class ChangelogGeneratorTask(IChangeLogGeneratorTaskOptions taskOptions, ILogger logger)
{
    public void Execute(VersioningOutputs versioningOutput)
    {
        Git2SemVerArgumentException.ThrowIfNull(versioningOutput, nameof(versioningOutput));

        if (!versioningOutput.Metadata.CalculationPerformed)
        {
            logger.LogDebug("Changelog generation skipped as no versioning data not calculated.");
            return;
        }

        logger.LogInfo("Generating changelog.");

        new ChangelogGenerator(logger).Execute(versioningOutput,
                                               taskOptions.ChangelogDataDirectory,
                                               taskOptions.WorkingDirectory,
                                               noFileWrites: false);
    }
}