﻿using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;


namespace NoeticTools.Git2SemVer.Tool.CommandLine;

internal class Git2SemVerCommandApp
{
    public int Execute(string[] args)
    {
        var servicesProvider = new Services().ConfigureServices();
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.SetApplicationName("Git2SemVer.Tool");
            config.UseAssemblyInformationalVersion();

            config.AddCommand<AddCliCommand>("add")
                  .WithDescription("Add Git2SemVer solution versioning to solution in working directory.")
                  .WithData(servicesProvider);
            config.AddCommand<RemoveCliCommand>("remove")
                  .WithDescription("Add Git2SemVer solution versioning to solution in working directory.")
                  .WithData(servicesProvider);
        });

        return app.Run(args);
    }
}