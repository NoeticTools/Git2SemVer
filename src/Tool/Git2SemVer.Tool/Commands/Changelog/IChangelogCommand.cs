﻿using NoeticTools.Git2SemVer.Tool.CommandLine;


namespace NoeticTools.Git2SemVer.Tool.Commands.Changelog;

internal interface IChangelogCommand
{
    bool HasError { get; }

    void Execute(ChangelogCommandSettings settings);
}