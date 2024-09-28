using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Semver;


// ReSharper disable MergeIntoPattern

namespace NoeticTools.Common.Tools.Git;

#pragma warning disable CS1591
public class Commit
{
    private const string TagVersionPrefix = "v";
    private readonly Regex _tagVersionRegex = new(@$"^{TagVersionPrefix}(?<version>\d+\.\d+\.\d+)([\W_]|$)", RegexOptions.IgnoreCase);

    public Commit(string sha, string[] parents, string message, string tags)
    {
        CommitId = new CommitId(sha);
        if (parents.Length == 1 && parents[0].Length == 0)
        {
            Parents = [];
        }
        else
        {
            Parents = parents.Select(x => new CommitId(x)).ToArray();
        }

        Message = message;
        Tags = tags;
        ReleasedVersion = GetReleaseTag();
    }

    public CommitId CommitId { get; }

    public string Message { get; }

    public CommitId[] Parents { get; }

    public SemVersion? ReleasedVersion { get; }

    public string Tags { get; }

    [JsonIgnore]
    public static Commit Null => new("00000000", [], "null commit", "");

    private SemVersion? GetReleaseTag()
    {
        var tags = Tags.Split(' ', ',');
        if (!tags.Any())
        {
            return null;
        }

        var versions = new List<SemVersion>();
        foreach (var tag in tags)
        {
            var match = _tagVersionRegex.Match(tag);
            if (!match.Success)
            {
                continue;
            }

            var version = SemVersion.Parse(match.Groups["version"].Value, SemVersionStyles.Strict);
            version = new SemVersion(version.Major, version.Minor, version.Patch);
            versions.Add(version);
        }

        if (!versions.Any())
        {
            return null;
        }

        return versions.Max()!;
    }
}