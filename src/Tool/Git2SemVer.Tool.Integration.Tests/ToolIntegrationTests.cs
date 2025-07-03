using NoeticTools.Git2SemVer.Tool.Integration.Tests.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Tool.Integration.Tests;

[TestFixture]
[NonParallelizable]
internal class ToolIntegrationTests : SolutionTestsBase
{
    private string _packageOutputDir;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OneTimeSetUpBase();

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

    /// <summary>
    ///     This test requires manual inspection of output to see that logging levels are correct.
    /// </summary>
    [TestCase("info")]
    [TestCase("debug")]
    [TestCase("trace")]
    public void RunCommandTest(string verbosity)
    {
        var result = ExecuteGit2SemVerTool($"run -u --verbosity {verbosity}");

        Console.WriteLine(result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.EqualTo(0));
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
        TestContext.Out.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.EqualTo(0));
        Assert.That(result.stdOutput, Does.Contain("-h, --help"));
        Assert.That(result.stdOutput, Does.Contain("-u, --unattended"));
    }

    [Test]
    public void ToolHelpRunCommand()
    {
        var result = ExecuteGit2SemVerTool("run --help");
        TestContext.Out.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
        Assert.That(result.stdOutput, Does.Contain("-h, --help"));
        Assert.That(result.stdOutput, Does.Contain("-u, --unattended"));
        Assert.That(result.stdOutput, Does.Contain("-v, --verbosity"));
        Assert.That(result.stdOutput, Does.Contain("--host-type"));
        Assert.That(result.stdOutput, Does.Contain("--output"));
        Assert.That(result.stdOutput, Does.Contain("--enable-json-write"));
    }

    [Test]
    public void ToolHelpCommand()
    {
        var result = ExecuteGit2SemVerTool("--help");
        TestContext.Out.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
        Assert.That(result.stdOutput, Does.Contain("-h, --help"));
        Assert.That(result.stdOutput, Does.Not.Contain("-u, --unattended"));
    }

    [Test]
    public void ToolInvalidArgumentHandling()
    {
        var result = ExecuteGit2SemVerTool("--unknown");

        TestContext.Out.WriteLine(Logger.Errors);
        Assert.That(result.returnCode, Is.Not.Zero);
    }

    [Test]
    public void ToolVersionCommand()
    {
        var result = ExecuteGit2SemVerTool("--version");
        Console.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
    }

    protected override string SolutionFolderName => "SolutionVersioning";

    protected override string SolutionName => "StandAloneVersioning.sln";
}