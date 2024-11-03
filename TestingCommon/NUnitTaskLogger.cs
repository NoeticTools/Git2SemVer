﻿using NoeticTools.Common.Logging;


namespace NoeticTools.Testing.Common;

public class NUnitTaskLogger : ILogger
{
    private const string LogScopeIndent = "  ";
    private readonly string _debugPrefix = "DEBUG: ";
    private readonly List<string> _errorMessages = [];
    private readonly string _errorPrefix = "ERROR: ";
    private readonly string _infoPrefix = "INFO: ";
    private readonly string _tracePrefix = "TRACE: ";
    private readonly string _warnPrefix = "WARN: ";

    public NUnitTaskLogger(bool showMessageLevelPrefix = true)
    {
        if (!showMessageLevelPrefix)
        {
            _errorPrefix = "";
            _warnPrefix = "";
            _infoPrefix = "";
            _debugPrefix = "";
            _tracePrefix = "";
        }
    }

    public string Errors => string.Join("\n", _errorMessages);

    public bool HasError { get; private set; }

    public LoggingLevel Level { get; set; } = LoggingLevel.Info;

    public string LogPrefix { get; private set; } = "";

    public void Dispose()
    {
    }

    public IDisposable EnterLogScope()
    {
        LogPrefix += LogScopeIndent;
        return new UsingScope(() => { LogPrefix = LogPrefix.Substring(0, LogPrefix.Length - LogScopeIndent.Length); });
    }

    public void Log(LoggingLevel level, string message)
    {
        if (Level <= level)
        {
            var lookup = new Dictionary<LoggingLevel, Action<string>>
            {
                { LoggingLevel.Trace, LogTrace },
                { LoggingLevel.Debug, LogDebug },
                { LoggingLevel.Info, LogInfo },
                { LoggingLevel.Warning, LogWarning },
                { LoggingLevel.Error, LogError }
            };

            lookup[level](message);
        }
    }

    public void LogDebug(string message)
    {
        if (Level >= LoggingLevel.Debug)
        {
            TestContext.Progress.WriteLine(_debugPrefix + LogPrefix + message);
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
        if (Level >= LoggingLevel.Error)
        {
            TestContext.Error.WriteLine(_errorPrefix + LogPrefix + message);
        }
    }

    public void LogError(string message, params object[] messageArgs)
    {
        LogError(string.Format(message, messageArgs));
    }

    public void LogError(Exception exception)
    {
        HasError = true;
        var message = $"Exception: {exception.Message}\nStack trace: {exception.StackTrace}";
        _errorMessages.Add(message);
        if (Level >= LoggingLevel.Error)
        {
            TestContext.Error.WriteLine(message);
        }
    }

    public void LogInfo(string message)
    {
        if (Level >= LoggingLevel.Info)
        {
            TestContext.Progress.WriteLine(_infoPrefix + LogPrefix + message);
        }
    }

    public void LogInfo(string message, params object[] messageArgs)
    {
        LogInfo(string.Format(message, messageArgs));
    }

    public void LogTrace(string message)
    {
        if (Level >= LoggingLevel.Trace)
        {
            TestContext.Progress.WriteLine(_tracePrefix + LogPrefix + message);
        }
    }

    public void LogTrace(string message, params object[] messageArgs)
    {
        LogTrace(string.Format(message, messageArgs));
    }

    public void LogWarning(string message)
    {
        if (Level >= LoggingLevel.Warning)
        {
            TestContext.Progress.WriteLine(_warnPrefix + LogPrefix + message);
        }
    }

    public void LogWarning(string format, params object[] args)
    {
        LogWarning(string.Format(format, args));
    }

    public void LogWarning(Exception exception)
    {
        LogWarning($"Exception - {exception.Message}");
    }
}