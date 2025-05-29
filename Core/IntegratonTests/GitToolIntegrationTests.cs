using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.FluentApi;
#pragma warning disable NUnit2045


namespace NoeticTools.Git2SemVer.Core.IntegrationTests;

[TestFixture, NonParallelizable]
public class GitToolIntegrationTests
{
    private GitTool _target;
    private ConsoleLogger _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = new ConsoleLogger();
        _target = new GitTool(_logger);
    }

    [TearDown]
    public void TearDown()
    {
        _target.Dispose();
        _logger.Dispose();
    }

    [Test]
    public void CanInvokeGitTest()
    {
        Assert.That(_target.BranchName, Is.Not.Empty);
    }

    [Test]
    public void MeasurePerformanceTest()
    {
        var stopwatch = Stopwatch.StartNew();

        var commits = _target.GetCommitsLibGit2Sharp(0);

        stopwatch.Stop();
        _logger.LogInfo($"Reading commits took {stopwatch.ElapsedMilliseconds/commits.Count}ms per commit.");
    }

    [Test]
    public void GetCommitsFromShaTest()
    {
        var commit = GetCommitAtIndex(_target, 5);

        var commits = _target.GetCommitsLibGit2Sharp(commit.CommitId.Sha);

        Assert.That(commits, Has.Count.LessThan(301));
        Assert.That(commits, Has.Count.GreaterThan(100));
        Assert.That(commits[0], Is.SameAs(commit));
    }

    private static Commit GetCommitAtIndex(GitTool target, int index)
    {
        var commit = target.Head!;
        for (var count = 0; count < index; count++)
        {
            commit = target.Get(commit.Parents[0]);
        }
        return commit;
    }
}