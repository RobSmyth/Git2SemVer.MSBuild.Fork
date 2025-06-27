using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;

public interface ITagParser
{
    /// <summary>
    ///     Parse git refs text, from a git log command, to identify release tags and return release version.
    /// </summary>
    SemVersion? ParseGitLogRefs(string refs);

    /// <summary>
    ///     Parse tags to identify release and waypoint tags.
    /// </summary>
    CommitMetadata ParseTagName(string friendlyName);
}