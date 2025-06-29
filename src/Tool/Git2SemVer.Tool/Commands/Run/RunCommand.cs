using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Tool.CommandLine;
using NoeticTools.Git2SemVer.Tool.Framework;


namespace NoeticTools.Git2SemVer.Tool.Commands.Run;

[RegisterSingleton]
internal sealed class RunCommand(IConsoleIO console) : IRunCommand
{
    public bool HasError => console.HasError;

    public void Execute(RunCommandSettings settings)
    {
        try
        {
            console.WriteInfoLine($"Running Git2SemVer version generator{(settings.Unattended ? " (unattended)" : "")}.");
            console.WriteLine();

            var inputs = new GeneratorInputs
            {
                VersioningMode = VersioningMode.StandAloneProject,
                IntermediateOutputDirectory = settings.OutputDirectory
            };

            if (settings.HostType != null)
            {
                inputs.HostType = settings.HostType;
            }

#pragma warning disable CA2000
            using var logger = new CompositeLogger();
            //logger.Add(new NoDisposeLoggerDecorator(_logger));
            logger.Add(new ConsoleLogger());
#pragma warning restore CA2000
            logger.Level = GetVerbosity(settings.Verbosity);

            IOutputsJsonIO outputJsonIO = settings.EnableJsonFileWrite ? new OutputsJsonFileIO() : new ReadOnlyOutputJsonIO();
            var versionGeneratorFactory = new VersionGeneratorFactory(logger);
            var projectVersioning = new ProjectVersioningFactory(msg => logger.LogInfo(msg),versionGeneratorFactory, logger)
                .Create(inputs, new NullMSBuildGlobalProperties(), outputJsonIO);
            projectVersioning.Run();

            console.WriteInfoLine("");
            console.WriteInfoLine("Completed");
        }
        catch (Exception exception)
        {
            console.WriteErrorLine(exception);
            throw;
        }
    }

    private LoggingLevel GetVerbosity(string verbosity)
    {
        if (Enum.TryParse(verbosity, true, out LoggingLevel level))
        {
            return level;
        }

        console.WriteErrorLine($"Verbosity {verbosity} is not valid. Must be 'Trace', 'Debug', 'Info', 'Warning', or 'Error'.");
        return LoggingLevel.Info;
    }
}