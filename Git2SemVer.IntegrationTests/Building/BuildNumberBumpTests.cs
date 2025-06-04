using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.IntegrationTests.Framework;

namespace NoeticTools.Git2SemVer.IntegrationTests.Building;

[TestFixture]
[NonParallelizable]
public class UncontrolledHostBuildTests
{
    [Test]
    [Explicit("Run manually on an uncontrolled host (dev box)")]
    public void BuildNumberIncrementsOnRebuildTest()
    {
        using var context = CreateTestContext();

        var firstBuildNumber = BuildAndRun(context);
        var secondBuildNumber = BuildAndRun(context);

        Assert.That(secondBuildNumber-firstBuildNumber, Is.EqualTo(1));
    }

    private int BuildAndRun(VersioningBuildTestContext context)
    {
        context.DotNetCliBuildTestSolution("--no-incremental");
        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        return GetBuildNumber(output);
    }

    private int GetBuildNumber(string output)
    {
        var regex = new Regex(@"^Informational version:\s+.+-.*\.(?<build_number>\d+)\+", RegexOptions.Multiline);
        var match = regex.Match(output);
        Assert.That(match.Success, Is.True);
        return int.Parse(match.Groups["build_number"].Value);
    }

    private VersioningBuildTestContext CreateTestContext()
    {
        return new VersioningBuildTestContext("UncontrolledHost", "StandAloneTestSolution",
            "StandAloneVersioning.sln", "TestApplication");
    }
}