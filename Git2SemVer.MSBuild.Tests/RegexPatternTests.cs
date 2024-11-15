using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.MSBuild.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.MSBuild.Tests;

[TestFixture]
internal class RegexPatternTests
{
    [Test]
    public void CanObtainGroupNamesTest()
    {
        const string input = "main";
        var regex = new Regex("^((?<release>main|release)|(?<beta>feat(ure)?)|(?<alpha>.*))[\\/_]?");

        var match = regex.Match(input);

        var groupNames = regex.GetGroupNames();
        foreach (var groupName in groupNames)
        {
            if (groupName[0].IsDigit())
            {
                continue;
            }

            TestContext.Out.WriteLine($"  {groupName} - {match.Groups[groupName].Success}");
        }
    }

    [TestCase("FORCE-VERSION: 1.2.3")]
    [TestCase("aaa FORCE-VERSION: 1.2.3 bbb")]
    [TestCase("aaa \nFORCE-VERSION: 1.2.3\n dd")]
    public void DemoScriptTest(string body)
    {
        var regex = new Regex(@"FORCE-VERSION: (?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)");

        var match = regex.Match(body);

        Assert.That(match.Groups["major"].Value, Is.EqualTo("1"));
        Assert.That(match.Groups["minor"].Value, Is.EqualTo("2"));
        Assert.That(match.Groups["patch"].Value, Is.EqualTo("3"));
    }
}