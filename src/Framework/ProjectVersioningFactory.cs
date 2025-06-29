﻿using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Tools.CI;


namespace NoeticTools.Git2SemVer.Framework;

[ExcludeFromCodeCoverage]
public sealed class ProjectVersioningFactory(Action<string> buildOutput, 
                                             VersionGeneratorFactory versionGeneratorFactory, ILogger logger)
{
    public ProjectVersioning Create(IVersionGeneratorInputs inputs, 
                                    IMSBuildGlobalProperties msBuildGlobalProperties,
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
        var versionGenerator = versionGeneratorFactory.Create(inputs, msBuildGlobalProperties, outputsJsonIO, host);
        var projectVersioning = new ProjectVersioning(inputs, host,
                                                      outputsJsonIO,
                                                      versionGenerator,
                                                      logger);
        return projectVersioning;
    }


}