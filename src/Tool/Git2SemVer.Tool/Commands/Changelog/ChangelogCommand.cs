using System.Diagnostics;
using JetBrains.Annotations;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.ChangeLogging;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Tools.CI;
using NoeticTools.Git2SemVer.Tool.Commands.Run;
using NoeticTools.Git2SemVer.Tool.Framework;


namespace NoeticTools.Git2SemVer.Tool.Commands.Changelog;

[RegisterSingleton]
internal sealed class ChangelogCommand(IConsoleIO console)
    : CommandBase(console), IChangelogCommand
{
    private const string ConfigurationFilename = "changelog.conf.json";

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

            var result = versionGenerator.GenerateVersionOutputs();
            var config = GetConfiguration(settings);
            var releaseUrl = settings.ArtifactUrl;
            new MarkdownChangelog(logger, config).Generate(releaseUrl,
                                                           settings.WriteToConsole,
                                                   settings.OutputFilePath,
                                                   result.Outputs,
                                                   result.Contributing);

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

    private static IBuildHost GetBuildHost(CompositeLogger logger, GeneratorInputs inputs)
    {
        var config = Git2SemVerConfiguration.Load();

        var host = new BuildHostFactory(config, logger.LogInfo, logger).Create(inputs.HostType,
                                                                               inputs.BuildNumber,
                                                                               inputs.BuildContext,
                                                                               inputs.BuildIdFormat);
        return host;
    }

    private static ChangelogConfiguration GetConfiguration(ChangelogCommandSettings settings)
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
        if (File.Exists(configPath))
        {
            return ChangelogConfiguration.Load(configPath);
        }

        var config = new ChangelogConfiguration()
        {
            Categories = MarkdownChangelog.DefaultCategories
        };
        config.Save(configPath);
        return config;
    }
}