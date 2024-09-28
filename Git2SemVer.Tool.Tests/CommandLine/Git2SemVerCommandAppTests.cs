using NoeticTools.Git2SemVer.Tool.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

