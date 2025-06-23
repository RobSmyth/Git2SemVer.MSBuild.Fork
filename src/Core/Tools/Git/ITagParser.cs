using LibGit2Sharp;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

public interface ITagParser
{
    /// <summary>
    ///     Parse tags to identify release tags and return release version.
    /// </summary>
    ReleaseState ParseVersion(string friendlyName);

    /// <summary>
    ///     Parse git refs text, from a git log command, to identify release tags and return release version.
    /// </summary>
    SemVersion? ParseGitLogRefs(string refs);
}