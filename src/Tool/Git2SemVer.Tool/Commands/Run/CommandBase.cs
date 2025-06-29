using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Tool.Framework;


namespace NoeticTools.Git2SemVer.Tool.Commands.Run;

internal abstract class CommandBase
{
    protected readonly IConsoleIO Console;

    protected CommandBase(IConsoleIO console)
    {
        Console = console;
    }

    public bool HasError => Console.HasError;

    protected LoggingLevel GetVerbosity(string verbosity)
    {
        if (Enum.TryParse(verbosity, true, out LoggingLevel level))
        {
            return level;
        }

        Console.WriteErrorLine($"Verbosity {verbosity} is not valid. Must be 'Trace', 'Debug', 'Info', 'Warning', or 'Error'.");
        return LoggingLevel.Info;
    }
}