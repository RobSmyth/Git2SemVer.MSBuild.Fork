using Injectio.Attributes;
using NoeticTools.Common.Logging;


namespace NoeticTools.Common.Tools.Git;

[RegisterTransient]
public class GitProcessCli : IGitProcessCli
{
    private readonly IProcessCli _inner;
    private readonly ILogger _logger;

    public GitProcessCli(ILogger logger) : this(new ProcessCli(logger), logger)
    {
    }

    public GitProcessCli(IProcessCli inner, ILogger logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public string WorkingDirectory
    {
        get => _inner.WorkingDirectory;
        set => _inner.WorkingDirectory = value;
    }

    public int Run(string commandLineArguments,
                   TextWriter standardOut, TextWriter errorOut)
    {
        return _inner.Run("git", commandLineArguments, standardOut, errorOut);
    }
}