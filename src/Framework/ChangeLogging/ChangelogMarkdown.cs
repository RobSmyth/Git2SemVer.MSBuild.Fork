using System.Text;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public class ChangelogMarkdown(ILogger logger)
{
    private const string ReleaseUrl = $"https://www.nuget.org/packages/NoeticTools.Git2SemVer.MSBuild/{VersionPlaceholder}";
    private const string VersionPlaceholder = "%VERSION%";

    private static readonly Dictionary<CommitChangeTypeId, string> CategoryNameLookup = new()
    {
        { CommitChangeTypeId.None, "" },
        { CommitChangeTypeId.Feature, "Added" },
        { CommitChangeTypeId.Fix, "Fixed" },
        { CommitChangeTypeId.Change, "Changed" },
        { CommitChangeTypeId.Deprecate, "Depreciated" },
        { CommitChangeTypeId.Remove, "Removed" },
        { CommitChangeTypeId.Security, "Security" },
        { CommitChangeTypeId.Custom, "Other" }
    };

    public void Generate(bool writeToConsole,
                         string outputFilePath,
                         IVersionOutputs versioning,
                         ContributingCommits contributing)
    {
        var stringBuilder = new StringBuilder();
        using var writer = new StringWriter(stringBuilder);
        writer.WriteLine();

        Write(writer, versioning, contributing);

        writer.WriteLine();
        var body = stringBuilder.ToString();
        if (writeToConsole)
        {
            Console.Out.WriteLine(body);
        }

        if (outputFilePath.Length == 0)
        {
            logger.LogDebug("Write changelog to file is disabled.");
            return;
        }

        logger.LogDebug("Writing changelog to: {0}", outputFilePath);
        File.WriteAllText(outputFilePath, body);
    }

    private static List<Commit> Extract(List<Commit> remainingCommits, CommitChangeTypeId changeType)
    {
        var extracted = remainingCommits.Where(x => x.MessageMetadata.ChangeType == changeType).ToList();
        extracted.ForEach(x => remainingCommits.Remove(x));
        return extracted;
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

    private static void Write(TextWriter writer,
                              IVersionOutputs versioning,
                              ContributingCommits contributing)
    {
        writer.WriteLine("""

                         # Changelog

                         All notable changes to this project will be documented in this file.

                         The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
                         and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

                         """);

        writer.WriteLine(versioning.Version!.IsRelease
                             ? $"## [{versioning.Version}]({ReleaseUrl.Replace(VersionPlaceholder, versioning.Version!.ToString())}) - _{DateTime.Now:yyyy-MM-dd}_"
                             : $"""

                                ## Unreleased

                                    {versioning.Version} - _{DateTime.Now:yyyy-MM-dd}_
                                    
                                    Generated metadata - do not edit. This metadata will not appear on a release build.
                                    
                                    Head commit:           {contributing.Head.CommitId.ShortSha}
                                    Branch:                {contributing.BranchName}
                                    Contributing commits:  {contributing.Commits.Count}
                                    
                                    The above metadata is used to incrementally generated the changelog.
                                    Edits made below will be preserved and changes from new commits
                                    will be added to this file. Delete this file to force it to be regenerated.
                                    
                                    A pre-release's changelog includes changes since last release.
                                    On a release all pre-release changes are rolled into the release changelog
                                    and no pre-release versions will be shown.

                                """);

        var remainingCommits = new List<Commit>(contributing.Commits.Where(x => x.MessageMetadata.ApiChangeFlags.Any));

        WriteChanges(writer, remainingCommits, CommitChangeTypeId.Feature);
        WriteChanges(writer, remainingCommits, CommitChangeTypeId.Change);
        WriteChanges(writer, remainingCommits, CommitChangeTypeId.Deprecate, true);
        WriteChanges(writer, remainingCommits, CommitChangeTypeId.Remove, true);
        WriteChanges(writer, remainingCommits, CommitChangeTypeId.Fix);
        WriteChanges(writer, remainingCommits, CommitChangeTypeId.Security, true);
        WriteChanges(writer, remainingCommits, CommitChangeTypeId.Custom, true);
    }

    private static void WriteChanges(TextWriter writer, List<Commit> remainingCommits, CommitChangeTypeId changeType, bool skipIfNone = false)
    {
        var extracted = Extract(remainingCommits, changeType);
        if (!skipIfNone || extracted.Count > 0)
        {
            WriteChanges(writer, CategoryNameLookup[changeType], extracted);
        }
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