namespace NoeticTools.Common.Tools.DotnetCli;

public interface IDotNetTool
{
    IDotNetSolutionCommands Solution { get; }

    IDotNetProjectCommands Projects { get; }

    (int returnCode, string stdOutput) Run(string commandLineArguments);

    /// <summary>
    ///     Run dotnet cli with provided command line arguments.
    /// </summary>
    int Run(string commandLineArguments,
            TextWriter standardOut, TextWriter? errorOut = null);
}