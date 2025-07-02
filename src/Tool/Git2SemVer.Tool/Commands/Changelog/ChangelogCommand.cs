using NoeticTools.Git2SemVer.Framework.ChangeLogging;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Tool.Commands.Run;
using NoeticTools.Git2SemVer.Tool.Framework;
using Spectre.Console;
using System.Diagnostics;


namespace NoeticTools.Git2SemVer.Tool.Commands.Changelog;

[RegisterSingleton]
internal sealed class ChangelogCommand(IConsoleIO console)
    : CommandBase(console), IChangelogCommand
{
    private const string ConfigurationFilename = "changelog.conf.json";
    private const string MarkdownTemplateFilename = "MarkdownChangelog.scriban";

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
                    Console.WriteLine();
                    Console.WriteMarkupInfoLine("Nothing to do (did not overwrite).");
                    Console.WriteMarkupLine("[bad]Aborted[/]");
                    return;
                }
                Console.WriteLine("");
            }

            var template = GetTemplate(settings);

            // todo - incremental updates

            var releaseUrl = settings.ArtifactUrl;
            var changelog = new ChangelogGenerator(config)
                .Generate(releaseUrl,
                          outputs,
                          contributing,
                          template);

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
            else
            {
                Console.WriteLine();
                var verb = (settings.Incremental && outputFileExists) ? "Updating" :
                    (!settings.Incremental && outputFileExists) ? "Overwriting" : "Creating";
                Console.WriteMarkupInfoLine($"{verb} changelog file: {settings.OutputFilePath}");
                File.WriteAllText(settings.OutputFilePath, changelog);
            }

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
            Categories = ChangelogGenerator.DefaultCategories
        };
        config.Save(configPath);
        return config;
    }

    private string GetTemplate(ChangelogCommandSettings settings)
    {
        var dataDirectory = settings.DataDirectory;

        var templatePath = Path.Combine(dataDirectory, MarkdownTemplateFilename);
        if (File.Exists(templatePath))
        {
            return File.ReadAllText(templatePath);
        }

        Console.WriteMarkupDebugLine($"Creating default template file: {templatePath}");
        var defaultTemplate = ChangelogGenerator.GetDefaultTemplate();
        File.WriteAllText(templatePath, defaultTemplate);
        return defaultTemplate;
    }
}