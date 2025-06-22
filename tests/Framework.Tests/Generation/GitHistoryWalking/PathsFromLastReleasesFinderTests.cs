using NoeticTools.Git2SemVer.Framework.Generation;


//#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Framework.Tests.Generation.GitHistoryWalking;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class PathsFromLastReleasesFinderTests
{
    [TestCaseSource(typeof(ScenariosFromBuildLogsTestSource))]
    public void BasicScenariosTest(string name, LoggedScenario scenario)
    {
        using var context = new GitHistoryWalkingTestsContext();
        context.SetupGitRepository(scenario);
        var target = new PathsFromLastReleasesFinder(context.GitTool.Object, context.Logger);

        var result = target.CalculateSemanticVersion();

        Assert.That(result.Version.ToString(), Is.EqualTo(scenario.ExpectedVersion));
    }
}