using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Tool.CommandLine;
using NoeticTools.Git2SemVer.Tool.Framework;


namespace NoeticTools.Git2SemVer.Tool.Commands.Run;

[RegisterSingleton]
internal sealed class RunCommand : IRunCommand
{
    private readonly IConsoleIO _console;

    public RunCommand(IConsoleIO console)
    {
        _console = console;
    }

    public bool HasError => _console.HasError;

    public void Execute(RunCommandSettings settings)
    {
        try
        {
            _console.WriteInfoLine($"Running Git2SemVer version generator{(settings.Unattended ? " (unattended)" : "")}.");
            _console.WriteLine();

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
            var versionGenerator = new ProjectVersioningFactory(msg => logger.LogInfo(msg), logger).Create(inputs, new NullMSBuildGlobalProperties(), outputJsonIO);
            versionGenerator.Run();

            _console.WriteInfoLine("");
            _console.WriteInfoLine("Completed");
        }
        catch (Exception exception)
        {
            _console.WriteErrorLine(exception);
            throw;
        }
    }

    private LoggingLevel GetVerbosity(string verbosity)
    {
        if (Enum.TryParse(verbosity, true, out LoggingLevel level))
        {
            return level;
        }

        _console.WriteErrorLine($"Verbosity {verbosity} is not valid. Must be 'Trace', 'Debug', 'Info', 'Warning', or 'Error'.");
        return LoggingLevel.Info;
    }
}