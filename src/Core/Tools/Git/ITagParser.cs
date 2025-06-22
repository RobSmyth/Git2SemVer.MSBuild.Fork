using LibGit2Sharp;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

public interface ITagParser
{
    /// <summary>
    ///     Parse tags to identify release tags and return release version.
    /// </summary>
    SemVersion? Parse(Tag tag);

    /// <summary>
    ///     Parse git refs text, from a git log command, to identify release tags and return release version.
    /// </summary>
    IReadOnlyList<SemVersion> Parse(string refs);
}