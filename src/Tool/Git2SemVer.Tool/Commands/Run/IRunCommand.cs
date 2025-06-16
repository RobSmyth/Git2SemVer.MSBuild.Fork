using NoeticTools.Git2SemVer.Tool.CommandLine;


namespace NoeticTools.Git2SemVer.Tool.Commands.Run;

internal interface IRunCommand
{
    bool HasError { get; }

    void Execute(RunCommandSettings unattended);
}