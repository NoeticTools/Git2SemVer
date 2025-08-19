using System.ComponentModel;
using NoeticTools.Git2SemVer.Framework.ChangeLog;
using Spectre.Console;
using Spectre.Console.Cli;


// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine.Changelog;

public class ChangelogCommandSettings : CommonCommandSettings
{
    [CommandOption("-a|--artifact-url <URL>")]
    [Description("Optional url to a version's artifacts. May contain version placeholder '%VERSION%'.")]
    public string ArtifactLinkPattern { get; set; } = "";

    [CommandOption("-d|--data-directory <DIRECTORY>")]
    [DefaultValue(ChangelogConstants.DefaultDataDirectory)]
    [Description("Path to generator's data and configuration files directory. May be a relative or absolute path.")]
    public string DataDirectory { get; set; } = "";

    [CommandOption("--host-type <TYPE>")]
    [Description("Optional the host type. Use for testing expected behaviour on other hosts. Valid values are 'Custom', 'Uncontrolled', 'TeamCity', or 'GitHub'.")]
    public string? HostType { get; set; } = null;

    [CommandOption("--no-writes")]
    [Description("Optional. Disable file writes. Useful for testing to avoid changing existing Changelog files.")]
    public bool? NoFileWrites { get; set; }

    [CommandOption("-o|--output <FILEPATH>")]
    [DefaultValue(ChangelogConstants.DefaultFilename)]
    [Description("Generated changelog file path. May be a relative or absolute path.")]
    public string OutputFilePath { get; set; } = "";

    [CommandOption("-r|--release-as <TITLE>")]
    [DefaultValue("")]
    [Description("If not an empty string, sets the changes version (normally version or 'Unreleased'). Any text permitted.")]
    public string ReleaseAs { get; set; } = "";

    [CommandOption("-v|--verbosity <LEVEL>")]
    [DefaultValue("info")]
    [Description("Sets output verbosity. Valid values are 'trace', 'debug', 'info', 'warning', or 'error'.")]
    public string Verbosity { get; set; } = "";

    [CommandOption("-s|--show")]
    [Description("Show changelog in console.")]
    public bool WriteToConsole { get; set; }

    public override ValidationResult Validate()
    {
        var result = new ChangelogRunCommandSettingsValidator().Validate(this);
        return result.IsValid
            ? ValidationResult.Success()
            : ValidationResult.Error(string.Join("\n", result.Errors));
    }
}