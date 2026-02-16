using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Tools.CI;


namespace NoeticTools.Git2SemVer.Framework;

[ExcludeFromCodeCoverage]
public sealed class ProjectVersioningFactory(
    Action<string> buildOutput,
    VersioningEngineFactory versioningEngineFactory,
    ILogger logger)
{
    public ProjectVersioning Create(IVersionGeneratorInputs inputs,
                                    IOutputsJsonIO? outputsJsonIO = null,
                                    IConfiguration? config = null)
    {
        if (inputs == null)
        {
            throw new ArgumentNullException(nameof(inputs), "Inputs is required.");
        }

        outputsJsonIO ??= new OutputsJsonFileIO();
        config ??= Git2SemVerConfiguration.Load();

        var host = new BuildHostFactory(config, buildOutput, logger).Create(inputs.HostType,
                                                                            inputs.BuildNumber,
                                                                            inputs.BuildContext,
                                                                            inputs.BuildIdFormat);
        var convCommitSettings = new ConventionalCommitsSettings();
        var versionGenerator = versioningEngineFactory.Create(inputs, outputsJsonIO, host, convCommitSettings);
        var projectVersioning = new ProjectVersioning(inputs, host,
                                                      outputsJsonIO,
                                                      versionGenerator,
                                                      logger);
        return projectVersioning;
    }
}