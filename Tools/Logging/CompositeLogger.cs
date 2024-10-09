namespace NoeticTools.Common.Logging;

public sealed class CompositeLogger : ILogger
{
    private readonly List<ILogger> _loggers;
    private readonly List<string> _errorMessages = [];
    private LoggingLevel _level;

    public CompositeLogger(params ILogger[] loggers)
    {
        _loggers = [..loggers];
        Level = LoggingLevel.Info;
    }

    public string Errors => string.Join("\n", _errorMessages);

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

    public IDisposable EnterLogScope()
    {
        var scopes = new List<IDisposable>();
        _loggers.ForEach(logger => scopes.Add(logger.EnterLogScope()));
        return new UsingScope(() => LeaveLogScope(scopes));
    }

    public void Log(LoggingLevel level, string message)
    {
        if (Level >= level)
        {
            _loggers.ForEach(logger => logger.Log(level, message));
        }
    }

    private static void LeaveLogScope(List<IDisposable> scopes)
    {
        scopes.ForEach(leaveScope => leaveScope.Dispose());
    }

    public void LogDebug(string message)
    {
        if (Level >= LoggingLevel.Debug)
        {
            _loggers.ForEach(logger => logger.LogDebug(message));
        }
    }

    public void LogDebug(string message, params object[] messageArgs)
    {
        if (Level >= LoggingLevel.Debug)
        {
            var formattedMessage = string.Format(message, messageArgs);
            LogDebug(formattedMessage);
        }
    }

    public void LogError(string message)
    {
        _errorMessages.Add(message);
        if (Level >= LoggingLevel.Error)
        {
            _loggers.ForEach(logger => logger.LogError(message));
        }
    }

    public void LogError(string message, params object[] messageArgs)
    {
        var formattedMessage = string.Format(message, messageArgs);
        _errorMessages.Add(formattedMessage);
        if (Level >= LoggingLevel.Error)
        {
            _loggers.ForEach(logger => logger.LogError(formattedMessage));
        }
    }

    public void LogError(Exception exception)
    {
        var message = $"Exception - {exception.Message}\nStack trace: {exception.StackTrace}";
        _errorMessages.Add(message);
        if (Level >= LoggingLevel.Error)
        {
            _loggers.ForEach(logger => logger.LogError(exception));
        }
    }

    public void LogInfo(string message)
    {
        if (Level >= LoggingLevel.Error)
        {
            _loggers.ForEach(logger => logger.LogInfo(message));
        }
    }

    public void LogInfo(string message, params object[] messageArgs)
    {
        if (Level < LoggingLevel.Error)
        {
            return;
        }

        var formattedMessage = string.Format(message, messageArgs);
        _loggers.ForEach(logger => logger.LogInfo(formattedMessage));
    }

    public void LogTrace(string message)
    {
        if (Level >= LoggingLevel.Trace)
        {
            _loggers.ForEach(logger => logger.LogTrace(message));
        }
    }

    public void LogTrace(string message, params object[] messageArgs)
    {
        if (Level < LoggingLevel.Error)
        {
            return;
        }

        var formattedMessage = string.Format(message, messageArgs);
        _loggers.ForEach(logger => logger.LogTrace(formattedMessage));
    }

    public void LogWarning(string message)
    {
        if (Level >= LoggingLevel.Warning)
        {
            _loggers.ForEach(logger => logger.LogWarning(message));
        }
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
        if (Level >= LoggingLevel.Warning)
        {
            _loggers.ForEach(logger => logger.LogWarning(exception));
        }
    }

    public void Dispose()
    {
        _loggers.ForEach(logger => logger.Dispose());
        _loggers.Clear();
    }

    public void Add(ILogger logger)
    {
        _loggers.Add(logger);
    }
}