using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

internal class ChangelogWriter
{
    public static void Write(TextWriter writer, SemanticVersionCalcResult result, ContributingCommits contributing)
    {
        writer.WriteLine("""
                         
                         # Changelog
                         
                         All notable changes to this project will be documented in this file.
                                          
                         The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
                         and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
                         
                         """);

        if (contributing.Head.IsARelease)
        {
            writer.WriteLine($"## {result.Version} - _{DateTime.Now:MMMM d, yyyy}_");
        }
        else
        {
            writer.WriteLine($"""
                               
                               ## NOT RELEASED: {result.Version}
                               
                                   Generated metadata - do not edit. This metadata will not appear on a release build.
                                   
                                   Head commit:           {contributing.Head.CommitId.ShortSha}
                                   Branch:                {contributing.BranchName}
                                   Contributing commits:  {contributing.Commits.Count}
                                   
                                   Prior releases contributing to this changelog:
                                     * {string.Join("\n      * ", result.PriorVersions)}
                                   
                                   The above metadata is used to incrementally generated the changelog.
                                   Edits made below will be preserved and changes from new commits
                                   will be added to this file. Delete this file to force it to be regenerated.
                               
                               """);
        }

        var commitsByChangeType = contributing.Commits.Where(x => x.MessageMetadata.ApiChangeFlags.Any)
                                                    .ToLookup(k => k.MessageMetadata.ChangeType);

        WriteChanges(writer, "Added", commitsByChangeType[CommitChangeTypeId.Feature].ToList());
        WriteChanges(writer, "Changed", []);
        WriteChanges(writer, "Fixed", commitsByChangeType[CommitChangeTypeId.Fix].ToList());

        var customCommits = commitsByChangeType[CommitChangeTypeId.Custom].ToList();
        if (customCommits.Count <= 0)
        {
            return;
        }

        writer.WriteLine();
        WriteChanges(writer, "=== TO SORT ===", commitsByChangeType[CommitChangeTypeId.Custom].ToList());
    }

    private static void WriteChanges(TextWriter writer, string groupName, List<Commit> commits)
    {
        writer.WriteLine($"## {groupName}");
        writer.WriteLine();

        if (commits.Count == 0)
        {
            writer.WriteLine($"None.");
        }

        var changesByDescription = GetUniqueChangeCommits(commits.ToList());
        foreach (var description in changesByDescription.Keys)
        {
            writer.WriteLine($"* {description}.");
        }

        writer.WriteLine();
    }

    private static Dictionary<string, Commit> GetUniqueChangeCommits(List<Commit> commits)
    {
        var changesByDescription = new Dictionary<string, Commit>();
        foreach (var commit in commits)
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
