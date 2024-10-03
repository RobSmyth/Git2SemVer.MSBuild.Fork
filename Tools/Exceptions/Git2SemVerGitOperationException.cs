namespace NoeticTools.Common.Exceptions;

public class Git2SemVerGitOperationException : Git2SemverExceptionBase
{
    public Git2SemVerGitOperationException(string message) : base(message)
    {
    }

    public Git2SemVerGitOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}