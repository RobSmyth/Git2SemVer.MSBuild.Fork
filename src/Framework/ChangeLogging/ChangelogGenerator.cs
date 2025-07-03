using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Scriban;
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
    /// <summary>
    ///     Generate a new changelog document.
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
        Git2SemVerArgumentException.ThrowIfNull(releaseUrl, nameof(releaseUrl));
        Git2SemVerArgumentException.ThrowIfNullOrEmpty(scribanTemplate, nameof(scribanTemplate));

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

    public string IncrementalUpdate(string releaseUrl,
                                    IVersionOutputs versioning,
                                    ContributingCommits contributing,
                                    string scribanTemplate,
                                    string changelogToUpdate)
    {
        Git2SemVerArgumentException.ThrowIfNull(releaseUrl, nameof(releaseUrl));
        Git2SemVerArgumentException.ThrowIfNullOrEmpty(scribanTemplate, nameof(scribanTemplate));
        Git2SemVerArgumentException.ThrowIfNullOrEmpty(scribanTemplate, nameof(changelogToUpdate));

        var version = versioning.Version!;
        var changeCategories = GetChanges(version, contributing);
        // todo - trim changes to new changes only
        var template = Template.Parse(scribanTemplate);
        var model = new ChangelogModel(version,
                                       contributing,
                                       changeCategories,
                                       releaseUrl,
                                       incremental: true,
                                       createNewDocument: false);
        var render = template.Render(model, member => member.Name);

        changelogToUpdate = ReplaceSection("version", changelogToUpdate, render);

        foreach (var category in changeCategories)
        {
            // todo - what if category not found in file?
            changelogToUpdate = ReplaceSection($"{category.Settings.ChangeType} incremental changes", changelogToUpdate, render);
        }

        return changelogToUpdate;
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
        return orderedCategories.Select(category => GetCategoryChanges(category, remainingCommits, version!.IsRelease)).OfType<CategoryChanges>()
                                .ToList();
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

    private string ReplaceSection(string section, string changelogToUpdate, string render)
    {
        var pattern = $"^\\<\\!-- Start {section} section -->.*?$(?<content>.*)^<\\!-- End marker section -->";
        var regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.Singleline);
        var match = regex.Match(render);
        if (!match.Success)
        {
            var message =
                $"The generated changelog is missing missing a start or end {section} section marker marker like '<!-- Start {section} section -->'.";
            throw new Git2SemVerInvalidFormatException(message);
        }

        var content = match.Value;

        match = regex.Match(changelogToUpdate);
        if (!match.Success)
        {
            var message =
                $"The existing changelog is missing missing a start or end {section} section marker marker like '<!-- Start {section} section -->'.";
            throw new Git2SemVerInvalidFormatException(message);
        }

        return regex.Replace(changelogToUpdate, content);
    }
}