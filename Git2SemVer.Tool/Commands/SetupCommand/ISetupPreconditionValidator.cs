namespace NoeticTools.Git2SemVer.Tool.Commands.SetupCommand;

internal interface ISetupPreconditionValidator
{
    bool Validate(DirectoryInfo directory, bool unattended);
}