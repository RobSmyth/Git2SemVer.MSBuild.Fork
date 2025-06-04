namespace NoeticTools.Git2SemVer.Core.Logging;

public abstract class LoggerBase
{
    private const string LogScopeIndent = "  ";

    protected readonly List<string> ErrorMessages = [];

    public string Errors => string.Join("\n", ErrorMessages);

    public bool HasError { get; protected set; }

    public abstract LoggingLevel Level { get; set; }

    public string LogPrefix { get; private set; } = "";

    public IDisposable EnterLogScope()
    {
        LogPrefix += LogScopeIndent;
        return new UsingScope(LeaveLogScope);
    }

    protected string IndentLines(string message)
    {
        message = message.Replace("\n", "\n" + LogPrefix);
        return LogPrefix + message;
    }

    private void LeaveLogScope()
    {
        LogPrefix = LogPrefix.Substring(0, LogPrefix.Length - LogScopeIndent.Length);
    }
}