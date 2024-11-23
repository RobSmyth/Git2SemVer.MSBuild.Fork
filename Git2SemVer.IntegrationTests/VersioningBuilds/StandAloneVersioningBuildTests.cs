using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;
using NoeticTools.Common;
using NoeticTools.Git2SemVer.IntegrationTests.Framework;
using NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;
using NoeticTools.Git2SemVer.Testing.Core;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.IntegrationTests.VersioningBuilds;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class StandAloneVersioningBuildTests
{
    private VersioningBuildTestContext CreateTestContext()
    {
        return new VersioningBuildTestContext("StandAlone", "StandAloneTestSolution", "StandAloneVersioning.sln", "TestApplication");
    }

    [Test]
    public void BuildAndPackWithForcingProperties2ScriptTest()
    {
        using var context = CreateTestContext();

        var scriptPath = context.DeployScript("ForceProperties2.csx");

        var result = context.DotNetCli.Pack(context.TestSolutionPath, context.BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath}");
        Assert.That(result.returnCode, Is.EqualTo(0), result.stdOutput);
        Assert.That(File.Exists(context.CompiledAppPath), Is.True, $"File '{context.CompiledAppPath}' does not exist after build and pack.");

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        Assert.That(output, Does.Contain("""
                                         Assembly version:       21.22.23.0
                                         File version:           21.22.23.0
                                         Informational version:  21.22.23-beta
                                         Product version:        21.22.23-beta
                                         """));
        VersioningBuildTestContext.AssertFileExists(context.PackageOutputDir, "NoeticTools.TestApplication.1.0.0.nupkg");

    }

    [Test]
    public void BuildOnlyWithForcingProperties1ScriptTest()
    {
        using var context = CreateTestContext();
        Console.WriteLine($"== {context.TestDirectory}");

        var scriptPath = context.DeployScript("ForceProperties1.csx");

        context.DotNetCli.Build(context.TestSolutionPath, context.BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath}");

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       1.2.3.0
                                               File version:           4.5.6
                                               Informational version:  11.12.13-a-prerelease+metadata
                                               Product version:        11.12.13-a-prerelease+metadata
                                               """));
    }

    // -----------------

    [Test]
    [CancelAfter(60000)]
    public void BuildAndThenPackWithoutRebuildTest()
    {
        using var context = CreateTestContext();

        var scriptPath = context.DeployScript("ForceProperties3.csx");
        context.DotNetCliBuildTestSolution($"-p:Git2SemVer_ScriptPath={scriptPath}");
        context.PackTestSolution();
        VersioningBuildTestContext.AssertFileExists(context.PackageOutputDir, "NoeticTools.TestApplication.1.2.3-alpha.nupkg");

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        Assert.That(output, Does.Contain("""
                                         Assembly version:       200.201.202.0
                                         File version:           200.201.212
                                         Informational version:  2.2.2-beta
                                         Product version:        2.2.2-beta
                                         """));
    }

    [Test]
    [CancelAfter(60000)]
    public void BuildOnlyTest()
    {
        using var context = CreateTestContext();

        var scriptPath = context.DeployScript("ForceProperties3.csx");
        context.DotNetCliBuildTestSolution($"-p:Git2SemVer_ScriptPath={scriptPath}");

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        Assert.That(output, Does.Contain("""
                                         Assembly version:       200.201.202.0
                                         File version:           200.201.212
                                         Informational version:  2.2.2-beta
                                         Product version:        2.2.2-beta
                                         """));
    }

    [Test]
    [CancelAfter(60000)]
    public void PackWithForcingProperties1ScriptTest()
    {
        using var context = CreateTestContext();

        var scriptPath = context.DeployScript("ForceProperties1.csx");

        var result = context.DotNetCli.Pack(context.TestSolutionPath, context.BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath} -fileLogger");
        Assert.That(result.returnCode, Is.EqualTo(0), result.stdOutput);

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       1.2.3.0
                                               File version:           4.5.6
                                               Informational version:  11.12.13-a-prerelease+metadata
                                               Product version:        11.12.13-a-prerelease+metadata
                                               """));
        VersioningBuildTestContext.AssertFileExists(context.PackageOutputDir, "NoeticTools.TestApplication.5.6.7.nupkg");
    }
}