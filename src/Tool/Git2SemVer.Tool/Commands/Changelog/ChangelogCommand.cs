using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Framework.ChangeLogging;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Tool.Commands.Run;


namespace NoeticTools.Git2SemVer.Tool.Commands.Changelog;

[RegisterSingleton]
internal sealed class ChangelogCommand(IConsoleIO console) : CommandBase(console), IChangelogCommand
{
    private readonly IConsoleIO _console = console;

    private const string ConfigurationFilename = "changelog.conf.json";

    public void Execute(ChangelogCommandSettings settings)
    {
        try
        {
            Console.WriteMarkupInfoLine($"Generating Changelog {(settings.Unattended ? " (unattended)" : "")}.");
            Console.WriteLine("");

            var proceed = Console.PromptYesNo("Proceed?");
            Console.WriteLine();
            if (!proceed)
            {
                Console.WriteErrorLine("Aborted.");
            }

            var stopwatch = Stopwatch.StartNew();

            var inputs = new GeneratorInputs
            {
                VersioningMode = VersioningMode.StandAloneProject,
                IntermediateOutputDirectory = settings.DataDirectory,
                HostType = settings.HostType ?? ""
            };

            using var logger = CreateLogger(settings.Verbosity);
            var host = GetBuildHost(logger, inputs);
            var versionGenerator = new VersionGeneratorFactory(logger).Create(inputs,
                                                                              new NullMSBuildGlobalProperties(),
                                                                              new NullJsonFileIO(),
                                                                              host);

            var (outputs, contributing) = versionGenerator.CalculateSemanticVersion();

            EnsureDataDirectoryExists(settings);
            var config = GetConfiguration(settings);

            var outputFileExists = File.Exists(settings.OutputFilePath);
            if (!CanOverwrite(settings, outputFileExists, contributing, config))
            {
                Console.WriteLine();
                Console.WriteMarkupInfoLine("Nothing to do (did not overwrite).");
                Console.WriteMarkupLine("[bad]Aborted[/]");
                return;
            }

            var template = GetTemplate(settings);

            // todo - incremental updates

            var releaseUrl = settings.ArtifactUrl;
            var changelog = new ChangelogGenerator(config)
                .Generate(releaseUrl,
                          outputs,
                          contributing,
                          template,
                          incremental: true);

            if (settings.WriteToConsole)
            {
                Console.WriteLine("\nGenerated changelog:");
                Console.WriteHorizontalLine();
                Console.WriteCodeLine(changelog.TrimEnd());
                Console.WriteHorizontalLine();
            }

            if (settings.OutputFilePath.Length == 0)
            {
                Console.WriteLine();
                Console.WriteMarkupDebugLine("Write changelog to file is disabled as the file output path is an empty string.");
                return;
            }

            Console.WriteLine();
            var verb = settings.Incremental && outputFileExists ? "Updating" :
                !settings.Incremental && outputFileExists ? "Overwriting" : "Creating";
            Console.WriteMarkupInfoLine($"{verb} changelog file: {settings.OutputFilePath}");
            File.WriteAllText(settings.OutputFilePath, changelog);

            config.LastRun.CommitSha = contributing.Head.CommitId.Sha;
            config.LastRun.CommitWhen = contributing.Head.When;
            config.LastRun.SemVersion = outputs.Version!.ToString();
            config.Save(Path.Combine(settings.DataDirectory, ConfigurationFilename));

            stopwatch.Stop();

            Console.WriteLine("");
            Console.WriteMarkupLine($"[good]Completed[/] (in {stopwatch.ElapsedMilliseconds:D0} ms)");
        }
        catch (Exception exception)
        {
            Console.WriteErrorLine(exception);
            throw;
        }
    }

    private bool CanOverwrite(ChangelogCommandSettings settings, bool outputFileExists, ContributingCommits contributing, ChangelogSettings config)
    {
        if (settings.Incremental &&
            outputFileExists &&
            contributing.Head.CommitId.Equals(config.LastRun.CommitSha))
        {
            Console.WriteMarkupWarningLine($"""
                                            It is not possible to do an incremental update of an existing changelog file ([em]{settings.OutputFilePath}[/]) as the head commit has not changed.

                                            The file can be overwritten. If so, [warn]all manual changes will be lost.[/]
                                            """);
            var overwrite = Console.PromptYesNo("Overwrite existing changelog file?", false);
            if (!overwrite)
            {
                return false;
            }

            Console.WriteLine("");
        }

        return true;
    }

    private static void EnsureDataDirectoryExists(ChangelogCommandSettings settings)
    {
        var dataDirectory = settings.DataDirectory;
        // ReSharper disable once InvertIf
        if (dataDirectory.Length > 0)
        {
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
        }
    }

    private static ChangelogSettings GetConfiguration(ChangelogCommandSettings settings)
    {
        var configPath = Path.Combine(settings.DataDirectory, ConfigurationFilename);

        if (File.Exists(configPath))
        {
            return ChangelogSettings.Load(configPath);
        }

        var config = new ChangelogSettings
        {
            Categories = ChangelogResources.DefaultCategories
        };
        config.Save(configPath);
        return config;
    }

    private string GetTemplate(ChangelogCommandSettings settings)
    {
        var dataDirectory = settings.DataDirectory;

        var templatePath = Path.Combine(dataDirectory, ChangelogResources.DefaultMarkdownTemplateFilename);
        if (File.Exists(templatePath))
        {
            return File.ReadAllText(templatePath);
        }

        Console.WriteMarkupDebugLine($"Creating default template file: {templatePath}");
        var defaultTemplate = ChangelogResources.GetDefaultTemplate();
        File.WriteAllText(templatePath, defaultTemplate);
        return defaultTemplate;
    }
}