using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.Framework.Tools.CI;


namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;

internal abstract class CommandBase(IConsoleIO console)
{
    public bool HasError => Console.HasError;

    protected CompositeLogger CreateLogger(string verbosity = "info")
    {
        CompositeLogger? logger = null;
        try
        {
            logger = new CompositeLogger();
            //logger.Add(new NoDisposeLoggerDecorator(_logger));
            logger.Add(new ConsoleLogger());
            logger.Level = GetVerbosity(verbosity);
            return logger;
        }
        catch
        {
            logger?.Dispose();
            throw;
        }
    }

    protected static IBuildHost GetBuildHost(CompositeLogger logger, VersionGeneratorInputs inputs)
    {
        var config = Git2SemVerLocalSettings.Load();

        var host = new BuildHostFactory(config, new TeamCityLoggerWriterFactory(logger).Create(), logger)
            .Create(inputs.HostType,
                    inputs.BuildNumber,
                    inputs.BuildContext,
                    inputs.BuildIdFormat);
        return host;
    }

    protected LoggingLevel GetVerbosity(string verbosity)
    {
        if (Enum.TryParse(verbosity, true, out LoggingLevel level))
        {
            return level;
        }

        Console.WriteErrorLine($"Verbosity {verbosity} is not valid. Must be 'Trace', 'Debug', 'Info', 'Warning', or 'Error'.");
        return LoggingLevel.Info;
    }

    protected readonly IConsoleIO Console = console;
}