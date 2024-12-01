namespace NoeticTools.Git2SemVer.IntegrationTests.VersioningBuilds;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class CrossTargetingSolutionVersioningBuildTests : VersioningBuildTestsBase
{
    protected override VersioningBuildTestContext CreateTestContext()
    {
        return new VersioningBuildTestContext("CrossTarget", "CrossTargetingTestSolution", "CrossTargetingVersioning.sln", "TestApplication");
    }
}