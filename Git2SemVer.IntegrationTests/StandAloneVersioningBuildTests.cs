using NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.IntegrationTests;

[TestFixture]
[NonParallelizable]
internal class StandAloneVersioningBuildTests : VersioningBuildTestsBase
{
    [Test]
    public void BuildAndPackWithForcingProperties2ScriptTest()
    {
        var scriptPath = DeployScript("ForceProperties2.csx");

        var result = DotNetCli.Pack(TestSolutionPath, BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath}");
        Assert.That(result.returnCode, Is.EqualTo(0));
        Assert.That(File.Exists(CompiledAppPath), Is.True, $"File '{CompiledAppPath}' does not exist after build and pack.");

        var output = DotNetProcessHelpers.RunDotnetApp(CompiledAppPath, Logger);
        Assert.That(output, Does.Contain("""
                                         Assembly version:       21.22.23.0
                                         File version:           21.22.23.0
                                         Informational version:  21.22.23-beta
                                         Product version:        21.22.23-beta
                                         """));
        AssertFileExists(PackageOutputDir, "NoeticTools.TestApplication.1.0.0.nupkg");
    }

    [Test]
    public void BuildOnlyWithForcingProperties1ScriptTest()
    {
        var scriptPath = DeployScript("ForceProperties1.csx");

        DotNetCli.Build(TestSolutionPath, BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath}");

        var output = DotNetProcessHelpers.RunDotnetApp(CompiledAppPath, Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       1.2.3.0
                                               File version:           4.5.6
                                               Informational version:  11.12.13-a-prerelease+metadata
                                               Product version:        11.12.13-a-prerelease+metadata
                                               """));
    }

    protected override string SolutionFolderName => "StandAloneVersioning";

    protected override string SolutionName => "StandAloneVersioning.sln";
}