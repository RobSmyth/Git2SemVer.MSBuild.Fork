namespace NoeticTools.Common.Exceptions;

public class Git2SemVerTimeoutException : Git2SemverExceptionBase
{
    public Git2SemVerTimeoutException(string message) : base(message)
    {
    }

    public Git2SemVerTimeoutException(string message, Exception innerException) : base(message, innerException)
    {
    }
}