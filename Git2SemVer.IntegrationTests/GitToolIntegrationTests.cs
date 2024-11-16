using NoeticTools.Common.Tools.Git;
using NoeticTools.Testing.Common;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.IntegrationTests;

[TestFixture]
internal class GitToolIntegrationTests
{
    [Test]
    public void RunGetVersionTest()
    {
        var logger = new NUnitLogger();
        var tool = new GitTool(logger);

        var result = tool.Run("--version");

        logger.LogInfo(result.stdOutput);
        Assert.That(result.returnCode, Is.EqualTo(0));
        Assert.That(logger.HasError, Is.False);
    }
}