namespace NoeticTools.Git2SemVer.Tool.Commands.RemoveCommand;

internal interface IRemoveCommand
{
    bool HasError { get; }

    void Execute(string inputSolutionFile, bool unattended);
}