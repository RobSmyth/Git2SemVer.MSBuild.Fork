using System.Diagnostics;
using System.Text;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

internal sealed class GitSegmentsWalker(Commit head, ContributingCommits contributingCommits, ILogger logger)
{
    public SemanticVersionCalcResult CalculateSemVer()
    {
        var stopwatch = Stopwatch.StartNew();

        var result = new SemanticVersionCalcResult
        {
            HeadCommitId = head.CommitId
        };

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("    Commit      Bumps       From -> To");
        var releasedSegments = contributingCommits.LeafSegments;
        foreach (var releaseLinkedSegment in releasedSegments)
        {
            var aggregatedResult = new SegmentsAggregator().Aggregate(head, releaseLinkedSegment);

            if (logger.IsLogging(LoggingLevel.Debug))
            {
                stringBuilder.AppendLine($"    {releaseLinkedSegment.OldestCommit.CommitId.ShortSha,-12} {aggregatedResult.ChangeFlags}  {aggregatedResult.PriorVersion,10} -> {aggregatedResult.Version,-10}");
            }

            if (aggregatedResult.Version.ComparePrecedenceTo(result.Version) <= 0)
            {
                continue;
            }

            result.Version = aggregatedResult.Version;
            result.PriorReleaseCommitId = releaseLinkedSegment.OldestCommit.CommitId;
            result.PriorReleaseVersion = aggregatedResult.PriorVersion;
            result.ChangeFlags = aggregatedResult.ChangeFlags;
        }

        stopwatch.Stop();
        logger.LogDebug("Found {0} prior releases reachable from head {1} (in {2:F0} ms):\n{3}",
                         releasedSegments.Count.ToString(),
                         head.CommitId.ShortSha,
                         stopwatch.ElapsedMilliseconds,
                         stringBuilder.ToString().TrimEnd());

        return result;
    }
}