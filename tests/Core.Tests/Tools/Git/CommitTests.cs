using System.Text.RegularExpressions;
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
    public void CommitWithFeatureAddedTest()
    {
        var messageMetadata = new CommitMessageMetadata("feat", false, "", "", []);

        var target = new Commit("SHA00002", ["SHA00001"], "summary", "body", messageMetadata, _tagParser.Object, []);

        Assert.That(target.ReleaseState.State, Is.EqualTo(ReleaseStateId.NotReleased));
        Assert.That(target.ReleaseState.ReleasedVersion, Is.EqualTo(new SemVersion(0, 0, 0)));
        Assert.That(target.ReleaseState.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.ReleaseState.ChangeFlags.FunctionalityChange, Is.True);
        Assert.That(target.ReleaseState.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void CommitWithReleaseTagAndFeatureAddedTest()
    {
        var tag = new Mock<IGitTag>();
        tag.Setup(x => x.FriendlyName).Returns("my tag");
        _tagParser.Setup(x => x.ParseTagName("my tag")).Returns(new ReleaseState(ReleaseStateId.Released, new SemVersion(1, 2, 3)));
        var messageMetadata = new CommitMessageMetadata("feat", false, "", "", []);

        var target = new Commit("SHA00002", ["SHA00001"], "summary", "body", messageMetadata, _tagParser.Object, [tag.Object]);

        Assert.That(target.ReleaseState.State, Is.EqualTo(ReleaseStateId.Released));
        Assert.That(target.ReleaseState.ReleasedVersion, Is.EqualTo(new SemVersion(1, 2, 3)));
        Assert.That(target.ReleaseState.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.ReleaseState.ChangeFlags.FunctionalityChange, Is.False);
        Assert.That(target.ReleaseState.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void CommitTest()
    {
        var messageMetadata = new CommitMessageMetadata("", false, "", "", []);

        var target = new Commit("SHA00002", ["SHA00001"], "summary", "body", messageMetadata, _tagParser.Object, []);

        Assert.That(target.ReleaseState.State, Is.EqualTo(ReleaseStateId.NotReleased));
        Assert.That(target.ReleaseState.ReleasedVersion, Is.EqualTo(new SemVersion(0, 0, 0)));
        Assert.That(target.ReleaseState.ChangeFlags, Is.EqualTo(new ApiChangeFlags()));
    }

    [Test]
    public void RootCommitTest()
    {
        var messageMetadata = new CommitMessageMetadata("", false, "", "", []);

        var target = new Commit("SHA00001", [], "summary", "body", messageMetadata, _tagParser.Object, []);

        Assert.That(target.ReleaseState.State, Is.EqualTo(ReleaseStateId.RootCommit));
        Assert.That(target.ReleaseState.ReleasedVersion, Is.EqualTo(new SemVersion(0, 1, 0)));
        Assert.That(target.ReleaseState.ChangeFlags, Is.EqualTo(new ApiChangeFlags()));
    }

    [Test]
    public void RootCommitWithFeatureAddedTest()
    {
        var messageMetadata = new CommitMessageMetadata("feat", false, "", "", []);

        var target = new Commit("SHA00001", [], "summary", "body", messageMetadata, _tagParser.Object, []);

        Assert.That(target.ReleaseState.State, Is.EqualTo(ReleaseStateId.RootCommit));
        Assert.That(target.ReleaseState.ReleasedVersion, Is.EqualTo(new SemVersion(0, 1, 0)));
        Assert.That(target.ReleaseState.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.ReleaseState.ChangeFlags.FunctionalityChange, Is.True);
        Assert.That(target.ReleaseState.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void RootCommitWithReleaseTagAndFeatureAddedTest()
    {
        var tag = new Mock<IGitTag>();
        tag.Setup(x => x.FriendlyName).Returns("my tag");
        _tagParser.Setup(x => x.ParseTagName("my tag")).Returns(new ReleaseState(ReleaseStateId.Released, new SemVersion(1, 2, 3)));
        var messageMetadata = new CommitMessageMetadata("feat", false, "", "", []);

        var target = new Commit("SHA00001", [], "summary", "body", messageMetadata, _tagParser.Object, [tag.Object]);

        Assert.That(target.ReleaseState.State, Is.EqualTo(ReleaseStateId.Released));
        Assert.That(target.ReleaseState.ReleasedVersion, Is.EqualTo(new SemVersion(1, 2, 3)));
        Assert.That(target.ReleaseState.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.ReleaseState.ChangeFlags.FunctionalityChange, Is.False);
        Assert.That(target.ReleaseState.ChangeFlags.Fix, Is.False);
    }
}