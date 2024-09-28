using NoeticTools.Common;
using NoeticTools.Git2SemVer.IntegrationTests.Framework;
using NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;
#pragma warning disable NUnit2045


namespace NoeticTools.Git2SemVer.IntegrationTests;

[TestFixture]
internal class StandAloneVersioningBuildTests : SolutionTestsBase
{
    private string _outputExePath;
    private string _packageOutputDir;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OneTimeSetUpBase();

        BuildGit2SemVerMSBuild();
        BuildGit2SemVerTool();

        var testProjectBinDirectory = Path.Combine(TestSolutionDirectory, "TestApplication/bin/", BuildConfiguration);
        _outputExePath = Path.Combine(testProjectBinDirectory, "net8.0", "NoeticTools.TestApplication.exe");
        _packageOutputDir = testProjectBinDirectory;
    }

    [SetUp]
    public void SetUp()
    {
        SetUpBase();
        DeleteAllNuGetPackages(_packageOutputDir);
    }

    [Test]
    public void BuildAndPackWithForcingProperties2ScriptTest()
    {
        var scriptPath = DeployScript("ForceProperties2.csx");

        DotNetCli.Pack(TestSolutionPath, BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath}");

        var output = DotNetProcessHelpers.RunExe(_outputExePath, Logger);

        Assert.That(output, Contains.Substring("""
                                               Assembly version:       21.22.23.0
                                               File version:           21.22.23.0
                                               Informational version:  21.22.23-beta
                                               Product version:        21.22.23-beta
                                               """));
        AssertFileExists(_packageOutputDir, "NoeticTools.TestApplication.1.0.0.nupkg");
    }

    [Test]
    public void BuildAndThenPackWithoutRebuildTest()
    {
        DotNetCli.Build(TestSolutionPath, BuildConfiguration);
        DotNetCli.Pack(TestSolutionPath, BuildConfiguration, "--no-restore --no-build");

        var output = DotNetProcessHelpers.RunExe(_outputExePath, Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       0.1.0.0
                                               File version:           0.1.0
                                               """));
        AssertFileExists(_packageOutputDir, "NoeticTools.TestApplication.0.1.0.nupkg");
    }

    [Test]
    public void BuildOnlyTest()
    {
        var result = DotNetCli.Build(TestSolutionPath, BuildConfiguration);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.EqualTo(0), result.stdOutput);

        var output = DotNetProcessHelpers.RunExe(_outputExePath, Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       0.1.0.0
                                               File version:           0.1.0
                                               """));
    }

    [Test]
    public void BuildOnlyWithForcingProperties1ScriptTest()
    {
        var scriptPath = DeployScript("ForceProperties1.csx");

        DotNetCli.Build(TestSolutionPath, BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath}");

        var output = DotNetProcessHelpers.RunExe(_outputExePath, Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       1.2.3.0
                                               File version:           4.5.6
                                               Informational version:  11.12.13-a-prerelease+metadata
                                               Product version:        11.12.13-a-prerelease+metadata
                                               """));
    }

    [Test]
    public void CanBuildTestSolutionTest()
    {
        DotNetCli.Build(TestSolutionPath, BuildConfiguration);
    }

    [Test]
    public void PackWithForcingProperties1ScriptTest()
    {
        var scriptPath = DeployScript("ForceProperties1.csx");

        DotNetCli.Pack(TestSolutionPath, BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath} -fileLogger");

        var output = DotNetProcessHelpers.RunExe(_outputExePath, Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       1.2.3.0
                                               File version:           4.5.6
                                               Informational version:  11.12.13-a-prerelease+metadata
                                               Product version:        11.12.13-a-prerelease+metadata
                                               """));
        AssertFileExists(_packageOutputDir, $"NoeticTools.TestApplication.4.6.7.nupkg");
    }

    protected override string SolutionFolderName => "StandAloneVersioning";

    protected override string SolutionName => "Git2SemVerTestApplication.sln";

    private static void AssertFileExists(string packageDirectory, string expectedFilename)
    {
        var path = Path.Combine(packageDirectory, $"{expectedFilename}");
        Assert.That(File.Exists(path), Is.True, $"File '{path}' does not exist.");
    }
}