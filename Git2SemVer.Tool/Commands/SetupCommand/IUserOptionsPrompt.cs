namespace NoeticTools.Git2SemVer.Tool.Commands.SetupCommand;

internal interface IUserOptionsPrompt
{
    UserOptions GetOptions(FileInfo solution);
}