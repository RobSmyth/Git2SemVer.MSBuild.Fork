using System.Text.Json.Serialization;
using LibGit2Sharp;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

#pragma warning disable CS1591
// ReSharper disable MergeIntoPattern
public class Commit : ICommit
{
    private readonly ITagParser _tagParser;

    /// <summary>
    ///     /Construct commit from LibGit2Sharp objects.
    /// </summary>
    public Commit(string sha, string[] parents, string summary, string messageBody,
                  CommitMessageMetadata metadata, ITagParser tagParser, IReadOnlyList<Tag>? tags)
        : this(sha, parents, summary, messageBody, metadata, tagParser)
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
    public Commit(string sha, string[] parents, string summary, string messageBody, string refs, CommitMessageMetadata metadata, ITagParser? tagParser = null)
        : this(sha, parents, summary, messageBody, metadata, tagParser ?? new TagParser())
    {
        ReleasedVersion = GetReleaseTag(refs);
    }

    private Commit(string sha, string[] parents, string summary, string messageBody, CommitMessageMetadata metadata, ITagParser tagParser)
    {
        _tagParser = tagParser;
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
    public static Commit Null => new("00000000", [], "null commit", "", "", new CommitMessageMetadata());

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
            var version = _tagParser.Parse(tag);
            if (version != null)
            {
                versions.Add(version);
            }
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

        var versions = _tagParser.Parse(refs);
        return versions.Count == 0 ? null : versions.OrderByDescending(x => x, new SemverSortOrderComparer()).FirstOrDefault();
    }
}