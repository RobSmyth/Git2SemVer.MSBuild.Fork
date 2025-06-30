using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Tool.Commands.Add;
using NoeticTools.Git2SemVer.Tool.Commands.Remove;
using Spectre.Console.Cli;


// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine;

internal class Git2SemVerCommandApp
{
    public static int Execute(string[] args)
    {
        using var logger = new FileLogger(GetLogFilePath());
        var servicesProvider = Services.ConfigureServices(logger);

        var app = new CommandApp();

        app.Configure(config =>
        {
            config.SetApplicationName("Git2SemVer");
            config.UseAssemblyInformationalVersion();

            config.AddCommand<AddCliCommand>("add")
                  .WithDescription("Add Git2SemVer solution versioning to solution in working directory.")
                  .WithData(servicesProvider);

            config.AddCommand<RemoveCliCommand>("remove")
                  .WithDescription("Remove Git2SemVer solution versioning from solution in working directory.")
                  .WithData(servicesProvider);

            config.AddCommand<RunCliCommand>("run")
                  .WithDescription("Run version generator.")
                  .WithData(servicesProvider);

            config.AddCommand<ChangelogCliCommand>("changelog")
                  .WithDescription("Generate changelog.")
                  .WithData(servicesProvider);
        });

        return app.Run(args);
    }

    private static string GetLogFilePath()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Git2SemVer");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return Path.Combine(folderPath, "Git2SemVer.Tool.log");
    }
}