using Moq;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Versioning;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;
using NoeticTools.Testing.Common;


namespace NoeticTools.Git2SemVer.MSBuild.Tests.Versioning;

internal abstract class ProjectVersioningTestsBase
{
    private NUnitLogger _logger;

    [SetUp]
    public void SetUpBase()
    {
        Inputs = new Mock<IVersionGeneratorInputs>();
        Host = new Mock<IBuildHost>();
        OutputsCacheJsonFile = new Mock<IGeneratedOutputsJsonFile>();
        VersionGenerator = new Mock<IVersionGenerator>();
        _logger = new NUnitLogger();

        Target = new ProjectVersioning(Inputs.Object, Host.Object, OutputsCacheJsonFile.Object, VersionGenerator.Object, _logger);

        Inputs.Setup(x => x.SolutionSharedDirectory).Returns("SolutionSharedDirectory");
        Inputs.Setup(x => x.IntermediateOutputDirectory).Returns("IntermediateOutputDirectory");
        Inputs.Setup(x => x.BuildNumber).Returns("");

        LocalCachedOutputs = new Mock<IVersionOutputs>();
        SharedCachedOutputs = new Mock<IVersionOutputs>();

        OutputsCacheJsonFile.Setup(x => x.Load("IntermediateOutputDirectory")).Returns(LocalCachedOutputs.Object);
        OutputsCacheJsonFile.Setup(x => x.Load("SolutionSharedDirectory")).Returns(SharedCachedOutputs.Object);

        GeneratedOutputs = new Mock<IVersionOutputs>();
        VersionGenerator.Setup(x => x.Run()).Returns(GeneratedOutputs.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }

    protected Mock<IVersionGeneratorInputs> Inputs;
    protected Mock<IBuildHost> Host;
    protected Mock<IGeneratedOutputsJsonFile> OutputsCacheJsonFile;
    protected Mock<IVersionGenerator> VersionGenerator;
    protected ProjectVersioning Target;
    protected Mock<IVersionOutputs> LocalCachedOutputs;
    protected Mock<IVersionOutputs> SharedCachedOutputs;
    protected Mock<IVersionOutputs> GeneratedOutputs;

    protected void ModeIs(VersioningMode mode)
    {
        Inputs.Setup(x => x.VersioningMode).Returns(mode);
    }
}