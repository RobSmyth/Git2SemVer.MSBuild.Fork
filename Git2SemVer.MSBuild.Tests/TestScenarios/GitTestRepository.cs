using NoeticTools.Common.Tools.Git;


namespace NoeticTools.Git2SemVer.MSBuild.Tests.TestScenarios;

public sealed class GitTestRepository
{
    public string Description { get; }

    public Commit[] Commits { get; }

    public string HeadBranchName { get; }

    public string HeadCommitId { get; }

    public int ExpectedPathCount { get; }

    public string ExpectedVersion { get; }

    public GitTestRepository(string description,
                             Commit[] commits,
                             string headBranchName,
                             string headCommitId,
                             int expectedPathCount,
                             string expectedVersion)
    {
        Description = description;
        Commits = commits;
        HeadBranchName = headBranchName;
        HeadCommitId = headCommitId;
        ExpectedPathCount = expectedPathCount;
        ExpectedVersion = expectedVersion;
    }
}