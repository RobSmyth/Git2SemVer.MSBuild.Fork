namespace NoeticTools.Common.Exceptions;

public class Git2SemVerRepositoryException : Git2SemverExceptionBase
{
    public Git2SemVerRepositoryException(string message) : base(message)
    {
    }

    public Git2SemVerRepositoryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}