using Moq;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tests.Tools.Git;

[TestFixture]
internal class CommitTests
{
    private Mock<ITagParser> _tagParser;

    [SetUp]
    public void SetUp()
    {
        _tagParser = new Mock<ITagParser>();
    }

    [Test]
    public void CommitTest()
    {
        var messageMetadata = new CommitMessageMetadata("", false, "", "", []);

        var target = new Commit("SHA00002", ["SHA00001"], "summary", "body", messageMetadata, _tagParser.Object, []);

        Assert.That(target.Metadata.ReleaseType, Is.EqualTo(ReleaseTypeId.NotReleased));
        Assert.That(target.Metadata.Version, Is.Null);
        Assert.That(target.Metadata.ChangeFlags, Is.EqualTo(new ApiChangeFlags()));
    }

    [Test]
    public void CommitWithFeatureAddedTest()
    {
        var messageMetadata = new CommitMessageMetadata("feat", false, "", "", []);

        var target = new Commit("SHA00002", ["SHA00001"], "summary", "body", messageMetadata, _tagParser.Object, []);

        Assert.That(target.Metadata.ReleaseType, Is.EqualTo(ReleaseTypeId.NotReleased));
        Assert.That(target.Metadata.Version, Is.Null);
        Assert.That(target.Metadata.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.Metadata.ChangeFlags.FunctionalityChange, Is.True);
        Assert.That(target.Metadata.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void CommitWithReleaseTagAndFeatureAddedTest()
    {
        var tag = new Mock<IGitTag>();
        tag.Setup(x => x.FriendlyName).Returns("my tag");
        _tagParser.Setup(x => x.ParseTagName("my tag")).Returns(new CommitMetadata(ReleaseTypeId.Released, new SemVersion(1, 2, 3)));
        var messageMetadata = new CommitMessageMetadata("feat", false, "", "", []);

        var target = new Commit("SHA00002", ["SHA00001"], "summary", "body", messageMetadata, _tagParser.Object, [tag.Object]);

        Assert.That(target.Metadata.ReleaseType, Is.EqualTo(ReleaseTypeId.Released));
        Assert.That(target.Metadata.Version, Is.EqualTo(new SemVersion(1, 2, 3)));
        Assert.That(target.Metadata.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.Metadata.ChangeFlags.FunctionalityChange, Is.False);
        Assert.That(target.Metadata.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void RootCommitTest()
    {
        var messageMetadata = new CommitMessageMetadata("", false, "", "", []);

        var target = new Commit("SHA00001", [], "summary", "body", messageMetadata, _tagParser.Object, []);

        Assert.That(target.Metadata.ReleaseType, Is.EqualTo(ReleaseTypeId.RootCommit));
        Assert.That(target.Metadata.Version, Is.Null);
        Assert.That(target.Metadata.ChangeFlags, Is.EqualTo(new ApiChangeFlags()));
    }

    [Test]
    public void RootCommitWithFeatureAddedTest()
    {
        var messageMetadata = new CommitMessageMetadata("feat", false, "", "", []);

        var target = new Commit("SHA00001", [], "summary", "body", messageMetadata, _tagParser.Object, []);

        Assert.That(target.Metadata.ReleaseType, Is.EqualTo(ReleaseTypeId.RootCommit));
        Assert.That(target.Metadata.Version, Is.Null);
        Assert.That(target.Metadata.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.Metadata.ChangeFlags.FunctionalityChange, Is.True);
        Assert.That(target.Metadata.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void RootCommitWithReleaseTagAndFeatureAddedTest()
    {
        var tag = new Mock<IGitTag>();
        tag.Setup(x => x.FriendlyName).Returns("my tag");
        _tagParser.Setup(x => x.ParseTagName("my tag")).Returns(new CommitMetadata(ReleaseTypeId.Released, new SemVersion(1, 2, 3)));
        var messageMetadata = new CommitMessageMetadata("feat", false, "", "", []);

        var target = new Commit("SHA00001", [], "summary", "body", messageMetadata, _tagParser.Object, [tag.Object]);

        Assert.That(target.Metadata.ReleaseType, Is.EqualTo(ReleaseTypeId.Released));
        Assert.That(target.Metadata.Version, Is.EqualTo(new SemVersion(1, 2, 3)));
        Assert.That(target.Metadata.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.Metadata.ChangeFlags.FunctionalityChange, Is.False);
        Assert.That(target.Metadata.ChangeFlags.Fix, Is.False);
    }
}