using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;


namespace NoeticTools.Git2SemVer.Framework.Generation;

public interface IVersionGenerator : IDisposable
{
    /// <summary>
    /// Perform a prebuild versioning run. Depending on the host may bump the build number.
    /// </summary>
    IVersionOutputs PrebuildRun();

    /// <summary>
    /// Generate versioning outputs. Build number is not bumped and outputs are not saved.
    /// </summary>
    (VersionOutputs Outputs, ContributingCommits Contributing) CalculateSemanticVersion();
}