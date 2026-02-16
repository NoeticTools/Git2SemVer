using System.Text.RegularExpressions;
using FluentValidation;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;


namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;

internal sealed class RunCommandSettingsValidator : AbstractValidator<RunCommandSettings>
{
    public RunCommandSettingsValidator()
    {
        RuleFor(setting => setting.ReleaseTagFormat).NotEmpty()
                                                    .WithMessage("ReleaseTagFormat must be a non-empty string.");
        RuleFor(setting => setting.ReleaseTagFormat).Must(BeAValidReleaseTagFormat)
                                                    .WithMessage($"ReleaseTagFormat must be a regular expression and include a {TagParsingConstants.VersionPlaceholder} version placeholder.");
        RuleFor(settings => settings.Verbosity).Must(BeAValidVerbosity)
                                               .WithMessage("The verbosity must be 'Trace', 'Debug', 'Info', 'Warning', or 'Error'.");
    }

    private static bool BeAValidReleaseTagFormat(string? releaseTagFormat)
    {
        if (string.IsNullOrEmpty(releaseTagFormat))
        {
            return false;
        }

        try
        {
            var regex = new Regex(releaseTagFormat!);
        }
#pragma warning disable CA1031
        catch
#pragma warning restore CA1031
        {
            return false;
        }

        return releaseTagFormat.Contains(TagParsingConstants.VersionPlaceholder, StringComparison.Ordinal);
    }

    private static bool BeAValidVerbosity(string verbosity)
    {
        return Enum.TryParse(verbosity, true, out LoggingLevel level);
    }
}