using System.ComponentModel;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using Spectre.Console;
using Spectre.Console.Cli;


// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;

internal class RunCommandSettings : CommonCommandSettings
{
    [CommandOption("-b|--branch-maturity-pattern <PATTERN>")]
    [DefaultValue(null)]
    [Description("Optional regular expression value to map branch name to release and prerelease labels.")]
    public string? BranchMaturityPattern { get; set; }

    [CommandOption("--conv-commits-json-write")]
    [DefaultValue(false)]
    [Description("Enables writing found conventional commits to file 'conventionalcommits.g.json'. Used for changelog generation.")]
    public bool EnableConvCommitsJsonWrite { get; set; }

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

    [CommandOption("-r|--release-tag-format <FORMAT>")]
    [DefaultValue(TagParsingConstants.DefaultVersionPrefix + TagParsingConstants.VersionPlaceholder)]
    [Description("Optional regular expression format to identify a release, and get the version, from a Git tag's friendly name. Must include `%VERSION%` placeholder text.")]
    public string? ReleaseTagFormat { get; set; } = null;

    [CommandOption("-v|--verbosity <LEVEL>")]
    [DefaultValue("info")]
    [Description("Sets output verbosity. Valid values are 'trace', 'debug', 'info', 'warning', or 'error'.")]
    public string Verbosity { get; set; } = "";

    public override ValidationResult Validate()
    {
        var result = new RunCommandSettingsValidator().Validate(this);
        return result.IsValid
            ? ValidationResult.Success()
            : ValidationResult.Error(string.Join("\n", result.Errors));
    }
}