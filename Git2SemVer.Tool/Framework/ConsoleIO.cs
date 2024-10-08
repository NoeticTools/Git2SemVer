using System.Text.RegularExpressions;
using Injectio.Attributes;
using NoeticTools.Common.Exceptions;
using NoeticTools.Common.Logging;
using Spectre.Console;


namespace NoeticTools.Git2SemVer.Tool.Framework;

[RegisterSingleton]
public class ConsoleIO : IConsoleIO
{
    private const string RegexHighlightMarkupPattern = @"\[(error|warn|em)\]";

    private readonly ILogger _logger;

    public ConsoleIO(ILogger logger)
    {
        _logger = logger;
    }

    public bool HasError => _logger.HasError;

    public LoggingLevel Level { get; set; } = LoggingLevel.Info;

    public bool Unattended { get; set; }

    public T Ask<T>(string prompt, T defaultValue)
    {
        if (Unattended)
        {
            return defaultValue;
        }

        var result = AnsiConsole.Ask(prompt, defaultValue);
        WriteDebugLine($"User returned '{result}' to ask '{prompt}'.");
        return result;
    }

    public void MarkupErrorLine(string message)
    {
        MarkupLine(message);
        _logger.LogError(RemoveMarkup(message));
    }

    public void MarkupLine(string message)
    {
        var markupText = Regex.Replace(message, RegexHighlightMarkupPattern, NamedColoursEvaluator,
                                       RegexOptions.Compiled | RegexOptions.IgnoreCase);
        AnsiConsole.MarkupLine(markupText);
    }

    public void MarkupWarningLine(string message)
    {
        MarkupLine(message);
        _logger.LogWarning(RemoveMarkup(message));
    }

    public T Prompt<T>(TextPrompt<T> prompt, T defaultValue)
    {
        if (Unattended)
        {
            if (prompt.Validator != null)
            {
                var result = prompt.Validator(defaultValue);
                if (!result.Successful)
                {
                    throw new
                        Git2SemVerConfigurationException($"No valid option in unattended mode for '{prompt}' default '{defaultValue}': {result.Message}");
                }
            }

            _logger.LogDebug($"Unattended used default '{defaultValue}' for prompt '{prompt}'.");
            return defaultValue;
        }

        {
            var result = AnsiConsole.Prompt(prompt.DefaultValue(defaultValue));
            _logger.LogDebug($"User returned '{result}' to prompt '{prompt}'.");
            return result;
        }
    }

    public bool PromptYesNo(string prompt, bool defaultValue = true)
    {
        if (Unattended)
        {
            _logger.LogDebug($"Unattended used default '{defaultValue}' for yes/no prompt '{prompt}'.");
            return defaultValue;
        }

        var result = AnsiConsole.Prompt(new TextPrompt<bool>("Proceed?")
                                        .AddChoices([true, false])
                                        .DefaultValue(defaultValue)
                                        .WithConverter(choice => choice ? "y" : "n"));
        _logger.LogDebug($"User returned '{result}' to prompt yes/no '{prompt}'.");
        return result;
    }

    public void WriteDebugLine(string message)
    {
        if (Level >= LoggingLevel.Debug)
        {
            AnsiConsole.MarkupLine("[grey]" + message + "[/]");
        }

        if (_logger.Level >= LoggingLevel.Debug)
        {
            _logger.LogDebug(message);
        }
    }

    public void WriteErrorLine(string message)
    {
        MarkupLine("[error]" + message + "[/]");
        _logger.LogError(message);
    }

    public void WriteInfoLine(string message)
    {
        AnsiConsole.WriteLine(message);
        _logger.LogInfo(message);
    }

    public void WriteLine()
    {
        AnsiConsole.WriteLine();
    }

    public void WriteLine(string message)
    {
        AnsiConsole.WriteLine(message);
    }

    public void WriteWarningLine(string message)
    {
        MarkupLine("[warn]" + message + "[/]");
        _logger.LogWarning(message);
    }

    private static string NamedColoursEvaluator(Match match)
    {
        if ("[error]".Equals(match.Value, StringComparison.InvariantCultureIgnoreCase))
        {
            return "[red]";
        }

        if ("[warn]".Equals(match.Value, StringComparison.InvariantCultureIgnoreCase))
        {
            return "[fuchsia]";
        }

        if ("[em]".Equals(match.Value, StringComparison.InvariantCultureIgnoreCase))
        {
            return "[aqua]";
        }

        return match.Value;
    }

    private static string RemoveMarkup(string message)
    {
        var markupRemovedText = Regex.Replace(message, RegexHighlightMarkupPattern, "",
                                              RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return markupRemovedText;
    }
}