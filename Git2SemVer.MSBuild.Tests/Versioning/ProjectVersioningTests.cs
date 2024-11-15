using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Versioning;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;
using NoeticTools.Testing.Common;
using Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Tests.Versioning
{
    internal class ProjectVersioningTests
    {
        private NUnitLogger _logger;
        private Mock<IVersionGeneratorInputs> _inputs;
        private Mock<IBuildHost> _host;
        private Mock<IGeneratedOutputsJsonFile> _generatedOutputsJsonFile;
        private Mock<IVersionGenerator> _versionGenerator;
        private ProjectVersioning _target;
        private Mock<IVersionOutputs> _localCachedOutputs;
        private Mock<IVersionOutputs> _sharedCachedOutputs;
        private Mock<IVersionOutputs> _generatedOutputs;

        [SetUp]
        public void SetUp()
        {
            _inputs = new Mock<IVersionGeneratorInputs>();
            _host = new Mock<IBuildHost>();
            _generatedOutputsJsonFile = new Mock<IGeneratedOutputsJsonFile>();
            _versionGenerator = new Mock<IVersionGenerator>();
            _logger = new NUnitLogger();

            _target = new ProjectVersioning(_inputs.Object, _host.Object, _generatedOutputsJsonFile.Object, _versionGenerator.Object, _logger);

            _inputs.Setup(x => x.SolutionSharedDirectory).Returns("SolutionSharedDirectory");
            _inputs.Setup(x => x.IntermediateOutputDirectory).Returns("IntermediateOutputDirectory");
            _inputs.Setup(x => x.BuildNumber).Returns("");

            _localCachedOutputs = new Mock<IVersionOutputs>();
            _sharedCachedOutputs = new Mock<IVersionOutputs>();

            _generatedOutputsJsonFile.Setup(x => x.Load("IntermediateOutputDirectory")).Returns(_localCachedOutputs.Object);
            _generatedOutputsJsonFile.Setup(x => x.Load("SolutionSharedDirectory")).Returns(_sharedCachedOutputs.Object);

            _generatedOutputs = new Mock<IVersionOutputs>();
            _versionGenerator.Setup(x => x.Run()).Returns(_generatedOutputs.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _logger.Dispose();
        }

        [Test]
        public void SolutionVersioningProject_DoesNotGenerate_WhenCachedOutputsAvailable()
        {
            _sharedCachedOutputs.Setup(x => x.BuildNumber).Returns("42");
            _inputs.Setup(x => x.VersioningMode).Returns(VersioningMode.SolutionVersioningProject);

            var result = _target.Run();

            _versionGenerator.Verify(x => x.Run(), Times.Never);
            _generatedOutputsJsonFile.Verify(x => x.Load("IntermediateOutputDirectory"), Times.Never);
            _host.Verify(x => x.SetBuildLabel(It.IsAny<string>()), Times.Never);
            Assert.That(result, Is.SameAs(_sharedCachedOutputs.Object));
        }

        [Test]
        public void SolutionVersioningProject_DoesNotGenerate_WhenCachedOutputsNotAvailable()
        {
            _sharedCachedOutputs.Setup(x => x.BuildNumber).Returns("");
            _inputs.Setup(x => x.VersioningMode).Returns(VersioningMode.SolutionVersioningProject);

            var result = _target.Run();

            _versionGenerator.Verify(x => x.Run(), Times.Once);
            _generatedOutputsJsonFile.Verify(x => x.Load("IntermediateOutputDirectory"), Times.Never);
            _host.Verify(x => x.SetBuildLabel(It.IsAny<string>()), Times.Never);
            Assert.That(result, Is.SameAs(_generatedOutputs.Object));
        }

        [Test]
        public void SolutionVersioningProject_UpdatesBuildLabel_WhenEnabledAndBuildNumberAvailable()
        {
            _sharedCachedOutputs.Setup(x => x.BuildNumber).Returns("42");
            var buildSystemVersion = SemVersion.ParsedFrom(1, 2, 3);
            _sharedCachedOutputs.Setup(x => x.BuildSystemVersion).Returns(buildSystemVersion);
            _inputs.Setup(x => x.VersioningMode).Returns(VersioningMode.SolutionVersioningProject);
            _inputs.Setup(x => x.UpdateHostBuildLabel).Returns(true);

            var result = _target.Run();

            _host.Verify(x => x.SetBuildLabel(buildSystemVersion.ToString()), Times.Once);
            _versionGenerator.Verify(x => x.Run(), Times.Never);
            _generatedOutputsJsonFile.Verify(x => x.Load("IntermediateOutputDirectory"), Times.Never);
            Assert.That(result, Is.SameAs(_sharedCachedOutputs.Object));
        }
    }
}
