using NoeticTools.Common.Logging;
using Spectre.Console;


namespace NoeticTools.Git2SemVer.Tool.Framework;

public interface IConsoleIO
{
    bool HasError { get; }

    LoggingLevel Level { get; set; }

    bool Unattended { get; set; }

    T Ask<T>(string prompt, T defaultValue);

    void MarkupErrorLine(string message);

    void MarkupLine(string message);

    void MarkupWarningLine(string message);

    T Prompt<T>(TextPrompt<T> prompt, T defaultValue);

    bool PromptYesNo(string prompt, bool defaultValue = true);

    void WriteDebugLine(string message);

    void WriteErrorLine(string message);

    void WriteInfoLine(string running);

    void WriteLine();

    void WriteLine(string message);

    void WriteWarningLine(string message);
}