using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using Semver;


#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS0612 // Type or member is obsolete

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
                  ICommitMessageMetadata messageMetadata, ITagParser tagParser, IReadOnlyList<IGitTag>? tags)
        : this(sha, parents, summary, messageBody, messageMetadata, tagParser)
    {
        if (tags != null)
        {
            Tags = tags;
        }

        var releaseState = GetReleaseState(tags);
        ReleasedVersion = releaseState.Version;
        Metadata = releaseState;
    }

    /// <summary>
    ///     Construct commit from git log information.
    /// </summary>
    public Commit(string sha, string[] parents, string summary, string messageBody, string refs,
                  ICommitMessageMetadata messageMetadata,
                  ITagParser? tagParser = null)
        : this(sha, parents, summary, messageBody, messageMetadata, tagParser ?? new TagParser())
    {
        ReleasedVersion = _tagParser.ParseGitLogRefs(refs);
        var state = Parents.Length == 0 ? ReleaseTypeId.RootCommit :
            ReleasedVersion == null ? ReleaseTypeId.NotReleased : ReleaseTypeId.Released;
        Metadata = new CommitMetadata(state,
                                      ReleasedVersion,
                                      new ApiChangeFlags());
    }

    private Commit(string sha, string[] parents, string summary, string messageBody,
                   ICommitMessageMetadata messageMetadata,
                   ITagParser tagParser)
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
        MessageMetadata = messageMetadata;
    }

    [JsonPropertyOrder(11)]
    public CommitId CommitId { get; }

    /// <summary>
    ///     Indicates if this commit has been released.
    /// </summary>
    [JsonIgnore]
    public bool IsARelease => Metadata.IsARelease;

    [JsonIgnore]
    public bool IsRootCommit => Metadata.IsRootCommit;

    [JsonPropertyOrder(22)]
    public string MessageBody { get; }

    /// <summary>
    ///     Commit message metadata.
    /// </summary>
    [JsonPropertyOrder(90)]
    public ICommitMessageMetadata MessageMetadata { get; }

    [JsonPropertyOrder(12)]
    public CommitMetadata Metadata { get; } = null!;

    /// <summary>
    ///     A null commit.
    /// </summary>
    [JsonIgnore]
    public static Commit Null => new("00000000", [], "null commit", "", "", new CommitMessageMetadata());

    [JsonPropertyOrder(31)]
    public CommitId[] Parents { get; }

    [JsonIgnore]
    public SemVersion? ReleasedVersion { get; } // depreciated

    [JsonPropertyOrder(21)]
    public string Summary { get; }

    [JsonIgnore]
    public IReadOnlyList<IGitTag> Tags { get; } = [];

    private CommitMetadata GetReleaseState(IReadOnlyList<IGitTag>? tags)
    {
        var defaultState = Parents.Length == 0 ? ReleaseTypeId.RootCommit : ReleaseTypeId.NotReleased;
        if (tags == null || tags.Count == 0)
        {
            return new CommitMetadata(defaultState, MessageMetadata.ApiChangeFlags);
        }

        var releaseStates = new Dictionary<SemVersion, CommitMetadata>();
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var tag in tags)
        {
            var tagReleaseState = _tagParser.ParseTagName(tag.FriendlyName);
            if (tagReleaseState.ReleaseType != ReleaseTypeId.NotReleased)
            {
                releaseStates.Add(tagReleaseState.Version!, tagReleaseState);
            }
        }

        return releaseStates.Count > 0
            ? releaseStates.OrderByDescending(x => x.Key, new SemverSortOrderComparer()).First().Value
            : new CommitMetadata(defaultState, MessageMetadata.ApiChangeFlags);
    }
}