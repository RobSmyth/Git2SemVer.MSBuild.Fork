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
        ReleasedVersion = releaseState.ReleasedVersion;
        ReleaseState = releaseState;
    }

    /// <summary>
    ///     Construct commit from git log information.
    /// </summary>
    public Commit(string sha, string[] parents, string summary, string messageBody, string refs,
                  ICommitMessageMetadata metadata,
                  ITagParser? tagParser = null)
        : this(sha, parents, summary, messageBody, metadata, tagParser ?? new TagParser())
    {
        ReleasedVersion = ParseGitLogRefsForReleasedVersion(refs);
        var state = Parents.Length == 0 ? ReleaseStateId.RootCommit :
            ReleasedVersion == null ? ReleaseStateId.NotReleased : ReleaseStateId.Released;
        ReleaseState = new ReleaseState(state,
                                        ReleasedVersion,
                                        new ApiChangeFlags());
    }

    private Commit(string sha, string[] parents, string summary, string messageBody,
                   ICommitMessageMetadata metadata,
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
        Metadata = metadata;
    }

    [JsonPropertyOrder(11)]
    public CommitId CommitId { get; }

    [JsonIgnore]
    public bool HasReleaseTag => ReleasedVersion != null;

    [JsonPropertyOrder(22)]
    public string MessageBody { get; }

    [JsonPropertyOrder(90)]
    public ICommitMessageMetadata Metadata { get; }

    /// <summary>
    ///     A null commit.
    /// </summary>
    [JsonIgnore]
    public static Commit Null => new("00000000", [], "null commit", "", "", new CommitMessageMetadata());

    [JsonPropertyOrder(31)]
    public CommitId[] Parents { get; }

    [JsonIgnore]
    public SemVersion? ReleasedVersion { get; } // depreciated

    [JsonPropertyOrder(12)]
    public ReleaseState ReleaseState { get; } = null!;

    [JsonPropertyOrder(21)]
    public string Summary { get; }

    [JsonIgnore]
    public IReadOnlyList<IGitTag> Tags { get; } = [];

    private ReleaseState GetReleaseState(IReadOnlyList<IGitTag>? tags)
    {
        var defaultState = Parents.Length == 0 ? ReleaseStateId.RootCommit : ReleaseStateId.NotReleased;
        if (tags == null || tags.Count == 0)
        {
            return new ReleaseState(defaultState, Metadata.ApiChangeFlags);
        }

        var releaseStates = new Dictionary<SemVersion, ReleaseState>();
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var tag in tags)
        {
            var tagReleaseState = _tagParser.ParseTagName(tag.FriendlyName);
            if (tagReleaseState.State != ReleaseStateId.NotReleased)
            {
                releaseStates.Add(tagReleaseState.ReleasedVersion!, tagReleaseState);
            }
        }

        return releaseStates.Count > 0
            ? releaseStates.OrderByDescending(x => x.Key, new SemverSortOrderComparer()).First().Value
            : new ReleaseState(defaultState, Metadata.ApiChangeFlags);
    }

    /// <summary>
    ///     Used to generate commits from git log history. Used by tests.
    /// </summary>
    private SemVersion? ParseGitLogRefsForReleasedVersion(string refs)
    {
        return refs.Length == 0 ? null : _tagParser.ParseGitLogRefs(refs);
    }
}