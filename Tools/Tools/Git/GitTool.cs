using System.Text;
using System.Text.RegularExpressions;
using Injectio.Attributes;
using NoeticTools.Common.Logging;


namespace NoeticTools.Common.Tools.Git;

#pragma warning disable CS1591
[RegisterTransient]
public class GitTool : IGitTool
{
    private readonly IGitProcessCli _inner;
    private readonly ILogger _logger;

    public GitTool(ILogger logger) : this(new GitProcessCli(logger), logger)
    {
    }

    public GitTool(IGitProcessCli inner, ILogger logger)
    {
        _logger = logger;
        _inner = inner;
        BranchName = GetBranchName();
        HasLocalChanges = GetHasLocalChanges();
    }

    public string BranchName { get; }

    public bool HasLocalChanges { get; }

    public string WorkingDirectory
    {
        get => _inner.WorkingDirectory;
        set => _inner.WorkingDirectory = value;
    }

    public IReadOnlyList<Commit> GetCommits(int skipCount, int takeCount)
    {
        var commits = new List<Commit>();

        var result = Run($"log --skip={skipCount} --max-count={takeCount} --pretty=\"format:%H|%P|%s|%d|\"");

        foreach (var line in result.Split('\n'))
        {
            if (line.Length == 0)
            {
                continue;
            }

            var regex = new Regex(@"^(?<shortSha>[^\|]*)?\|(?<parents>[^\|]*)?\|(?<summary>[^\|]*)?\|(( \(tag: (?<tags>[^\|,]+),.*\))|([^\|]*))\|$",
                                  RegexOptions.Multiline);
            var match = regex.Match(line.Trim());
            if (!match.Success)
            {
                _logger.LogWarning($"Unexpected git log line: {line}.");
            }

            var tags = GetGroupValue(match, "tags");
            var shortSha = GetGroupValue(match, "shortSha");
            var parents = GetGroupValue(match, "parents").Split(' ');
            var summary = GetGroupValue(match, "summary");
            commits.Add(new Commit(shortSha, parents, summary, tags));
        }

        _logger.LogTrace($"Read {commits.Count} commits from git history. Skipped {skipCount}.");

        return commits;
    }

    /// <summary>
    ///     Run dotnet cli with provided command line arguments.
    /// </summary>
    public int Run(string commandLineArguments,
                   TextWriter standardOut, TextWriter? errorOut = null)
    {
        return _inner.Run(commandLineArguments, standardOut, errorOut);
    }

    public string Run(string arguments)
    {
        var outStringBuilder = new StringBuilder();
        var outWriter = new StringWriter(outStringBuilder);
        var errorStringBuilder = new StringBuilder();
        var errorWriter = new StringWriter(errorStringBuilder);
        _inner.Run(arguments, outWriter, errorWriter);
        var errorOutput = errorStringBuilder.ToString();
        if (!string.IsNullOrWhiteSpace(errorOutput))
        {
            _logger.LogError($"Git command '{arguments}' returned error: {errorOutput}");
        }

        return outStringBuilder.ToString();
    }

    private string GetBranchName()
    {
        var stdOutput = Run("status -b -s --porcelain");

        var regex = new Regex(@"^## (?<branchName>\S+)\.\.\.");
        var match = regex.Match(stdOutput);

        if (!match.Success)
        {
            _logger.LogError($"Unable to read branch name from Git status. Received: '{stdOutput}'");
        }

        return match.Groups["branchName"].Value;
    }

    private static string GetGroupValue(Match match, string groupName)
    {
        var group = match.Groups[groupName];
        return group.Success ? group.Value : "";
    }

    private bool GetHasLocalChanges()
    {
        var stdOutput = Run("status -u -s --porcelain");
        return stdOutput.Length > 0;
    }
}