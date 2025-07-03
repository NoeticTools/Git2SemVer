﻿using Microsoft.Extensions.DependencyInjection;
using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Tool.Commands;
using NoeticTools.Git2SemVer.Tool.Commands.Changelog;
using Spectre.Console.Cli;


// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine;

internal class ChangelogCliCommand : Command<ChangelogCommandSettings>
{
    public override int Execute(CommandContext context, ChangelogCommandSettings settings)
    {
        var serviceProvider = (IServiceProvider)context.Data!;
        var console = serviceProvider.GetService<IConsoleIO>()!;
        var commandFactory = serviceProvider.GetService<ICommandFactory>()!;

        console.Unattended = settings.Unattended;
        var runner = commandFactory.CreateChangelogCommand();
        runner.Execute(settings);
        return (int)(runner.HasError ? ReturnCodes.CommandError : ReturnCodes.Succeeded);
    }
}