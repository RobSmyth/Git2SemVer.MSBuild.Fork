using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation;

internal interface IGitHistoryPathsFinder
{
    SemanticVersionCalcResult CalculateSemanticVersion();
}