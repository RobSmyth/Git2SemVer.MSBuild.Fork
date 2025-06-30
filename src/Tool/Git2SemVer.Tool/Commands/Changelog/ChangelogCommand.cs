using System.Diagnostics;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Framework;
using NoeticTools.Git2SemVer.Framework.ChangeLogging;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Tools.CI;
using NoeticTools.Git2SemVer.Tool.CommandLine;
using NoeticTools.Git2SemVer.Tool.Commands.Run;
using NoeticTools.Git2SemVer.Tool.Framework;
using NoeticTools.Git2SemVer.Tool.MSBuild;
using System.Text;


namespace NoeticTools.Git2SemVer.Tool.Commands.Changelog;

[RegisterSingleton]
internal sealed class ChangelogCommand(IConsoleIO console)
    : CommandBase(console), IChangelogCommand
{
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
                IntermediateOutputDirectory = settings.OutputDirectory
            };

#pragma warning disable CA2000
            using var logger = CreateLogger(settings.Verbosity);
#pragma warning restore CA2000

            var config = Git2SemVerConfiguration.Load();

            var host = new BuildHostFactory(config, msg => logger.LogInfo(msg), logger).Create(inputs.HostType,
                                                                                               inputs.BuildNumber,
                                                                                               inputs.BuildContext,
                                                                                               inputs.BuildIdFormat);

            var outputsJsonIO = new NullJsonFileIO(); // todo - why is this here?
            var versionGenerator = new VersionGeneratorFactory(logger).Create(inputs, 
                                                                              new NullMSBuildGlobalProperties(),
                                                                              outputsJsonIO,
                                                                              host);

            var result = versionGenerator.GenerateVersionOutputs();
            new MarkdownChangelog(logger).Generate(settings.WriteToConsole,
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
}