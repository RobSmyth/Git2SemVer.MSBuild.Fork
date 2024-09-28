namespace NoeticTools.Common.Tools.Git;

public interface IGitTool
{
    string BranchName { get; }

    bool HasLocalChanges { get; }

    IReadOnlyList<Commit> GetCommits(int skipCount, int takeCount);

    string Run(string arguments);
}