using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Framework.Tests.Generation.GitHistoryWalking;

#pragma warning disable CS1591
public class GitResponseParser : GitLogCommitParserBase
{
    public GitResponseParser(ICommitsCache cache, IConventionalCommitsParser conventionalCommitParser)
        : base(cache, conventionalCommitParser)
    {
    }

    public Commit? ParseGitLogLine(string line)
    {
        return ParseCommitAndGraph(line).commit;
    }
}