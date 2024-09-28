namespace NoeticTools.Git2SemVer.Tool.SetupCommand;

internal interface ISetupCommand
{
    bool HasError { get; }

    void Execute(string inputSolutionFile, bool unattended);
}