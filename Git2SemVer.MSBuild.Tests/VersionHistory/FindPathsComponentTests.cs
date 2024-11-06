using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Testing.Common;


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
        var logger = new NUnitTaskLogger(false)
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