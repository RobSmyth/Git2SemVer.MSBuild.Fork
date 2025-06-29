using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

internal static class ChangelogWriter
{
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
        WriteChanges(writer, 
                     """
                     === TO SORT ===
                     
                         These changes use custom change types and cannot automatically
                         be added to the conventional Added, Changed, or Fixed lists.
                     """, 
                     commitsByChangeType[CommitChangeTypeId.Custom].ToList());
    }

    private static Dictionary<string, Commit> GetUniqueChangeCommits(List<Commit> commits)
    {
        var changesByDescription = new Dictionary<string, Commit>();
        foreach (var commit in commits)
        {
            // todo - duplicate must include change type - return result type
            // todo - collect issue number(s) from commits with duplicate ==change type== and description

            var description = commit.MessageMetadata.ChangeDescription;
            // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
            if (!changesByDescription.ContainsKey(description))
            {
                changesByDescription.Add(description, commit);
            }
        }

        return changesByDescription;
    }

    private static void WriteChanges(TextWriter writer, string groupName, List<Commit> commits)
    {
        writer.WriteLine($"## {groupName}");
        writer.WriteLine();

        if (commits.Count == 0)
        {
            writer.WriteLine("None.");
        }

        var changesByDescription = GetUniqueChangeCommits(commits.ToList());
        foreach (var description in changesByDescription.Keys)
        {

            writer.WriteLine($"* {description}.");
        }

        writer.WriteLine();
    }
}