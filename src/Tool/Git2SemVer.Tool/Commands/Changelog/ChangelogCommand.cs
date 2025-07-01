using System.Diagnostics;
using NoeticTools.Git2SemVer.Framework.ChangeLogging;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Tool.Commands.Run;
using NoeticTools.Git2SemVer.Tool.Framework;


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
            Console.WriteInfoLine($"Generating Changelog {(settings.Unattended ? " (unattended)" : "")}.");
            Console.WriteLine();

            if (!settings.Unattended)
            {
                Console.WriteLine("""

                                  Ready to generate Changelog from current working directory's Git repository.


                                  """);
                var proceed = Console.PromptYesNo("Proceed?");
                Console.WriteLine();
                if (!proceed)
                {
                    Console.WriteErrorLine("Aborted.");
                }
            }

            var stopwatch = Stopwatch.StartNew();

            var inputs = new GeneratorInputs
            {
                VersioningMode = VersioningMode.StandAloneProject,
                IntermediateOutputDirectory = settings.DataDirectory
            };

            using var logger = CreateLogger(settings.Verbosity);

            var host = GetBuildHost(logger, inputs);

            var outputsJsonIO = new NullJsonFileIO(); // todo - why is this here?
            var versionGenerator = new VersionGeneratorFactory(logger).Create(inputs,
                                                                              new NullMSBuildGlobalProperties(),
                                                                              outputsJsonIO,
                                                                              host);

            var (outputs, contributing) = versionGenerator.CalculateSemanticVersion();
            var configPath = GetConfigFilePath(settings);
            var config = GetConfiguration(configPath);

            // todo - incremental updates
            var template = GetTemplate(settings);

            var releaseUrl = settings.ArtifactUrl;
            new MarkdownGenerator(logger, config).Generate(releaseUrl,
                                                           settings.WriteToConsole,
                                                           settings.OutputFilePath,
                                                           outputs,
                                                           contributing,
                                                           template);

            config.LastRun.CommitSha = contributing.Head.CommitId.Sha;
            config.LastRun.CommitWhen = contributing.Head.When;
            config.LastRun.SemVersion = outputs.Version!.ToString();
            config.Save(configPath);

            stopwatch.Stop();

            Console.WriteInfoLine("");
            Console.WriteInfoLine($"Completed (in {stopwatch.ElapsedMilliseconds:D0} ms)");
        }
        catch (Exception exception)
        {
            Console.WriteErrorLine(exception);
            throw;
        }
    }

    private string GetTemplate(ChangelogCommandSettings settings)
    {
        var dataDirectory = settings.DataDirectory;

        var templatePath = Path.Combine(dataDirectory, MarkdownTemplateFilename);
        if (File.Exists(templatePath))
        {
            return File.ReadAllText(templatePath);
        }

        Console.WriteDebugLine($"Creating default template file: {templatePath}");
        var defaultTemplate = MarkdownGenerator.GetDefaultTemplate();
        File.WriteAllText(templatePath, defaultTemplate);
        return defaultTemplate;

    }

    private static string GetConfigFilePath(ChangelogCommandSettings settings)
    {
        var dataDirectory = settings.DataDirectory;
        if (dataDirectory.Length > 0)
        {
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
        }

        var configPath = Path.Combine(dataDirectory, ConfigurationFilename);
        return configPath;
    }

    private static ChangelogSettings GetConfiguration(string configPath)
    {
        if (File.Exists(configPath))
        {
            return ChangelogSettings.Load(configPath);
        }

        var config = new ChangelogSettings
        {
            Categories = MarkdownGenerator.DefaultCategories
        };
        config.Save(configPath);
        return config;
    }
}