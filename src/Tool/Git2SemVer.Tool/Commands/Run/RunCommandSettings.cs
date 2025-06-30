﻿using System.ComponentModel;
using NoeticTools.Git2SemVer.Tool.CommandLine;
using Spectre.Console.Cli;


// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.Commands.Run;

public class RunCommandSettings : CommonCommandSettings
{
    [CommandOption("--enable-json-write")]
    [DefaultValue(false)]
    [Description("Enables writing generated versions to file 'Git2SemVer.VersionInfo.g.json'.")]
    public bool EnableJsonFileWrite { get; set; }

    [CommandOption("--host-type <TYPE>")]
    [Description("Force the host type. Use for testing expected behaviour on other hosts. Valid values are 'Custom', 'Uncontrolled', 'TeamCity', or 'GitHub'.")]
    public string? HostType { get; set; } = null;

    [CommandOption("-o|--output <DIRECTORY>")]
    [DefaultValue("")]
    [Description("Directory in which to place the generated version JSON file and the build log.")]
    public string OutputDirectory { get; set; } = "";

    [CommandOption("-v|--verbosity <LEVEL>")]
    [DefaultValue("info")]
    [Description("Sets output verbosity. Valid values are 'trace', 'debug', 'info', 'warning', or 'error'.")]
    public string Verbosity { get; set; } = "";
}