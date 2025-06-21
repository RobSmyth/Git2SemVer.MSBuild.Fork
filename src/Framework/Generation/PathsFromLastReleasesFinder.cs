using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

// ReSharper disable CanSimplifyDictionaryLookupWithTryAdd

namespace NoeticTools.Git2SemVer.Framework.Generation;

internal sealed class PathsFromLastReleasesFinder(IGitTool gitTool, ILogger logger) : IGitHistoryPathsFinder
{
    /// <summary>
    /// Find all git history paths from head to preceding releases.
    /// </summary>
    /// <returns></returns>
    public HistoryPaths FindPathsToHead()
    {
        logger.LogDebug("Walking git history.");
        using (logger.EnterLogScope())
        {
            var stopwatch = Stopwatch.StartNew();
            var segments = new VersionHistorySegmentsBuilder(gitTool, logger).BuildTo(gitTool.Head);
            new NextReleaseVersionFinder(segments, logger).Find(gitTool.Head); //>>> testing
            var paths = new VersionHistoryPathsBuilder(segments, logger).BuildTo();
            stopwatch.Stop();
            logger.LogDebug($"Git history walk completed (in {stopwatch.Elapsed.TotalSeconds:F1} seconds).");
            return paths;
        }
    }
}