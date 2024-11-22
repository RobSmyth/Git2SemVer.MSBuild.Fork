using NoeticTools.Git2SemVer.IntegrationTests.Framework;
using NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.IntegrationTests;

[NonParallelizable]
internal abstract class VersioningBuildTestsBase : SolutionTestsBase
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OneTimeSetUpBase();

        //BuildGit2SemVerMSBuild();
        //BuildGit2SemVerTool();

        var testProjectBinDirectory = Path.Combine(TestSolutionDirectory, "TestApplication/bin/", BuildConfiguration);
        CompiledAppPath = Path.Combine(testProjectBinDirectory, "net8.0", "NoeticTools.TestApplication.dll");
        PackageOutputDir = testProjectBinDirectory;
    }

    [SetUp]
    public void SetUp()
    {
        SetUpBase();
        if (Directory.Exists(PackageOutputDir))
        {
            Directory.Delete(PackageOutputDir, true);
        }
    }

    protected void PackTestSolution(string solutionPath)
    {
        var result = DotNetCli.Pack(solutionPath, BuildConfiguration, "--no-restore --no-build");
        TestContext.Out.WriteLine(result.stdOutput);
        Assert.That(result.returnCode, Is.EqualTo(0));
        Assert.That(Logger.HasError, Is.False);
    }

    protected string CompiledAppPath;
    protected string PackageOutputDir;

    protected void DotNetCliBuildTestSolution(string solutionPath, params string[] arguments)
    {
        var result = DotNetCli.Build(solutionPath, BuildConfiguration, arguments);
        TestContext.Out.WriteLine(result.stdOutput);
        Assert.That(result.returnCode, Is.EqualTo(0), result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
    }

    protected static void AssertFileExists(string packageDirectory, string expectedFilename)
    {
        var directory = new DirectoryInfo(packageDirectory);
        var foundFiles = directory.GetFiles(expectedFilename);
        Assert.That(foundFiles.Length, Is.EqualTo(1), $"File '{expectedFilename}' does not exist.");
    }
}