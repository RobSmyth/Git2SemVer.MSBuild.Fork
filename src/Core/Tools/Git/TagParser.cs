using System.Text.RegularExpressions;
using LibGit2Sharp;
using NoeticTools.Git2SemVer.Core.Exceptions;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

/// <summary>
/// Parse tags to identify release tags and return release version.
/// </summary>
#pragma warning disable CS1591
[RegisterSingleton]
public sealed class TagParser : ITagParser
{
    private readonly Regex _tagVersionFromRefsRegex;
    private readonly Regex _tagVersionRegex;
    private const string VersionPlaceholder = "%VERSION%";
    private const string DefaultVersionPrefix = "v";
    private const string VersionPattern = @"(?<version>\d+\.\d+\.\d+)";

    public TagParser(string? releaseTagFormat = null)
    {
        var parsePattern = GetParsePattern(releaseTagFormat);
        _tagVersionFromRefsRegex = new Regex($"tag: {parsePattern}", RegexOptions.IgnoreCase);
        _tagVersionRegex = new Regex($"^{parsePattern}", RegexOptions.IgnoreCase);
    }

    private static string GetParsePattern(string? releaseTagFormat)
    {
        if (string.IsNullOrWhiteSpace(releaseTagFormat))
        {
            return DefaultVersionPrefix + VersionPattern;
        }

        if (releaseTagFormat!.StartsWith("^", StringComparison.InvariantCulture))
        {
            throw new Git2SemVerConfigurationException($"The build property `Git2SemVer_ReleaseTagFormat` value `{releaseTagFormat}` may not commence with '^'.");//>>> use exception supporting diagnostic code
        }

        if (!releaseTagFormat!.Contains(VersionPlaceholder))
        {
            throw new Git2SemVerConfigurationException($"The build property `Git2SemVer_ReleaseTagFormat` value `{releaseTagFormat}` must contain the placeholder text '{VersionPattern}'.");//>>> use exception supporting diagnostic code
        }

        return releaseTagFormat!.Replace(VersionPlaceholder, VersionPattern);
    }

    public SemVersion? Parse(Tag tag)
    {
        var match = _tagVersionRegex.Match(tag.FriendlyName);
        return !match.Success ? null : SemVersion.Parse(match.Groups["version"].Value, SemVersionStyles.Strict);
    }

    public IReadOnlyList<SemVersion> Parse(string refs)
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
            var value = match.Groups["version"].Value;
            var version = SemVersion.Parse(value, SemVersionStyles.Strict);
            versions.Add(version);
        }

        return versions;
    }
}