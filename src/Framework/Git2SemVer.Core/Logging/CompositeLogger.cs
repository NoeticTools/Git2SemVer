using NoeticTools.Git2SemVer.Core.Diagnostics;


namespace NoeticTools.Git2SemVer.Core.Logging;

public sealed class CompositeLogger : ILogger
{
    private readonly List<string> _errorMessages = [];
    private readonly List<ILogger> _loggers;
    private LoggingLevel _level;

    public CompositeLogger(params ILogger[] loggers)
    {
        _loggers = [..loggers];
        Level = LoggingLevel.Info;
    }

    public string Errors => string.Join(Environment.NewLine, _errorMessages);

    public bool HasError => _errorMessages.Any();

    public LoggingLevel Level
    {
        get => _level;
        set
        {
            _level = value;
            _loggers.ForEach(logger => logger.Level = value);
        }
    }

    public string MessagePrefix => _loggers[0].MessagePrefix;

    public void Add(ILogger logger)
    {
        _loggers.Add(logger);
    }

    public void Dispose()
    {
        _loggers.ForEach(logger => logger.Dispose());
        _loggers.Clear();
    }

    public IDisposable EnterLogScope()
    {
        var scopes = new List<IDisposable>();
        _loggers.ForEach(logger => scopes.Add(logger.EnterLogScope()));
        return new UsingScope(() => LeaveLogScope(scopes));
    }

    public bool IsLogging(LoggingLevel level)
    {
        return _loggers.Any(x => x.IsLogging(level));
    }

    public void Log(LoggingLevel level, string message)
    {
        if (Level < level)
        {
            return;
        }

        _loggers.ForEach(logger => logger.Log(level, message));
    }

    public void LogDebug(string message)
    {
        if (Level < LoggingLevel.Debug)
        {
            return;
        }

        _loggers.ForEach(logger => logger.LogDebug(message));
    }

    public void LogDebug(Func<string> messageGenerator)
    {
        if (Level < LoggingLevel.Debug)
        {
            return;
        }

        _loggers.ForEach(logger => logger.LogDebug(messageGenerator()));
    }

    public void LogDebug(string message, params object[] messageArgs)
    {
        if (Level < LoggingLevel.Debug)
        {
            return;
        }

        var formattedMessage = string.Format(message, messageArgs);
        LogDebug(formattedMessage);
    }

    public void LogError(string message)
    {
        _errorMessages.Add(message);
        _loggers.ForEach(logger => logger.LogError(message));
    }

    public void LogError(string message, params object[] messageArgs)
    {
        var formattedMessage = string.Format(message, messageArgs);
        _errorMessages.Add(formattedMessage);
        _loggers.ForEach(logger => logger.LogError(formattedMessage));
    }

    public void LogError(Exception exception)
    {
        var message = $"Exception - {exception.Message}\nStack trace: {exception.StackTrace}";
        if (exception.InnerException != null)
        {
            message += $"\nInner exception: {exception.InnerException.Message}";
        }
        _errorMessages.Add(message);
        _loggers.ForEach(logger => logger.LogError(exception));
    }

    public void LogError(DiagnosticCodeBase code)
    {
        _errorMessages.Add(code.MessageWithCode);
        _loggers.ForEach(logger => logger.LogError(code));
    }

    public void LogInfo(string message)
    {
        if (Level < LoggingLevel.Info)
        {
            return;
        }

        _loggers.ForEach(logger => logger.LogInfo(message));
    }

    public void LogInfo(string message, params object[] messageArgs)
    {
        if (Level < LoggingLevel.Info)
        {
            return;
        }

        var formattedMessage = string.Format(message, messageArgs);
        _loggers.ForEach(logger => logger.LogInfo(formattedMessage));
    }

    public void LogTrace(string message)
    {
        if (Level < LoggingLevel.Trace)
        {
            return;
        }

        _loggers.ForEach(logger => logger.LogTrace(message));
    }

    public void LogTrace(Func<string> messageGenerator)
    {
        if (Level < LoggingLevel.Trace)
        {
            return;
        }

        _loggers.ForEach(logger => logger.LogTrace(messageGenerator()));
    }

    public void LogTrace(string message, params object[] messageArgs)
    {
        if (Level < LoggingLevel.Trace)
        {
            return;
        }

        var formattedMessage = string.Format(message, messageArgs);
        _loggers.ForEach(logger => logger.LogTrace(formattedMessage));
    }

    public void LogWarning(DiagnosticCodeBase code)
    {
        _loggers.ForEach(logger => logger.LogWarning(code));
    }

    public void LogWarning(string message)
    {
        if (Level < LoggingLevel.Warning)
        {
            return;
        }

        _loggers.ForEach(logger => logger.LogWarning(message));
    }

    public void LogWarning(string format, params object[] args)
    {
        if (Level < LoggingLevel.Warning)
        {
            return;
        }

        var formattedMessage = string.Format(format, args);
        _loggers.ForEach(logger => logger.LogWarning(formattedMessage));
    }

    public void LogWarning(Exception exception)
    {
        if (Level < LoggingLevel.Trace)
        {
            return;
        }

        _loggers.ForEach(logger => logger.LogWarning(exception));
    }

    private static void LeaveLogScope(List<IDisposable> scopes)
    {
        scopes.ForEach(leaveScope => leaveScope.Dispose());
    }
}