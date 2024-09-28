namespace NoeticTools.Common.Exceptions;

public class Git2SemVerArgumentException : Git2SemverExceptionBase
{
    public Git2SemVerArgumentException(string message) : base(message)
    {
    }

    public Git2SemVerArgumentException(string message, Exception innerException) : base(message, innerException)
    {
    }
}