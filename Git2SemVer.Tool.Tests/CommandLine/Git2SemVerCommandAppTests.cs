using NoeticTools.Git2SemVer.Tool.CommandLine;


namespace NoeticTools.Git2SemVer.Tool.Tests.CommandLine;

[TestFixture]
internal class Git2SemVerCommandAppTests
{
    [Test]
    public void Test()
    {
        var target = new Git2SemVerCommandApp();

        //target.Execute(["-h"]);

        //target.Execute(["add", "-s", "xxxx", "-x"]);

        target.Execute(["--version"]);
    }
}