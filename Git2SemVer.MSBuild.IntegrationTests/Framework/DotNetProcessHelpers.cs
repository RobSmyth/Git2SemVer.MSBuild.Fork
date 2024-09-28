using System.Text;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools;
using NoeticTools.Common.Tools.DotnetCli;


namespace NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;

public static class DotNetProcessHelpers
{
    public static string GetSolutionDirectory()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "../../../..")).FullName;
        return solutionDirectory;
    }

    public static void Publish(string projectPath, string buildConfiguration)
    {
        TestContext.Progress.WriteLine($"\nPublishing {projectPath}\n");
        const string profileFile = "FolderProfile.pubxml";
        var dotNetCommandLine = $"publish {projectPath} /p:PublishProfile={profileFile} --configuration {buildConfiguration} --no-build --no-restore";
        var result = new DotNetTool().Run(dotNetCommandLine, TestContext.Progress, TestContext.Error);
        Assert.That(result, Is.EqualTo(0), $"Command failed: dotnet {dotNetCommandLine}");
    }

    public static string RunExe(string exePath, ILogger logger)
    {
        TestContext.Progress.WriteLine();
        var process = new ProcessCli(logger)
        {
            WorkingDirectory = Path.GetDirectoryName(exePath)!
        };
        var outputStringBuilder = new StringBuilder();
        var outputWriter = new StringWriter(outputStringBuilder);
        var returnCode = process.Run(exePath, "", outputWriter, TestContext.Error);
        Assert.That(returnCode, Is.EqualTo(0));
        var output = outputStringBuilder.ToString();
        TestContext.Progress.WriteLine();
        return output;
    }
}