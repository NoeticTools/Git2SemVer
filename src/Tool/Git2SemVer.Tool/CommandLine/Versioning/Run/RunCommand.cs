using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Tools.CI;
using NoeticTools.Git2SemVer.Framework.Versioning;
using NoeticTools.Git2SemVer.Framework.Versioning.Builders.Scripting;


namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;

[RegisterSingleton]
internal sealed class RunCommand(IConsoleIO console) : CommandBase(console), IRunCommand
{
    public void Execute(RunCommandSettings settings)
    {
        Console.WriteMarkupInfoLine($"Running Git2SemVer version generator{(settings.Unattended ? " (unattended)" : "")}.");
        Console.WriteLine();

        var inputs = new VersionGeneratorInputs
        {
            IntermediateOutputDirectory = settings.OutputDirectory,
            WriteConventionalCommitsInfo = settings.EnableConvCommitsJsonWrite,
            ReleaseTagFormat = settings.ReleaseTagFormat!
        };

        if (!string.IsNullOrEmpty(settings.BranchMaturityPattern))
        {
            inputs.BranchMaturityPattern = settings.BranchMaturityPattern;
        }

        if (settings.HostType != null)
        {
            inputs.HostType = settings.HostType;
        }

#pragma warning disable CA2000
        using var logger = new CompositeLogger();
        logger.Add(new ConsoleLogger());
#pragma warning restore CA2000
        logger.Level = GetVerbosity(settings.Verbosity);

        IOutputsJsonIO outputJsonIO = settings.EnableJsonFileWrite ? new VersioningOutputsJsonFileIO() : new ReadOnlyOutputJsonIO();
        var versionGeneratorFactory = new VersioningEngineFactory(logger);
        var configuration = Git2SemVerLocalSettings.Load();
        var hostFactory = new BuildHostFactory(configuration, new TeamCityLoggerWriterFactory(logger).Create(), logger);
        var projectVersioning = new ProjectVersioningFactory(inputs, new NullMSBuildGlobalProperties(),
                                                             versionGeneratorFactory, hostFactory, logger)
            .Create(VersioningMode.StandAloneProject, outputsJsonIO: outputJsonIO);
        projectVersioning.Run(VersioningMode.StandAloneProject);

        Console.WriteMarkupInfoLine("");
        Console.WriteMarkupInfoLine("Completed");
    }
}