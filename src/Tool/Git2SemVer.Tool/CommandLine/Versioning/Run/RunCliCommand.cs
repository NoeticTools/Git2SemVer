using NoeticTools.Git2SemVer.Tool.CommandLine.Changelog;
using Spectre.Console.Cli;


// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;

internal sealed class RunCliCommand : CliCommandBase<RunCommandSettings>
{
    protected override int Execute(CommandContext context, RunCommandSettings settings, CancellationToken cancellationToken)
    {
        Validate(context);
        if (!settings.Validate().Successful)
        {
            return (int)ExitCodes.InvalidCommandSettingsError;
        }

        var commandFactory = GetCommandFactory(context, settings);

        var runner = commandFactory.CreateRunCommand();
        runner.Execute(settings);
        return (int)(runner.HasError ? ReturnCodes.CommandError : ReturnCodes.Succeeded);
    }
}