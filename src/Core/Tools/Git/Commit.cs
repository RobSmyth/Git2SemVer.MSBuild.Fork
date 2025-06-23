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
    ///     Git commit.
    /// </summary>
    public Commit(string sha, string[] parents, string summary, string messageBody,
                  CommitMessageMetadata metadata, ITagParser tagParser, IReadOnlyList<Tag>? tags)
        : this(sha, parents, summary, messageBody, metadata, tagParser)
    {
        if (tags != null)
        {
            Tags = tags;
        }

        var releaseInfo = GetReleasedVersion(tags);
        ReleasedVersion = releaseInfo.ReleasedVersion;
        ReleaseState = releaseInfo;
    }

    /// <summary>
    ///     Construct commit from git log information.
    /// </summary>
    public Commit(string sha, string[] parents, string summary, string messageBody, string refs, CommitMessageMetadata metadata,
                  ITagParser? tagParser = null)
        : this(sha, parents, summary, messageBody, metadata, tagParser ?? new TagParser())
    {
        ReleasedVersion = GetReleasedVersion(refs);
        ReleaseState = new ReleaseState(ReleasedVersion == null ? ReleaseStateId.NotReleased : ReleaseStateId.Released,
                                       ReleasedVersion,
                                       new ApiChangeFlags());
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

    //[JsonPropertyOrder(12)]
    [JsonIgnore]
    public SemVersion? ReleasedVersion { get; } // being depreciated

    [JsonPropertyOrder(12)]
    public ReleaseState ReleaseState { get; } = null!;

    [JsonPropertyOrder(21)]
    public string Summary { get; }

    [JsonIgnore]
    public IReadOnlyList<Tag> Tags { get; } = [];

    private ReleaseState GetReleasedVersion(IReadOnlyList<Tag>? tags)
    {
        if (tags == null || tags.Count == 0)
        {
            return new ReleaseState();
        }

        var releaseInfos = new Dictionary<SemVersion, ReleaseState>();
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var tag in tags)
        {
            var tagReleaseInfo = _tagParser.ParseVersion(tag.FriendlyName);
            if (tagReleaseInfo.ReleasedVersion != null)
            {
                releaseInfos.Add(tagReleaseInfo.ReleasedVersion, tagReleaseInfo);
            }
        }

        if (releaseInfos.Count == 0)
        {
            return new ReleaseState();
        }

        var releaseInfo = releaseInfos.OrderByDescending(x => x.Key, new SemverSortOrderComparer()).FirstOrDefault();

        return releaseInfo.Value;
    }

    /// <summary>
    ///     Legacy use to get release tag by parsing refs text from git log output.
    /// </summary>
    private SemVersion? GetReleasedVersion(string refs)
    {
        if (refs.Length == 0)
        {
            return null;
        }

        return _tagParser.ParseGitLogRefs(refs);
    }
}