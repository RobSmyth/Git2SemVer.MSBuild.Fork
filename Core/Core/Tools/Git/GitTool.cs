using System.Diagnostics;
using Injectio.Attributes;
using LibGit2Sharp;

//using LibGit2Sharp;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git.FluentApi;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using Semver;


#pragma warning disable SYSLIB1045
#pragma warning disable CS1591

namespace NoeticTools.Git2SemVer.Core.Tools.Git;

[RegisterTransient]
public class GitTool : IGitTool, IDisposable
{
    private const int DefaultTakeLimit = 300;
    private readonly SemVersion _assumedLowestGitVersion = new(2, 34, 1);
    private readonly ICommitsCache? _cache;
    private readonly IGitResponseParser _gitResponseParser;
    private readonly IGitProcessCli _inner;
    private readonly ILogger _logger;
    private int _commitsReadCountFromHead;
    private Commit? _head;
    private bool _initialised;
    private string _repositoryDirectory;
    private Repository? _repository;
    private readonly ConventionalCommitsParser _metadataParser;

    public GitTool(ILogger logger)
    {
        _cache = new CommitsCache();
        _inner = new GitProcessCli(logger);
        _gitResponseParser = new GitResponseParser(_cache, new ConventionalCommitsParser(), logger);
        _logger = logger;
        RepositoryDirectory = DiscoverRepositoryDirectory(_inner.WorkingDirectory);
        _metadataParser = new ConventionalCommitsParser();
    }

    public string RepositoryDirectory
    {
        get => _repositoryDirectory;
        set
        {
            _inner.WorkingDirectory = value;
            _repositoryDirectory = DiscoverRepositoryDirectory(value);
        }
    }

    private string DiscoverRepositoryDirectory(string currentDirectory)
    {
        return currentDirectory.EndsWith(".git") ? 
            currentDirectory : 
            new DirectoryInfo(Repository.Discover(currentDirectory)).Parent!.FullName;
    }

    public string BranchName => Repository.Head.FriendlyName;

    public ICommitsCache Cache
    {
        get
        {
            if (_cache == null)
            {
                PrimeCache();
            }

            return _cache!;
        }
    }

    public bool HasLocalChanges => GetHasLocalChanges();

    public Commit Head
    {
        get
        {
            if (_head == null)
            {
                PrimeCache();
            }

            return _head!;
        }
        private set => _head = value;
    }

    public Commit Get(CommitId commitId)
    {
        return Get(commitId.Sha);
    }

    public Commit Get(string commitSha)
    {
        if (Cache.TryGet(commitSha, out var existingCommit))
        {
            return existingCommit;
        }

        var commits = GetCommitsLibGit2Sharp(commitSha);
        if (commits.Count == 0)
        {
            throw new Git2SemVerRepositoryException($"Unable to find git commit '{commitSha}' in the repository.");
        }

        Cache.Add(commits);
        return Cache.Get(commitSha);
    }

    internal IReadOnlyList<Commit> GetCommitsLibGit2Sharp(string commitSha)
    {
        return Repository.Commits.QueryBy(new CommitFilter()
        {
            IncludeReachableFrom = commitSha,
        }).Take(DefaultTakeLimit).Select(Convert).ToList();
    }

    internal IReadOnlyList<Commit> GetCommitsLibGit2Sharp(int skipCount)
    {
        //>>> todo - temporary
        if (skipCount > 0)
        {
            throw new ArgumentException("Must be 0", nameof(skipCount));
        }
        //>>>

        Repository.Commits.Skip(skipCount).Take(DefaultTakeLimit);

        var result = Repository.Commits;
        var commits = result.Select(Convert).ToList();
        if (skipCount == 0)
        {
            _head = commits[0];
        }

        return commits;
    }

    private Commit Convert(LibGit2Sharp.Commit rawCommit)
    {
        if (!Cache.TryGet(rawCommit.Sha, out var commit))
        {
            var parents = rawCommit.Parents.Select(x => x.Sha).ToArray();
            var metadata = _metadataParser.Parse(rawCommit.MessageShort, rawCommit.Message);
            var tags = Repository.Tags.Where(x => x.Target.Equals(rawCommit)).ToList();

            commit = new Commit(
                rawCommit.Sha,
                parents, rawCommit.MessageShort,
                rawCommit.Message,
                metadata,
                tags);

            Cache.Add(commit);
        }

        return commit;
    }

    private Repository Repository => _repository ??= new Repository(RepositoryDirectory);

    /// <summary>
    ///     Get next set of commits from head.
    /// </summary>
    private IReadOnlyList<Commit> GetCommitsLibGit2Sharp()
    {
        var commits = GetCommitsLibGit2Sharp(_commitsReadCountFromHead);
        if (_commitsReadCountFromHead == 0)
        {
            if (commits.Count == 0)
            {
                throw new Git2SemVerGitOperationException("Unable to get commits. Either new repository and no commits or problem accessing git.");
            }
            _head = commits[0];
        }
        _commitsReadCountFromHead += commits.Count;
        return commits;
    }

    private bool GetHasLocalChanges()
    {
        return Task.Run(GetHasLocalChangesAsync).Result;
    }

    private Task<bool> GetHasLocalChangesAsync()
    {
        var statusOptions = new StatusOptions();
        var status = Repository.RetrieveStatus(statusOptions);
        return Task.FromResult(status.IsDirty);
    }

    private void PrimeCache()
    {
        Task.Run(PrimeCacheAsync).Wait();
    }

    private async Task PrimeCacheAsync()
    {
        if (_initialised)
        {
            return;
        }

        _initialised = true;

        var commits = GetCommitsLibGit2Sharp();
        Cache.Add(commits.ToArray());
    }

    public void Dispose()
    {
        _repository?.Dispose();
    }
}