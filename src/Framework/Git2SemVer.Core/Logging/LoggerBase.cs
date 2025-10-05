using System.Text.RegularExpressions;


namespace NoeticTools.Git2SemVer.Core.Logging;

public abstract class LoggerBase
{
    private const string LogScopeIndent = "  ";

    public string Errors => string.Join(Environment.NewLine, ErrorMessages);

    public bool HasError { get; protected set; }

    public abstract LoggingLevel Level { get; set; }

    public string MessagePrefix { get; private set; } = "";

    public IDisposable EnterLogScope()
    {
        MessagePrefix += LogScopeIndent;
        return new UsingScope(LeaveLogScope);
    }

    public bool IsLogging(LoggingLevel level)
    {
        return Level >= level;
    }

    protected string IndentLines(string message)
    {
        return IndentLines(message, MessagePrefix, MessagePrefix);
    }

    protected string IndentLines(string message, string firstLinePrefix, string followingLinesPrefix)
    {
        var lines = Regex.Split(message, "\r\n|\r|\n");
        return firstLinePrefix + MessagePrefix + string.Join(Environment.NewLine + followingLinesPrefix + MessagePrefix, lines);
    }

    private void LeaveLogScope()
    {
        MessagePrefix = MessagePrefix.Substring(0, MessagePrefix.Length - LogScopeIndent.Length);
    }

    protected readonly List<string> ErrorMessages = [];
}