using Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Framework.Semver;

/// <summary>
///     <see cref="SemVersion" /> extensions.
/// </summary>
public static class SemVersionExtensions
{
    /// <summary>
    ///     Returns true if the provided version is an [initial development version](https://semver.org/#spec-item-4).
    /// </summary>
    public static bool IsInEarlyDevelopment(this SemVersion semVersion)
    {
        return semVersion.Major == 0;
    }
}