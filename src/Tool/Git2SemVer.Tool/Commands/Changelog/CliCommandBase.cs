﻿using Microsoft.Extensions.DependencyInjection;
using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Tool.Commands.Versioning;
using Spectre.Console.Cli;


namespace NoeticTools.Git2SemVer.Tool.Commands.Changelog;

internal abstract class CliCommandBase<T> : Command<T> where T : CommandSettings
{
    protected static void Validate(CommandContext context)
    {
        var remainingParsed = context.Remaining.Parsed;
        if (remainingParsed.Any())
        {
            throw new Git2SemVerUnknownCommandOptionException($"Unknown command argument '{remainingParsed.First().Key}'");
        }
    }

    protected static ICommandFactory GetCommandFactory(CommandContext context, CommonCommandSettings settings)
    {
        var serviceProvider = (IServiceProvider)context.Data!;
        var console = serviceProvider.GetService<IConsoleIO>()!;
        console.Unattended = settings.Unattended;
        var commandFactory = serviceProvider.GetService<ICommandFactory>()!;
        return commandFactory;
    }
}