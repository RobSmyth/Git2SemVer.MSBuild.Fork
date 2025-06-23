using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;

#pragma warning disable CS1591
/// <summary>
///     Information about a commit's release state.
/// </summary>
public sealed class ReleaseState
{
    public ReleaseState() : this(ReleaseStateId.NotReleased, new SemVersion(0, 0, 0), new ApiChangeFlags())
    {
    }

    public ReleaseState(ReleaseStateId state, SemVersion? releasedVersion, ApiChangeFlags waypointChangeFlags)
    {
        WaypointChangeFlags = waypointChangeFlags;
        ReleasedVersion = releasedVersion;
        State = state;
    }

    /// <summary>
    /// If <c>State</c> is <c>Released</c>, the released version.
    /// If <c>State</c> is <c>ReleaseWaypoint</c>, the prior released version.
    /// If <c>State</c> is <c>RootCommit</c>, then version `0.1.0`.
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
    /// If the <c>State</c> is not <c>NotReleased</c> then the change flags up to this commit from the prior release commit.
    /// </summary>
    [JsonPropertyOrder(3)]
    public ApiChangeFlags WaypointChangeFlags { get; }
}