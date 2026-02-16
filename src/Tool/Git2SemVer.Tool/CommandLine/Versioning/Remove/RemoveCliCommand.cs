using NoeticTools.Git2SemVer.Tool.CommandLine.Changelog;
using Spectre.Console.Cli;


// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Remove;

internal class RemoveCliCommand : CliCommandBase<SolutionCommandSettings>
{
    protected override int Execute(CommandContext context, SolutionCommandSettings settings, CancellationToken cancellationToken)
    {
        Validate(context);
        if (!settings.Validate().Successful)
        {
            return (int)ExitCodes.InvalidCommandSettingsError;
        }

        var commandFactory = GetCommandFactory(context, settings);

        var runner = commandFactory.CreateRemoveCommand();
        runner.Execute(settings.SolutionName, settings.Unattended);
        return (int)(runner.HasError ? ReturnCodes.CommandError : ReturnCodes.Succeeded);
    }
}