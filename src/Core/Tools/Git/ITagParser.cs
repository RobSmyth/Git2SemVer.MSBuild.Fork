using LibGit2Sharp;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

public interface ITagParser
{
    SemVersion? Parse(Tag tag);
    List<SemVersion> Parse(string refs);
}