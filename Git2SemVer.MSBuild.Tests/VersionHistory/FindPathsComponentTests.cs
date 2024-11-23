using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Git2SemVer.Testing.Core;


// ReSharper disable TailRecursiveCall

namespace NoeticTools.Git2SemVer.MSBuild.Tests.VersionHistory;

[TestFixture]
internal class FindPathsComponentTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void Test()
    {
        var logger = new NUnitLogger(false)
        {
            Level = LoggingLevel.Debug
        };

        var gitTool = new GitTool(logger);
        TestContext.Out.WriteLine();
        var commitsRepo = new CommitsRepository(gitTool);
        var paths = new PathsFromLastReleasesFinder(commitsRepo, gitTool, logger).FindPathsToHead();
        TestContext.Out.WriteLine(paths.GetReport());
    }
}