namespace NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;

internal interface IDetectableBuildHost : IBuildHost
{
    bool MatchesHostSignature();
}