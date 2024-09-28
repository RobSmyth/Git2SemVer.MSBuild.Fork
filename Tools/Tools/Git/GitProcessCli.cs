using Injectio.Attributes;
using NoeticTools.Common.Logging;


namespace NoeticTools.Common.Tools.Git;

[RegisterTransient]
public class GitProcessCli : IGitProcessCli
{
    private readonly IProcessCli _inner;

    public GitProcessCli(ILogger logger) : this(new ProcessCli(logger), logger)
    {
    }

    public GitProcessCli(IProcessCli inner, ILogger logger)
    {
        _inner = inner;
    }

    public string WorkingDirectory
    {
        get => _inner.WorkingDirectory;
        set => _inner.WorkingDirectory = value;
    }

    public int Run(string commandLineArguments,
                   TextWriter standardOut, TextWriter? errorOut = null)
    {
        return _inner.Run("git", commandLineArguments, standardOut, errorOut);
    }
}