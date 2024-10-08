namespace NoeticTools.Common.Logging;

public abstract class TaskLoggerBase : ILogger
{
    private const string LogScopeIndent = "  ";
    private readonly ITaskLoggerAdapter _adapter;
    private readonly List<string> _errorMessages = [];
    private string _logPrefix = "";

    protected TaskLoggerBase(ITaskLoggerAdapter adapter)
    {
        _adapter = adapter;
    }

    public string Errors => string.Join("\n", _errorMessages);

    public bool HasError { get; private set; }

    /// <summary>
    /// This logger ignores the set log level as the underlying task logger sets the logging level.
    /// </summary>
    public LoggingLevel Level
    {
        get => LoggingLevel.Trace;
        set {}
    }

    public IDisposable EnterLogScope()
    {
        _logPrefix += LogScopeIndent;
        return new UsingScope(LeaveLogScope);
    }

    public void Log(LoggingLevel level, string message)
    {
        throw new NotImplementedException();
    }

    public void LogDebug(string message)
    {
        _adapter.LogDebug(_logPrefix + message);
    }

    public void LogDebug(string message, params object[] messageArgs)
    {
        LogDebug(string.Format(message, messageArgs));
    }

    public void LogError(string message)
    {
        HasError = true;
        _errorMessages.Add(message);
        _adapter.LogError(message);
    }

    public void LogError(string message, params object[] messageArgs)
    {
        LogError(string.Format(message, messageArgs));
    }

    public void LogError(Exception exception)
    {
        HasError = true;
        _errorMessages.Add($"Exception: {exception.Message}\nStack trace: {exception.StackTrace}");
        _adapter.LogError(exception);
    }

    public void LogInfo(string message)
    {
        _adapter.LogInfo(_logPrefix + message);
    }

    public void LogInfo(string message, params object[] messageArgs)
    {
        LogDebug(string.Format(message, messageArgs));
    }

    public void LogTrace(string message)
    {
        _adapter.LogTrace(_logPrefix + message);
    }

    public void LogTrace(string message, params object[] messageArgs)
    {
        LogTrace(string.Format(message, messageArgs));
    }

    public void LogWarning(string message)
    {
        _adapter.LogWarning(_logPrefix + message);
    }

    public void LogWarning(string format, params object[] args)
    {
        LogWarning(string.Format(format, args));
    }

    public void LogWarning(Exception exception)
    {
        _adapter.LogWarning(exception);
    }

    private void LeaveLogScope()
    {
        _logPrefix = _logPrefix.Substring(0, _logPrefix.Length - LogScopeIndent.Length);
    }

    public void Dispose()
    {
    }
}