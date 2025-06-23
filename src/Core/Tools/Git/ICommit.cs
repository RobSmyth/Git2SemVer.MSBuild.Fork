using System.Text.Json.Serialization;
using LibGit2Sharp;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

[JsonDerivedType(typeof(Commit), "Commit")]
public interface ICommit
{
    /// <summary>
    ///     Commit ID.
    /// </summary>
    CommitId CommitId { get; }

    /// <summary>
    ///     Commit message body or description.
    /// </summary>
    string MessageBody { get; }

    /// <summary>
    ///     This commit's parent commits.
    /// </summary>
    CommitId[] Parents { get; }

    /// <summary>
    ///     The released version parsed from an associated release tag or null if no release tag found.
    /// </summary>
    SemVersion? ReleasedVersion { get; }

    /// <summary>
    ///     Commit message summary or title.
    /// </summary>
    string Summary { get; }

    /// <summary>
    ///     Tags associated with this commit.
    /// </summary>
    IReadOnlyList<Tag>? Tags { get; }

    ReleaseState ReleaseState { get; }
}