namespace NoeticTools.Git2SemVer.Core.Diagnostics;

public class DiagnosticCodeBase
{
    protected DiagnosticCodeBase(int id,
                                 string description,
                                 string resolution,
                                 string message,
                                 params object[] messageArgs)
    {
        Code = $"GSV{id:D3}";
        Description = description;
        Resolution = resolution;
        Message = string.Format(message, messageArgs);
    }

    public string Message { get; }

    public string MessageWithCode => $"{Code}: {Message}";

    public string SubCategory { get; } = "Versioning";

    public string Code { get; }

    public string Description { get; }

    public string Resolution { get; }

    public string HelpLink => $"https://noetictools.github.io/Git2SemVer.MSBuild/Reference/ErrorCodes/{Code}.html";
}