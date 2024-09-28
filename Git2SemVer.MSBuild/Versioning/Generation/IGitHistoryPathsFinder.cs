using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistory;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;

internal interface IGitHistoryPathsFinder
{
    HistoryPaths FindPathsToHead();
}