using LibGit2Sharp;
using NoeticTools.Git2SemVer.Core.Diagnostics;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tests.Tools.Git.Parsers;

[TestFixture]
internal class TagParserTests
{
    private const string VersionPlaceholder = "%VERSION%";

    [TestCase("^")]
    [TestCase("^X")]
    [TestCase("^ ")]
    [TestCase("tag: ")]
    [TestCase("TAG: ")]
    [TestCase(".gsm")]
    [TestCase(".GSM")]
    [TestCase(".gsm.something")]
    public void ConstructorThrowsExceptionWhenFormatPrefixInvalid(string tagFormatPrefix)
    {
        Assert.Throws(Is.TypeOf<Git2SemVerDiagnosticCodeException>()
                        .And.Property(nameof(Git2SemVerDiagnosticCodeException.DiagCode)).InstanceOf(typeof(GSV005)),
                      () =>
                      {
                          var tagParser = new TagParser(tagFormatPrefix + VersionPlaceholder);
                      });
    }

    [TestCase("x")]
    [TestCase(@"\d+.\d+.\d+")]
    [TestCase("release: ")]
    [TestCase("release tag: ")]
    [TestCase("VERSION")]
    public void ConstructorThrowsExceptionWhenFormatMissingPlaceholder(string tagFormat)
    {
        Assert.Throws(Is.TypeOf<Git2SemVerDiagnosticCodeException>()
                        .And.Property(nameof(Git2SemVerDiagnosticCodeException.DiagCode)).InstanceOf(typeof(GSV006)),
                      () =>
                      {
                          var tagParser = new TagParser(tagFormat);
                      });
    }

    [TestCase(@"\d+ abc %VERSION%")]
    [TestCase("release: %VERSION%")]
    [TestCase("release tag: %VERSION% abc")]
    [TestCase("%VERSION%")]
    [TestCase("v%VERSION%")]
    public void ConstructorDoesNotThrowExceptionWhenFormatValid(string tagFormat)
    {
        Assert.That(new TagParser(tagFormat), Is.Not.Null);
    }

    [TestCase("", "v12.34.56")]
    [TestCase(@"\d+ abc %VERSION%", "77 abc 12.34.56")]
    [TestCase("release: %VERSION%", "release: 12.34.56")]
    [TestCase("release tag: %VERSION% abc", "release tag: 12.34.56 abc")]
    [TestCase("%VERSION%", "12.34.56")]
    [TestCase("v%VERSION%", "v12.34.56")]
    public void ParseTagFriendlyNameWithValidReleaseTag(string tagFormat, string tagText)
    {
        var expected = new SemVersion(12, 34, 56);
        var tagParser = new TagParser(tagFormat);

        var foundReleaseSemVersion = tagParser.ParseTagFriendlyName(tagText);

        Assert.That(foundReleaseSemVersion, Is.Not.Null);
        Assert.That(foundReleaseSemVersion, Is.EqualTo(expected));
    }

    [TestCase(@"\d+ abc %VERSION%", "77 abc 12.34")]
    [TestCase("release: %VERSION%", "release: 123456")]
    [TestCase("release %VERSION%", "release: 12.34.56")]
    [TestCase("release tag: %VERSION% abc", "release tag: 12.34.56 ab")]
    [TestCase("%VERSION%", "v12.34.56")]
    [TestCase("v%VERSION%", "12.34.56")]
    public void ParseTagFriendlyNameWithInvalidReleaseTag(string tagFormat, string tagText)
    {
        var expected = new SemVersion(12, 34, 56);
        var tagParser = new TagParser(tagFormat);

        var foundReleaseSemVersion = tagParser.ParseTagFriendlyName(tagText);

        Assert.That(foundReleaseSemVersion, Is.Null);
    }
}
