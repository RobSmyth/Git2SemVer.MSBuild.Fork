using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

#pragma warning disable CS1591
// ReSharper disable MergeIntoPattern

public class Commit : ICommit
{
    private const string TagVersionPrefix = "v";
    private readonly Regex _tagVersionFromRefsRegex = new (@$"tag: {TagVersionPrefix}(?<version>\d+\.\d+\.\d+)", RegexOptions.IgnoreCase);
    private readonly Regex _tagVersionRegex = new (@$"^{TagVersionPrefix}(?<version>\d+\.\d+\.\d+)", RegexOptions.IgnoreCase);

    /// <summary>
    ///     /Construct commit from LibGit2Sharp objects.
    /// </summary>
    public Commit(string sha, string[] parents, string summary, string messageBody,
                  CommitMessageMetadata metadata, IReadOnlyList<Tag>? tags)
        : this(sha, parents, summary, messageBody, metadata)
    {
        if (tags != null)
        {
            Tags = tags;
        }

        ReleasedVersion = GetReleaseTag(tags);
    }

    /// <summary>
    ///     Construct commit from git log information.
    /// </summary>
    public Commit(string sha, string[] parents, string summary, string messageBody, string refs, CommitMessageMetadata metadata)
        : this(sha, parents, summary, messageBody, metadata)
    {
        ReleasedVersion = GetReleaseTag(refs);
    }

    private Commit(string sha, string[] parents, string summary, string messageBody, CommitMessageMetadata metadata)
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

        Summary = summary;
        MessageBody = messageBody;
        Metadata = metadata;
    }

    [JsonPropertyOrder(11)]
    public CommitId CommitId { get; }

    [JsonIgnore]
    public bool HasReleaseTag => ReleasedVersion != null;

    [JsonPropertyOrder(22)]
    public string MessageBody { get; }

    [JsonPropertyOrder(90)]
    public CommitMessageMetadata Metadata { get; }

    [JsonIgnore]
    public static Commit Null => new ("00000000", [], "null commit", "", "", new CommitMessageMetadata());

    [JsonPropertyOrder(31)]
    public CommitId[] Parents { get; }

    [JsonPropertyOrder(12)]
    public SemVersion? ReleasedVersion { get; }

    [JsonPropertyOrder(21)]
    public string Summary { get; }

    [JsonIgnore]
    public IReadOnlyList<Tag> Tags { get; } = [];

    private SemVersion? GetReleaseTag(IReadOnlyList<Tag>? tags)
    {
        if (tags == null || tags.Count == 0)
        {
            return null;
        }

        var versions = new List<SemVersion>();
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var tag in tags)
        {
            var match = _tagVersionRegex.Match(tag.FriendlyName);
            if (!match.Success)
            {
                continue;
            }

            var version = SemVersion.Parse(match.Groups["version"].Value, SemVersionStyles.Strict);
            versions.Add(version);
        }

        return versions.OrderByDescending(x => x, new SemverSortOrderComparer()).FirstOrDefault();
    }

    /// <summary>
    ///     Legacy use to get release tag by parsing refs text from git log output.
    /// </summary>
    private SemVersion? GetReleaseTag(string refs)
    {
        if (refs.Length == 0)
        {
            return null;
        }

        var matches = _tagVersionFromRefsRegex.Matches(refs);
        if (matches.Count == 0)
        {
            return null;
        }

        var versions = new List<SemVersion>();
        foreach (Match match in matches)
        {
            var version = SemVersion.Parse(match.Groups["version"].Value, SemVersionStyles.Strict);
            versions.Add(version);
        }

        return versions.OrderByDescending(x => x, new SemverSortOrderComparer()).FirstOrDefault();
    }
}