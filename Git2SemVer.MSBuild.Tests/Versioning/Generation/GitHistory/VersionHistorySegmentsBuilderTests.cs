using Moq;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistory;
using NoeticTools.Testing.Common;
//#pragma warning disable NUnit2045


namespace NoeticTools.Git2SemVer.MSBuild.Tests.Versioning.Generation.GitHistory;

[TestFixture]
internal class VersionHistorySegmentsBuilderTests
{
    private VersionHistorySegmentsBuilder _target;
    private Mock<ICommitsRepository> _repository;
    private NUnitTaskLogger _logger;

    [SetUp]
    public void Setup()
    {
        VersionHistorySegment.Reset();
        GitObfuscation.Reset();

        _logger = new NUnitTaskLogger(false) {Level = LoggingLevel.Trace};
        _repository = new Mock<ICommitsRepository>();

        _target = new VersionHistorySegmentsBuilder(_repository.Object, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }

    [Test]
    public void Scenario01()
    {
        const string gitLog = """
                              .|0001|0002 0003|REDACTED| (HEAD -> REDACTED_BRANCH, origin/REDACTED_BRANCH)|
                              .|0002|0004|REDACTED||
                              .|0004|0005 0006|REDACTED||
                              .|0005|0007|REDACTED||
                              .|0003|0008|REDACTED||
                              .|0006|0007 0008|REDACTED||
                              .|0007|0009|REDACTED||
                              .|0008|0010|REDACTED||
                              .|0009|0011|REDACTED||
                              .|0011|0010|REDACTED||
                              .|0010|0012|REDACTED||
                              .|0012|0013|REDACTED||
                              .|0013|0014|REDACTED| (tag: v0.3.1)|
                              .|0014|0015|REDACTED||
                              .|0015|0016 0017|REDACTED||
                              .|0017|0018|REDACTED||
                              .|0018|0019|REDACTED||
                              .|0019|0020|REDACTED||
                              .|0020|0021|REDACTED||
                              .|0021|0016|REDACTED||
                              .|0016|0022|REDACTED||
                              .|0022|0023 0024|REDACTED||
                              .|0024|0025|REDACTED||
                              .|0025|0026|REDACTED||
                              .|0026|0027|REDACTED||
                              .|0027|0028|REDACTED||
                              .|0028|0029|REDACTED||
                              .|0029|0030|REDACTED||
                              .|0030|0031|REDACTED||
                              .|0031|0032|REDACTED||
                              .|0032|0033|REDACTED||
                              .|0033|0034|REDACTED||
                              .|0034|0035|REDACTED||
                              .|0035|0036|REDACTED||
                              .|0036|0037|REDACTED||
                              .|0037|0038|REDACTED||
                              .|0038|0039|REDACTED||
                              .|0039|0040|REDACTED||
                              .|0040|0041|REDACTED||
                              .|0041|0042|REDACTED||
                              .|0023|0042|REDACTED||
                              .|0042|0043|REDACTED||
                              .|0043|0044|REDACTED||
                              .|0044|0045|REDACTED||
                              .|0045|0046|REDACTED| (tag: v0.3.0)|
                              .|0046|0047|REDACTED||
                              """;
        var commits = SetupGitRepository(gitLog);

        var segments = _target.BuildTo(commits["0001"]);

        Assert.That(segments, Is.Not.Null);
        Assert.That(segments, Has.Count.EqualTo(8));
        Assert.Multiple(() =>
        {
            var segment = segments[0];
            Assert.That(segment.Id, Is.EqualTo(1));
            Assert.That(segment.Commits, Has.Count.EqualTo(1));
            Assert.That(segment.FirstCommit.CommitId.ObfuscatedSha, Is.EqualTo("0001"));
            Assert.That(segment.LastCommit.CommitId.ObfuscatedSha, Is.EqualTo("0001"));
            Assert.That(segment.To, Has.Count.EqualTo(0));
            Assert.That(segment.From, Has.Count.EqualTo(2));
            Assert.That(segment.TaggedReleasedVersion, Is.Null);
        });
        Assert.That(segments[1].Commits, Has.Count.EqualTo(2));
        Assert.That(segments[2].Commits, Has.Count.EqualTo(1));
        Assert.That(segments[3].Commits, Has.Count.EqualTo(1));
        Assert.That(segments[4].Commits, Has.Count.EqualTo(3));
        Assert.That(segments[5].Commits, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            var segment = segments[6];
            Assert.That(segment.Commits, Has.Count.EqualTo(3));
            Assert.That(segment.To, Has.Count.EqualTo(2));
            Assert.That(segment.To[0].Id, Is.EqualTo(5));
            Assert.That(segment.To[1].Id, Is.EqualTo(6));
            Assert.That(segment.TaggedReleasedVersion!.ToString(), Is.EqualTo("0.3.1"));
        });
        Assert.That(segments[7].Commits, Has.Count.EqualTo(1));
    }

    private Dictionary<string, Commit> SetupGitRepository(string gitLog)
    {
        var commits = GetCommits(gitLog).ToDictionary(k => k.CommitId.Id, v => v);
        _repository.Setup(x => x.Get(It.IsAny<CommitId>())).Returns<CommitId>(id => commits[id.Id]);
        return commits;
    }

    private List<Commit> GetCommits(string gitLog)
    {
        var commits = new List<Commit>();
        foreach (var logLine in gitLog.Split('\n'))
        {
            var commit = GitTool.ParseLogLine(logLine.Trim(), _logger);
            commits.Add(commit);
        }
        return commits;
    }
}

