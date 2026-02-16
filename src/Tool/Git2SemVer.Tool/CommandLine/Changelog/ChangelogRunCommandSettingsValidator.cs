using FluentValidation;
using NoeticTools.Git2SemVer.Core.Logging;


namespace NoeticTools.Git2SemVer.Tool.CommandLine.Changelog;

internal sealed class ChangelogRunCommandSettingsValidator : AbstractValidator<ChangelogCommandSettings>
{
    public ChangelogRunCommandSettingsValidator()
    {
        RuleFor(settings => settings.Verbosity).Must(BeAValidVerbosity)
                                               .WithMessage("The verbosity must be 'Trace', 'Debug', 'Info', 'Warning', or 'Error'.");

        RuleFor(settings => settings.ConvCommitsInfoFilePath).Must(BeAValidConvCommitsInfoPath)
                                                             .WithMessage("The Conventional Commits info file does not exist.");
    }

    private static bool BeAValidConvCommitsInfoPath(string path)
    {
        if (path.Length == 0)
        {
            return true;
        }

        return File.Exists(path);
    }

    private static bool BeAValidVerbosity(string verbosity)
    {
        return Enum.TryParse(verbosity, true, out LoggingLevel level);
    }
}