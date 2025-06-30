using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Semver;
using System.Globalization;
using NoeticTools.Git2SemVer.Core;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

internal static class ChangelogWriter
{
    private static readonly string[] FeatureChangeVerbs =
    [
        "change",
        "optimise",
        "optimize",
        "reduce",
        "remove",
        "improve"
    ];

    public static void Write(TextWriter writer, IVersionOutputs versioning, ContributingCommits contributing)
    {
        writer.WriteLine("""

                         # Changelog

                         All notable changes to this project will be documented in this file.

                         The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
                         and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

                         """);

        writer.WriteLine(versioning.Version!.IsRelease
                             ? $"## {versioning.Version} - _{DateTime.Now:MMMM d, yyyy}_"
                             : $"""

                                ## {versioning.Version}

                                    Generated metadata - do not edit. This metadata will not appear on a release build.
                                    
                                    Head commit:           {contributing.Head.CommitId.ShortSha}
                                    Branch:                {contributing.BranchName}
                                    Contributing commits:  {contributing.Commits.Count}
                                    
                                    The above metadata is used to incrementally generated the changelog.
                                    Edits made below will be preserved and changes from new commits
                                    will be added to this file. Delete this file to force it to be regenerated.

                                """);

        var remainingCommits = new List<Commit>(contributing.Commits.Where(x => x.MessageMetadata.ApiChangeFlags.Any));

        var changeCommits = remainingCommits.Where(x => x.MessageMetadata.ChangeType == CommitChangeTypeId.Feature &&
                                                        StartsWithChangeVerb(x.MessageMetadata.ChangeDescription)).ToList();
        remainingCommits.RemoveAll(x => changeCommits.Contains(x));

        var addCommits = remainingCommits.Where(x => x.MessageMetadata.ChangeType == CommitChangeTypeId.Feature).ToList();
        remainingCommits.RemoveAll(x => addCommits.Contains(x));

        var fixCommits = remainingCommits.Where(x => x.MessageMetadata.ChangeType == CommitChangeTypeId.Fix).ToList();
        remainingCommits.RemoveAll(x => fixCommits.Contains(x));

        WriteChanges(writer, "Added", addCommits);
        WriteChanges(writer, "Changed", changeCommits);
        WriteChanges(writer, "Fixed", fixCommits);

        if (remainingCommits.Count == 0)
        {
            return;
        }
        if (versioning.Version.IsPrerelease)
        {
            writer.WriteLine("""
                                 The following categories may need to further grouped.
                             """);
        }
        writer.WriteLine();
        var categories = remainingCommits.ToLookup(k => k.MessageMetadata.ChangeTypeText.ToSentenceCase());
        foreach (var category in categories)
        {
            WriteChanges(writer, category.Key, category.ToList());
        }
    }

    private static bool StartsWithChangeVerb(string description)
    {
        return FeatureChangeVerbs.Any(x => description.StartsWith(x, StringComparison.CurrentCultureIgnoreCase));
    }

    private static IReadOnlyList<ChangeLogEntry> GetUniqueChangelogEntries(IReadOnlyList<Commit> commits)
    {
        var changeEntries = new List<ChangeLogEntry>();
        foreach (var commit in commits)
        {
            var entry = changeEntries.SingleOrDefault(x => x.Equals(commit.MessageMetadata));
            if (entry == null)
            {
                entry = new ChangeLogEntry(commit.MessageMetadata);
                changeEntries.Add(entry);
            }
            else
            {
                entry.AddIssues(commit.MessageMetadata.FooterKeyValues["issues"]);
            }
        }

        return changeEntries;
    }

    private static void WriteChanges(TextWriter writer, string category, IReadOnlyList<Commit> commits)
    {
        writer.WriteLine($"## {category}");
        writer.WriteLine();

        if (commits.Count == 0)
        {
            writer.WriteLine("None.");
        }

        var changeEntries = GetUniqueChangelogEntries(commits);
        foreach (var change in changeEntries)
        {
            var issuesSuffix = change.Issues.Count == 0 ? "" : $" ({string.Join(", ", change.Issues)})";
            writer.WriteLine($"* {change.Description.ToSentenceCase()}{issuesSuffix}.");
        }

        writer.WriteLine();
    }
}