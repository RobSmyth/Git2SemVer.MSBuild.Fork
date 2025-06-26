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
public sealed class ReleaseState
{
    public ReleaseState(ReleaseStateId state) 
        : this(state,
               null, 
               new ApiChangeFlags())
    {
    }

    public ReleaseState(ReleaseStateId state, ApiChangeFlags changeFlags)
        : this(state, 
               null, 
               changeFlags)
    {
    }

    public ReleaseState(ReleaseStateId state, SemVersion releasedVersion)
        : this(state, releasedVersion, new ApiChangeFlags())
    {
    }

    /// <summary>
    ///     Information about a commit's release state.
    /// </summary>
    public ReleaseState(ReleaseStateId state, SemVersion? releasedVersion, ApiChangeFlags changeFlags)
    {
        ReleasedVersion = releasedVersion;
        State = state;
        ChangeFlags = changeFlags;

        if ((state == ReleaseStateId.NotReleased && ReleasedVersion != null) || 
            (state == ReleaseStateId.Released && ReleasedVersion == null))
        {
            throw new Git2SemVerInvalidOperationException($"Property {nameof(ReleasedVersion)} and the state {nameof(ReleaseStateId.NotReleased)} are mutually exclusive.");
        }
    }

    /// <summary>
    /// If <c>State</c> is <c>Released</c>, the released version.
    /// If <c>State</c> is <c>ReleaseWaypoint</c>, the prior released version.
    /// If <c>State</c> is <c>RootCommit</c>, then null.
    /// If <c>State</c> is <c>NoReleased</c>, then null.
    /// Otherwise, null.
    /// </summary>
    [JsonPropertyOrder(2)]
    public SemVersion? ReleasedVersion { get; }

    /// <summary>
    /// The commit's versioning release state. Indicates if this is a release commit and what type of release commit.
    /// </summary>
    [JsonPropertyOrder(1)]
    public ReleaseStateId State { get; }

    /// <summary>
    ///     Changes (breaking, features, or fixes).
    /// </summary>
    /// <remarks>
    ///     On a commit with a release tag all flags are false.
    /// </remarks>
    [JsonPropertyOrder(3)]
    public ApiChangeFlags ChangeFlags { get; }

    public ReleaseState Aggregate(ApiChangeFlags changeFlags)
    {
        return new ReleaseState(State, ReleasedVersion, ChangeFlags.Aggregate(changeFlags));
    }
}