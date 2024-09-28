namespace NoeticTools.Common.Exceptions;

public abstract class Git2SemverExceptionBase : Exception
{
    public Git2SemverExceptionBase(string message) : base(message)
    {
    }

    public Git2SemverExceptionBase(string message, Exception innerException) : base(message, innerException)
    {
    }
}