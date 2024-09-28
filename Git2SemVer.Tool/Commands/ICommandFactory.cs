using NoeticTools.Git2SemVer.Tool.Commands.RemoveCommand;
using NoeticTools.Git2SemVer.Tool.SetupCommand;


namespace NoeticTools.Git2SemVer.Tool.Commands;

internal interface ICommandFactory
{
    ISetupCommand CreateAddCommand();

    IRemoveCommand CreateRemoveCommand();
}