namespace NoeticTools.Git2SemVer.Core.Tools.DotnetCli;

public sealed class SolutionCommands : ISolutionCommands
{
    private readonly DotNetTool _inner;

    internal SolutionCommands(DotNetTool inner)
    {
        _inner = inner;
    }

    public void New(string solutionName)
    {
        _inner.Run($"new sln --name \"{solutionName}\"");
    }

    public void New()
    {
        _inner.Run("new sln");
    }
}