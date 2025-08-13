using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Tools.CI;
using NoeticTools.Git2SemVer.Framework.Versioning;
using NoeticTools.Git2SemVer.Framework.Versioning.Builders.Scripting;


namespace NoeticTools.Git2SemVer.Framework;

[ExcludeFromCodeCoverage]
[RegisterTransient]
public sealed class ProjectVersioningFactory(
    IVersionGeneratorInputs inputs,
    IMSBuildGlobalProperties msBuildGlobalProperties,
    IVersioningEngineFactory versioningEngineFactory,
    IBuildHostFactory buildHostFactory,
    ILogger logger)
{
    public ProjectVersioning Create(IOutputsJsonIO? outputsJsonIO = null)
    {
        if (inputs == null)
        {
            throw new ArgumentNullException(nameof(inputs), "Inputs is required.");
        }

        outputsJsonIO ??= new VersioningOutputsJsonFileIO();

        var host = buildHostFactory.Create(inputs.HostType,
                                           inputs.BuildNumber,
                                           inputs.BuildContext,
                                           inputs.BuildIdFormat);
        var convCommitSettings = new ConventionalCommitsSettings();
        var versionGenerator = versioningEngineFactory.Create(inputs, 
                                                              msBuildGlobalProperties, 
                                                              outputsJsonIO, 
                                                              host, 
                                                              convCommitSettings);
        var projectVersioning = new ProjectVersioning(inputs, 
                                                      host,
                                                      outputsJsonIO,
                                                      versionGenerator,
                                                      logger);
        return projectVersioning;
    }
}