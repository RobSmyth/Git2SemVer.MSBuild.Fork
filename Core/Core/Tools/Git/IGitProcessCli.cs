namespace NoeticTools.Git2SemVer.Core.Tools.Git;

public interface IGitProcessCli
{
    string WorkingDirectory { get; set; }

    Task<int> RunAsync(string commandLineArguments,
                       TextWriter standardOut, TextWriter errorOut);
}