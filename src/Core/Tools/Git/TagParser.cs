using System.Text.RegularExpressions;
using LibGit2Sharp;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

#pragma warning disable CS1591
[RegisterSingleton]
public sealed class TagParser : ITagParser
{
    private readonly Regex _tagVersionFromRefsRegex = new(@$"tag: {TagVersionPrefix}(?<version>\d+\.\d+\.\d+)", RegexOptions.IgnoreCase);
    private readonly Regex _tagVersionRegex = new(@$"^{TagVersionPrefix}(?<version>\d+\.\d+\.\d+)", RegexOptions.IgnoreCase);
    private const string TagVersionPrefix = "v";

    public SemVersion? Parse(Tag tag)
    {
        var match = _tagVersionRegex.Match(tag.FriendlyName);
        return !match.Success ? null : SemVersion.Parse(match.Groups["version"].Value, SemVersionStyles.Strict);
    }

    public List<SemVersion> Parse(string refs)
    {
        if (refs.Length == 0)
        {
            return [];
        }

        var matches = _tagVersionFromRefsRegex.Matches(refs);
        if (matches.Count == 0)
        {
            return [];
        }

        var versions = new List<SemVersion>();
        foreach (Match match in matches)
        {
            var version = SemVersion.Parse(match.Groups["version"].Value, SemVersionStyles.Strict);
            versions.Add(version);
        }

        return versions;
    }
}