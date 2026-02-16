using System.ComponentModel;
using Spectre.Console.Cli;


// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine;

internal class CommonCommandSettings : CommandSettings
{
    [CommandOption("-u|--unattended")]
    [Description("Run unattended. If used, does not ask user before operation is performed and choices defaults are used.")]
    public bool Unattended { get; set; }
}