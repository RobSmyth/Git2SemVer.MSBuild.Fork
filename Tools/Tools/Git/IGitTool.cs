namespace NoeticTools.Common.Tools.Git;

public interface IGitTool
{
    string BranchName { get; }

    bool HasLocalChanges { get; }

    IReadOnlyList<Commit> GetCommits(int skipCount, int takeCount);

    (int returnCode, string stdOutput) Run(string arguments);
}