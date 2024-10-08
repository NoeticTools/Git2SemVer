using Injectio.Attributes;
using Spectre.Console;


namespace NoeticTools.Common.Logging;

[RegisterTransient]
public class ConsoleLogger : ILogger
{
    private const string LogScopeIndent = "  ";
    private readonly List<string> _errorMessages = [];
    private string _logPrefix = "";

    public string Errors => string.Join("\n", _errorMessages);

    public bool HasError { get; private set; }

    public LoggingLevel Level { get; set; } = LoggingLevel.Info;

    public IDisposable EnterLogScope()
    {
        _logPrefix += LogScopeIndent;
        return new UsingScope(LeaveLogScope);
    }

    public void LogDebug(string message)
    {
        if (Level >= LoggingLevel.Debug)
        {
            Console.Out.WriteLine(_logPrefix + message);
        }
    }

    public void LogDebug(string message, params object[] messageArgs)
    {
        LogDebug(string.Format(message, messageArgs));
    }

    public void LogError(string message)
    {
        HasError = true;
        _errorMessages.Add(message);
        AnsiConsole.MarkupLine("[red]" + message + "[/]");
    }

    public void LogError(string message, params object[] messageArgs)
    {
        LogError(_logPrefix + string.Format(message, messageArgs));
    }

    public void LogError(Exception exception)
    {
        HasError = true;
        var message = $"Exception - {exception.Message}\nStack trace: {exception.StackTrace}";
        LogError(message);
    }

    public void LogInfo(string message)
    {
        Console.Out.WriteLine(_logPrefix + message);
    }

    public void LogInfo(string message, params object[] messageArgs)
    {
        LogInfo(string.Format(message, messageArgs));
    }

    public void LogTrace(string message)
    {
        if (Level == LoggingLevel.Trace)
        {
            Console.Out.WriteLine(_logPrefix + message);
        }
    }

    public void LogTrace(string message, params object[] messageArgs)
    {
        if (Level == LoggingLevel.Trace)
        {
            LogTrace(string.Format(message, messageArgs));
        }
    }

    public void LogWarning(string message)
    {
        Console.Out.WriteLine(_logPrefix + message);
        AnsiConsole.MarkupLine("[fuchsia]" + message + "[/]");
    }

    public void LogWarning(string format, params object[] args)
    {
        LogWarning(string.Format(format, args));
    }

    public void LogWarning(Exception exception)
    {
        LogWarning($"Exception - {exception.Message}");
    }

    public void WriteTraceLine(string format, params object[] args)
    {
        WriteTraceLine(string.Format(format, args));
    }

    public void WriteTraceLine(string message)
    {
        if (Level >= LoggingLevel.Trace)
        {
            Console.Out.WriteLine(_logPrefix + message);
        }
    }

    private void LeaveLogScope()
    {
        _logPrefix = _logPrefix.Substring(0, _logPrefix.Length - LogScopeIndent.Length);
    }

    public void Dispose()
    {
    }
}