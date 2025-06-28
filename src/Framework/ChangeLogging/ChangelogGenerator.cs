using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

internal class ChangelogGenerator
{
    public void Build(SemVersion version, Commit head, string branchName, IReadOnlyList<Commit> contributingCommits, TextWriter writer)
    {
        if (head.IsARelease)
        {
            writer.WriteLine($"# {version}");
            writer.WriteLine($"_{DateTime.Now.ToLongDateString()}_");
        }
        else
        {
            writer.WriteLine($"# NOT RELEASED");
            writer.WriteLine($"Up to head commit {head.CommitId.ShortSha} on branch {branchName}.");
            writer.WriteLine($"{contributingCommits.Count} contributing commits");
        }
        writer.WriteLine();

        var commitsByChangeType = contributingCommits.Where(x => x.MessageMetadata.ApiChangeFlags.Any)
                                                    .ToLookup(k => k.MessageMetadata.ChangeType);

        foreach (var commitsOfType in commitsByChangeType)
        {
            writer.WriteLine($"## {commitsOfType.Key} changes");
            writer.WriteLine();

            var changesByDescription = GetUniqueChangeCommits(commitsOfType);
            foreach (var description in changesByDescription.Keys)
            {
                writer.WriteLine($"* {description}.");
            }

            writer.WriteLine();
        }
    }

    private static Dictionary<string, Commit> GetUniqueChangeCommits(IGrouping<CommitChangeTypeId, Commit> commitsOfType)
    {
        var changesByDescription = new Dictionary<string, Commit>();
        foreach (var commit in commitsOfType)
        {
            var description = commit.MessageMetadata.ChangeDescription;
            // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
            if (!changesByDescription.ContainsKey(description))
            {
                changesByDescription.Add(description, commit);
            }
        }

        return changesByDescription;
    }
}
