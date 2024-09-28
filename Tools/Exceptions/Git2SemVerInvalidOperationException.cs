namespace NoeticTools.Common.Exceptions;

public class Git2SemVerInvalidOperationException : Git2SemverExceptionBase
{
    public Git2SemVerInvalidOperationException(string message) : base(message)
    {
    }

    public Git2SemVerInvalidOperationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}