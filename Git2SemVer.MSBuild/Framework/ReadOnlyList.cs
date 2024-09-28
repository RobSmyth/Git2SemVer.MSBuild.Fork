// Copied from semver NuGet package to resolve runtime build issues.

namespace NoeticTools.Git2SemVer.MSBuild.Framework;

/// <summary>
///     Internal helper for efficiently creating empty read only lists
/// </summary>
internal static class ReadOnlyList<T>
{
    public static readonly IReadOnlyList<T> Empty = new List<T>().AsReadOnly();
}