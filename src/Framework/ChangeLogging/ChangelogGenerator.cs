using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Scriban;
using Scriban.Runtime;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

/*
 * todo:
 * - Changelog update mode
 * - Read existing file
 * - Store used commits in file of same name as the target changelog file (with .g2sv.json extension)
 */
public class ChangelogGenerator(ChangelogSettings config)
{
    public string IncrementalUpdate(string releaseUrl,
                           IVersionOutputs versioning,
                           ContributingCommits contributing,
                           string scribanTemplate,
                           string changelog)
    {
        // todo
        var version = versioning.Version!;
        var changes = GetChanges(version, contributing);
        var template = Template.Parse(scribanTemplate);
        var model = new ChangelogModel(version, 
                                       contributing, 
                                       changes, 
                                       releaseUrl,
                                       incremental: true,
                                       createNewDocument: false);
        return template.Render(model, member => member.Name);
    }

    /// <summary>
    /// Generate a new changelog document.
    /// </summary>
    /// <param name="releaseUrl"></param>
    /// <param name="versioning"></param>
    /// <param name="contributing"></param>
    /// <param name="scribanTemplate"></param>
    /// <param name="incremental"></param>
    /// <returns></returns>
    public string Generate(string releaseUrl,
                           IVersionOutputs versioning,
                           ContributingCommits contributing,
                           string scribanTemplate,
                           bool incremental)
    {
        var version = versioning.Version!;
        var changes = GetChanges(version, contributing);
        var template = Template.Parse(scribanTemplate);
        var model = new ChangelogModel(version, 
                                       contributing, 
                                       changes, 
                                       releaseUrl,
                                       incremental,
                                       createNewDocument: true);
        return template.Render(model, member => member.Name);
    }

    private static List<Commit> Extract(List<Commit> remainingCommits, string changeType)
    {
        var regex = new Regex(changeType);
        var extracted = remainingCommits.Where(x => regex.IsMatch(x.MessageMetadata.ChangeTypeText)).ToList();
        extracted.ForEach(x => remainingCommits.Remove(x));
        return extracted;
    }

    private static CategoryChanges? GetCategoryChanges(ChangelogCategorySettings categorySettings, List<Commit> remainingCommits, bool isRelease)
    {
        var commits = Extract(remainingCommits, categorySettings.ChangeType);
        if ((categorySettings.SkipIfNone && commits.Count <= 0) || (isRelease && !categorySettings.SkipIfRelease))
        {
            return null;
        }

        var categoryChanges = new CategoryChanges(categorySettings);
        categoryChanges.AddRange(GetUniqueChangelogEntries(commits));
        return categoryChanges;
    }

    private IReadOnlyList<CategoryChanges> GetChanges(SemVersion version, ContributingCommits contributing)
    {
        var remainingCommits = new List<Commit>(contributing.Commits.Where(x => x.MessageMetadata.ApiChangeFlags.Any));
        var orderedCategories = config.Categories.OrderBy(x => x.Order);
        return orderedCategories.Select(category => GetCategoryChanges(category, remainingCommits, version!.IsRelease)).OfType<CategoryChanges>().ToList();
    }

    private static IReadOnlyList<ChangeLogEntry> GetUniqueChangelogEntries(IReadOnlyList<Commit> commits)
    {
        var changeEntries = new List<ChangeLogEntry>();
        foreach (var commit in commits)
        {
            // >>> todo - incremental change. reject change if commitId is recorded in LastRun. Add commits to ChangeLogEntry

            var entry = changeEntries.SingleOrDefault(x => x.Equals(commit.MessageMetadata));
            if (entry == null)
            {
                entry = new ChangeLogEntry(commit.MessageMetadata);
                changeEntries.Add(entry);
            }
            else
            {
                // todo - incremental update will need to check issues count
                entry.AddIssues(commit.MessageMetadata.FooterKeyValues["issues"]);
            }

            entry.AddCommitId(commit.CommitId.ShortSha);
        }

        return changeEntries;
    }
}