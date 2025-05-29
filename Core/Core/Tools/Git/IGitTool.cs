using NoeticTools.Git2SemVer.Core.Tools.Git.FluentApi;


namespace NoeticTools.Git2SemVer.Core.Tools.Git;

public interface IGitTool
{
    /// <summary>
    ///     The current head's branch name.
    /// </summary>
    string BranchName { get; }

    ICommitsCache Cache { get; }

    /// <summary>
    ///     True if there are uncommited local changes.
    /// </summary>
    bool HasLocalChanges { get; }

    Commit Head { get; }

    string RepositoryDirectory { get; set; }

    Commit Get(CommitId commitId);

    Commit Get(string commitSha);

    /// <summary>
    ///     Get commits in range given by range builder.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Example usage:
    ///     </para>
    ///     <code>
    ///         var commits = GetCommits(x => x.ReachableFrom(commitSha)
    ///                                        .NotReachableFrom(prior));
    ///     </code>
    /// </remarks>
    Task<IReadOnlyList<Commit>> GetCommitsAsync(Action<IGitRevisionsBuilder> rangeBuilderAction);

    /// <summary>
    ///     Get all commits contributing to code at a commit after a prior commit.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is a shortcut for:
    ///     </para>
    ///     <code>
    ///         var commits = GetCommits(x => x.ReachableFrom(commitSha)
    ///                                        .NotReachableFrom(prior));
    ///     </code>
    /// </remarks>
    IReadOnlyList<Commit> GetContributingCommits(CommitId commit, CommitId prior);

    /// <summary>
    ///     Get all commits contributing to code at a commit after a prior commit.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is a shortcut for:
    ///     </para>
    ///     <code>
    ///         var commits = GetCommits(x => x.ReachableFrom(commitSha)
    ///                                        .NotReachableFrom(prior));
    ///     </code>
    /// </remarks>
    Task<IReadOnlyList<Commit>> GetContributingCommitsAsync(CommitId commit, CommitId prior);

    /// <summary>
    ///     Invoke Git with given arguments.
    /// </summary>
    string Run(string arguments);

    /// <summary>
    ///     Invoke Git with given arguments.
    /// </summary>
    Task<string> RunAsync(string arguments);
}