using System.Dynamic;
using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Exceptions;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;

#pragma warning disable CS1591
/// <summary>
///     Information about a commit's release state.
/// </summary>
public sealed class CommitMetadata
{
    public CommitMetadata(ReleaseTypeId state) 
        : this(state,
               null, 
               new ApiChangeFlags())
    {
    }

    public CommitMetadata(ReleaseTypeId state, ApiChangeFlags changeFlags)
        : this(state, 
               null, 
               changeFlags)
    {
    }

    public CommitMetadata(ReleaseTypeId state, SemVersion version)
        : this(state, version, new ApiChangeFlags())
    {
    }

    /// <summary>
    ///     Information about a commit's release state.
    /// </summary>
    public CommitMetadata(ReleaseTypeId state, SemVersion? version, ApiChangeFlags changeFlags)
    {
        Version = version;
        ReleaseType = state;
        ChangeFlags = changeFlags;

        if ((state == ReleaseTypeId.NotReleased && Version != null) || 
            (state == ReleaseTypeId.Released && Version == null))
        {
            throw new Git2SemVerInvalidOperationException($"Property {nameof(Version)} and the state {nameof(ReleaseTypeId.NotReleased)} are mutually exclusive.");
        }
    }

    [JsonIgnore]
    public bool IsRootCommit => ReleaseType == ReleaseTypeId.RootCommit;

    [JsonIgnore]
    public bool IsARelease => ReleaseType == ReleaseTypeId.Released;

    /// <summary>
    /// Version to be used in the context of the State (ReleaseStateId).
    /// </summary>
    /// <remarks>
    /// If <c>State</c> is <c>Released</c>, the released version.
    /// If <c>State</c> is <c>ReleaseWaypoint</c>, the prior released version.
    /// If <c>State</c> is <c>RootCommit</c>, then null.
    /// If <c>State</c> is <c>NoReleased</c>, then null.
    /// </remarks>
    [JsonPropertyOrder(2)]
    public SemVersion? Version { get; }

    /// <summary>
    /// The commit's versioning release state. Indicates if this is a release commit and what type of release commit.
    /// </summary>
    [JsonPropertyOrder(1)]
    public ReleaseTypeId ReleaseType { get; }

    /// <summary>
    ///     Changes (breaking, features, or fixes).
    /// </summary>
    /// <remarks>
    ///     On a commit with a release tag all flags are false.
    /// </remarks>
    [JsonPropertyOrder(3)]
    public ApiChangeFlags ChangeFlags { get; }

    //public CommitMetadata Aggregate(CommitMetadata changeFlags)
    //{
    //    if (State == ReleaseStateId.Released)
    //    {
    //        return new CommitMetadata(State, Version, new ApiChangeFlags());
    //    }
    //    return new CommitMetadata(State, Version, ChangeFlags.Aggregate(changeFlags));
    //}
}