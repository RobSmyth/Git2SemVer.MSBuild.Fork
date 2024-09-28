using NoeticTools.Git2SemVer.IntegrationTests.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.IntegrationTests;

[TestFixture]
internal class SolutionVersioningBuildTests : SolutionTestsBase
{
    private string _packageOutputDir;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OneTimeSetUpBase();

        BuildGit2SemVerMSBuild();
        BuildGit2SemVerTool();

        var testProjectBinDirectory = Path.Combine(TestSolutionDirectory, "TestApplication/bin/", BuildConfiguration);
        _packageOutputDir = testProjectBinDirectory;
    }

    [SetUp]
    public void SetUp()
    {
        SetUpBase();
        DeleteAllNuGetPackages(_packageOutputDir);
    }

    [TearDown]
    public void TearDown()
    {
        ExecuteGit2SemVerTool("remove -u");
    }

    [Test]
    public void AddCommandTest()
    {
        var result = ExecuteGit2SemVerTool("add -u");

        Console.WriteLine(result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.EqualTo(0));
    }

    [Test]
    public void RemoveCommandTest()
    {
        var result = ExecuteGit2SemVerTool("remove -u");

        Console.WriteLine(result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.EqualTo(0));
    }

    [Test]
    public void ToolHelpAddCommand()
    {
        var result = ExecuteGit2SemVerTool("add --help");

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.EqualTo(0));
        Assert.That(result.stdOutput, Contains.Substring("-h, --help"));
        Assert.That(result.stdOutput, Contains.Substring("USAGE:\r\n    Git2SemVer.Tool add [OPTIONS]"));
    }

    [Test]
    public void ToolHelpCommand()
    {
        var result = ExecuteGit2SemVerTool("--help");
        Console.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.EqualTo(0));
        Assert.That(result.stdOutput, Contains.Substring("-h, --help"));
        Assert.That(result.stdOutput, Contains.Substring("USAGE:\r\n    Git2SemVer.Tool [OPTIONS] <COMMAND>"));
    }

    [Test]
    public void ToolInvalidArgumentHandling()
    {
        var result = ExecuteGit2SemVerTool("--unknown");

        TestContext.Out.WriteLine(Logger.Errors);
        Assert.That(result.returnCode, Is.Not.EqualTo(0));
    }

    [Test]
    public void ToolVersionCommand()
    {
        var result = ExecuteGit2SemVerTool("--version");
        Console.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.EqualTo(0));
    }

    protected override string SolutionFolderName => "SolutionVersioning";

    protected override string SolutionName => "Git2SemVerTestSolutionVersioning.sln";
}