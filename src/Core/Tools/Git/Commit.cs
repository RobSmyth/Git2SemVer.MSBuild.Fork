using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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
    private readonly Regex _tagFromRefsRegex;
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

        var metadata = GetReleaseMetadata(tags);
        ReleasedVersion = metadata.Version;
        TagMetadata = metadata;
    }

    /// <summary>
    ///     Construct commit from git log information.
    /// </summary>
    public Commit(string sha, string[] parents, string summary, string messageBody, string refs,
                  ICommitMessageMetadata messageMetadata,
                  ITagParser? tagParser = null)
        : this(sha, parents, summary, messageBody, messageMetadata, tagParser ?? new TagParser())
    {
        var metadata = GetReleaseMetadata(refs);
        ReleasedVersion = metadata.Version;
        TagMetadata = metadata;
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
        _tagFromRefsRegex = new Regex(@"tag: (?<name>[^,]+)", RegexOptions.IgnoreCase);
    }

    [JsonPropertyOrder(11)]
    public CommitId CommitId { get; }

    /// <summary>
    ///     Indicates if this commit has been released.
    /// </summary>
    [JsonIgnore]
    public bool IsARelease => TagMetadata.IsARelease;

    [JsonIgnore]
    public bool IsAWaypoint => TagMetadata.IsAWaypoint;

    [JsonIgnore]
    public bool IsRootCommit => TagMetadata.IsRootCommit;

    [JsonPropertyOrder(22)]
    public string MessageBody { get; }

    /// <summary>
    ///     Commit message metadata.
    /// </summary>
    [JsonPropertyOrder(90)]
    public ICommitMessageMetadata MessageMetadata { get; }

    [JsonPropertyOrder(12)]
    public TagMetadata TagMetadata { get; } = null!;

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

    private TagMetadata GetReleaseMetadata(string gitRefs)
    {
        var defaultState = Parents.Length == 0 ? ReleaseTypeId.RootCommit : ReleaseTypeId.NotReleased;
        if (gitRefs.Length == 0)
        {
            return new TagMetadata(defaultState, MessageMetadata.ApiChangeFlags);
        }

        var tagMatches = _tagFromRefsRegex.Matches(gitRefs);
        if (tagMatches.Count == 0)
        {
            return new TagMetadata(defaultState, MessageMetadata.ApiChangeFlags);
        }

        var tagNames = new List<string>();
        foreach (Match match in tagMatches)
        {
            tagNames.Add(match.Groups["name"].Value);
        }

        return ParseTagNames(tagNames, defaultState);
    }

    private TagMetadata GetReleaseMetadata(IReadOnlyList<IGitTag>? tags)
    {
        var defaultState = Parents.Length == 0 ? ReleaseTypeId.RootCommit : ReleaseTypeId.NotReleased;
        if (tags == null || tags.Count == 0)
        {
            return new TagMetadata(defaultState, MessageMetadata.ApiChangeFlags);
        }

        var tagNames = tags.Select(x => x.FriendlyName).ToList();
        return ParseTagNames(tagNames, defaultState);
    }

    private TagMetadata ParseTagNames(List<string> tagNames, ReleaseTypeId defaultState)
    {
        var tagMetadata = new Dictionary<SemVersion, TagMetadata>();
        foreach (var tagName in tagNames)
        {
            var metadata = _tagParser.ParseTagName(tagName);
            if (metadata.ReleaseType != ReleaseTypeId.NotReleased)
            {
                tagMetadata.Add(metadata.Version!, metadata);
            }
        }

        return tagMetadata.Count > 0
            ? tagMetadata.OrderByDescending(x => x.Key, new SemverSortOrderComparer()).First().Value
            : new TagMetadata(defaultState, MessageMetadata.ApiChangeFlags);
    }
}