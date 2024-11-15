namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;

internal interface IVersionGenerator
{
    IVersionOutputs Run();
}