using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Persistence;


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
            VersioningMode = VersioningMode.StandAloneProject,
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
        //logger.Add(new NoDisposeLoggerDecorator(_logger));
        logger.Add(new ConsoleLogger());
#pragma warning restore CA2000
        logger.Level = GetVerbosity(settings.Verbosity);

        IOutputsJsonIO outputJsonIO = settings.EnableJsonFileWrite ? new OutputsJsonFileIO() : new ReadOnlyOutputJsonIO();
        var versionGeneratorFactory = new VersioningEngineFactory(logger);
        var projectVersioning = new ProjectVersioningFactory(msg => logger.LogInfo(msg), versionGeneratorFactory, logger)
            .Create(inputs, outputJsonIO);
        projectVersioning.Run();

        Console.WriteMarkupInfoLine("");
        Console.WriteMarkupInfoLine("Completed");
    }
}