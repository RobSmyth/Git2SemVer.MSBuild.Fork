using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using Semver;


namespace NoeticTools.Git2SemVer.Framework;

/// <summary>
///     Git output properties for JSON serialisation.
/// </summary>
public sealed class GitOutputs : IGitOutputs
{
    [JsonConstructor]
    internal GitOutputs()
    {
    }

    internal GitOutputs(IGitTool gitTool, SemVersion priorReleaseVersion, CommitId priorReleaseCommitId)
    {
        PriorReleaseVersion = priorReleaseVersion;
        PriorReleaseCommit = gitTool.Get(priorReleaseCommitId);
        HeadCommit = gitTool.Head;
        BranchName = gitTool.BranchName;
        HasLocalChanges = gitTool.HasLocalChanges;
    }

    /// <summary>
    ///     The local Git repository head's last commit SHA.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Not output from the MSBuild task.
    ///         Following tasks should use the MSBuild property
    ///         <see href="https://learn.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target">RepositoryBranch</see>
    ///     </para>
    /// </remarks>
    public string BranchName { get; } = "";

    /// <summary>
    ///     True if there are local changes since the last commit.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Not used by Git2SemVer. Provided for C# script use.
    ///     </para>
    /// </remarks>
    public bool HasLocalChanges { get; }

    /// <summary>
    ///     The local Git repository head's last commit SHA.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Not output from the Git2SemVer MSBuild task.
    ///         Following MSBuild tasks should use the MSBuild property
    ///         <see href="https://learn.microsoft.com/en-us/nuget/reference/msbuild-targets#pack-target">RepositoryCommit</see>
    ///     </para>
    /// </remarks>
    public ICommit HeadCommit { get; } = Commit.Null;

    /// <summary>
    ///     The last release's commit. Null if no prior release found.
    /// </summary>
    [JsonPropertyName("LastReleaseCommit")]
    public ICommit? PriorReleaseCommit { get; }

    /// <summary>
    ///     The last release's version. Null if no prior release.
    /// </summary>
    [JsonPropertyName("LastReleaseVersion")]
    public SemVersion? PriorReleaseVersion { get; }

    [JsonIgnore]
    public int CommitsSinceLastRelease = 0;
}