using Moq;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Tests.Versioning;

internal class SolutionVersionProjectVersioningTests : ProjectVersioningTestsBase
{
    [SetUp]
    public void SetUp()
    {
        ModeIs(VersioningMode.SolutionVersioningProject);
    }

    [Test]
    public void DoesGenerate_WhenCachedOutputsNotAvailable()
    {
        SharedCachedOutputs.Setup(x => x.BuildNumber).Returns("");

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Once);
        Host.Verify(x => x.SetBuildLabel(It.IsAny<string>()), Times.Never);
        Assert.That(result, Is.SameAs(GeneratedOutputs.Object));
        OutputsCacheJsonFile.Verify(x => x.Load("IntermediateOutputDirectory"), Times.Never);
    }

    [Test]
    public void DoesNotGenerate_WhenCachedOutputsAvailable()
    {
        SharedCachedOutputs.Setup(x => x.BuildNumber).Returns("42");

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Never);
        Host.Verify(x => x.SetBuildLabel(It.IsAny<string>()), Times.Never);
        Assert.That(result, Is.SameAs(SharedCachedOutputs.Object));
        OutputsCacheJsonFile.Verify(x => x.Load("IntermediateOutputDirectory"), Times.Never);
    }

    [Test]
    public void DoesNotUpdatesBuildLabel_WhenEnabledButBuildSystemVersion()
    {
        ModeIs(VersioningMode.SolutionVersioningProject);
        SharedCachedOutputs.Setup(x => x.BuildNumber).Returns("42");
        Inputs.Setup(x => x.UpdateHostBuildLabel).Returns(true);

        var result = Target.Run();

        Host.Verify(x => x.SetBuildLabel(It.IsAny<string>()), Times.Never);
        Assert.That(result, Is.SameAs(SharedCachedOutputs.Object));
    }

    [Test]
    public void SolutionVersioningProject_DoesNotUpdatesBuildLabel_WhenNotEnabledButHasBuildSystemVersion()
    {
        ModeIs(VersioningMode.SolutionVersioningProject);
        SharedCachedOutputs.Setup(x => x.BuildNumber).Returns("42");
        var buildSystemVersion = SemVersion.ParsedFrom(1, 2, 3);
        SharedCachedOutputs.Setup(x => x.BuildSystemVersion).Returns(buildSystemVersion);
        Inputs.Setup(x => x.UpdateHostBuildLabel).Returns(false);

        var result = Target.Run();

        Host.Verify(x => x.SetBuildLabel(It.IsAny<string>()), Times.Never);
        Assert.That(result, Is.SameAs(SharedCachedOutputs.Object));
    }

    [Test]
    public void UpdatesBuildLabel_WhenEnabledAndBuildNumberAvailable()
    {
        SharedCachedOutputs.Setup(x => x.BuildNumber).Returns("42");
        var buildSystemVersion = SemVersion.ParsedFrom(1, 2, 3);
        SharedCachedOutputs.Setup(x => x.BuildSystemVersion).Returns(buildSystemVersion);
        Inputs.Setup(x => x.UpdateHostBuildLabel).Returns(true);

        var result = Target.Run();

        Host.Verify(x => x.SetBuildLabel(buildSystemVersion.ToString()), Times.Once);
        VersionGenerator.Verify(x => x.Run(), Times.Never);
        Assert.That(result, Is.SameAs(SharedCachedOutputs.Object));
        OutputsCacheJsonFile.Verify(x => x.Load("IntermediateOutputDirectory"), Times.Never);
    }
}