using Injectio.Attributes;
using Spectre.Console;


namespace NoeticTools.Common.Logging;

[RegisterTransient]
public class FileLogger : ILogger
{
    private const string LogScopeIndent = "  ";
    private readonly List<string> _errorMessages = [];
    private string _logPrefix = "";
    private readonly StreamWriter _stream;

    public FileLogger(string filePath)
    {
        _stream = new StreamWriter(filePath, false);
    }

    public string Errors => string.Join("\n", _errorMessages);

    public bool HasError { get; private set; }

    public LoggingLevel Level { get; set; } = LoggingLevel.Trace;

    public IDisposable EnterLogScope()
    {
        _logPrefix += LogScopeIndent;
        return new UsingScope(LeaveLogScope);
    }

    public void Log(LoggingLevel level, string message)
    {
        if (Level < level)
        {
            return;
        }

        var levelPrefix = new Dictionary<LoggingLevel, string>
        {
            { LoggingLevel.Trace, "TRACE|" },
            { LoggingLevel.Debug, "DEBUG|" },
            { LoggingLevel.Info, "INFO|" },
            { LoggingLevel.Warning, "WARN|" },
            { LoggingLevel.Error, "ERROR|" },
        };

        _stream.WriteLine(levelPrefix[level] + _logPrefix + message);
    }

    public void LogDebug(string message)
    {
        Log(LoggingLevel.Debug, message);
    }

    public void LogDebug(string message, params object[] messageArgs)
    {
        Log(LoggingLevel.Debug, string.Format(message, messageArgs));
    }

    public void LogError(string message)
    {
        HasError = true;
        _errorMessages.Add(message);
        Log(LoggingLevel.Error, message);
    }

    public void LogError(string message, params object[] messageArgs)
    {
        LogError(string.Format(message, messageArgs));
    }

    public void LogError(Exception exception)
    {
        HasError = true;
        var message = $"Exception - {exception.Message}\nStack trace: {exception.StackTrace}";
        LogError(message);
    }

    public void LogInfo(string message)
    {
        Log(LoggingLevel.Info, message);
    }

    public void LogInfo(string message, params object[] messageArgs)
    {
        LogInfo(string.Format(message, messageArgs));
    }

    public void LogTrace(string message)
    {
        Log(LoggingLevel.Trace, message);
    }

    public void LogTrace(string message, params object[] messageArgs)
    {
        if (Level >= LoggingLevel.Trace)
        {
            LogTrace(string.Format(message, messageArgs));
        }
    }

    public void LogWarning(string message)
    {
        Log(LoggingLevel.Warning, message);
    }

    public void LogWarning(string format, params object[] args)
    {
        LogWarning(string.Format(format, args));
    }

    public void LogWarning(Exception exception)
    {
        LogWarning($"Exception - {exception.Message}");
    }

    private void LeaveLogScope()
    {
        _logPrefix = _logPrefix.Substring(0, _logPrefix.Length - LogScopeIndent.Length);
    }

    public void Dispose()
    {
        _stream.Flush();
        _stream.Close();
    }
}