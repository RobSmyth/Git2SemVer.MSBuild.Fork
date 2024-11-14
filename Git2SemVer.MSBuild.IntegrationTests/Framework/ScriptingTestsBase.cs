using System.Diagnostics;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools;
using NoeticTools.Common.Tools.DotnetCli;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Testing.Common;


namespace NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;

public abstract class ScriptingTestsBase
{
    protected string TestFolderPath = "";

    protected void SetUpBase()
    {
        Logger = new NUnitLogger { Level = LoggingLevel.Trace };
        TestFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                      "Git2SemVer",
                                      "TestData");
        if (Directory.Exists(TestFolderPath))
        {
            Directory.Delete(TestFolderPath, true);
            if (!WaitUntil(() => !Directory.Exists(TestFolderPath)))
            {
                Assert.Fail($"Unable to deleted folder '{TestFolderPath}'.");
            }
        }

        Directory.CreateDirectory(TestFolderPath);
    }

    protected DotNetTool DotNetCli { get; private set; } = null!;

    protected ILogger Logger { get; private set; } = null!;

    private static bool WaitUntil(Func<bool> predicate)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!predicate())
        {
            if (stopwatch.Elapsed > TimeSpan.FromSeconds(30))
            {
                return false;
            }
            Thread.Sleep(5);
        }

        return true;
    }

    protected virtual void OneTimeSetUpBase()
    {
        Logger = new NUnitLogger();
        DotNetCli = new DotNetTool(new ProcessCli(Logger));
        Git = new GitTool(Logger);
    }

    protected GitTool Git { get; private set; }
}