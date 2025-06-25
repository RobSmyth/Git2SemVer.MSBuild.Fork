using System.Dynamic;
using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;

#pragma warning disable CS1591
/// <summary>
///     Information about a commit's release state.
/// </summary>
public sealed class ReleaseState(ReleaseStateId state, SemVersion? releasedVersion, ApiChangeFlags changeFlags)
{
    public ReleaseState(ReleaseStateId state) 
        : this(state, new SemVersion(0, 0, 0), new ApiChangeFlags())
    {
    }

    public ReleaseState(ReleaseStateId state, ApiChangeFlags changeFlags)
        : this(state, new SemVersion(0,0,0), changeFlags)
    {
    }

    public ReleaseState(ReleaseStateId state, SemVersion releasedVersion)
        : this(state, releasedVersion, new ApiChangeFlags())
    {
    }

    /// <summary>
    /// If <c>State</c> is <c>Released</c>, the released version.
    /// If <c>State</c> is <c>ReleaseWaypoint</c>, the prior released version.
    /// If <c>State</c> is <c>RootCommit</c>, then version is `0.1.0`.
    /// If <c>State</c> is <c>NoReleased</c>, then version is `0.0.0`.
    /// Otherwise, null.
    /// </summary>
    [JsonPropertyOrder(2)]
    public SemVersion? ReleasedVersion { get; } = releasedVersion;

    /// <summary>
    /// The commit's versioning release state. Indicates if this is a release commit and what type of release commit.
    /// </summary>
    [JsonPropertyOrder(1)]
    public ReleaseStateId State { get; } = state;

    /// <summary>
    ///     Changes (breaking, features, or fixes).
    /// </summary>
    /// <remarks>
    ///     On a commit with a release tag all flags are false.
    /// </remarks>
    [JsonPropertyOrder(3)]
    public ApiChangeFlags ChangeFlags { get; } = changeFlags;

    public ReleaseState Aggregate(ApiChangeFlags changeFlags)
    {
        return new ReleaseState(State, ReleasedVersion, ChangeFlags.Aggregate(changeFlags));
    }
}