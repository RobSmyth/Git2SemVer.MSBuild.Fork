namespace NoeticTools.Git2SemVer.Framework.Generation;

internal interface IVersionGenerator : IDisposable
{
    IVersionOutputs Run();
}