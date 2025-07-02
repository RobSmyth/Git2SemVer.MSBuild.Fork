using System.Text;
using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Scriban;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public class ChangelogGenerator(ChangelogSettings config)
{
    public static readonly ChangelogCategorySettings[] DefaultCategories =
    [
        new(1, "Added", "feat", false),
        new(2, "Changed", "change", false),
        new(3, "Depreciated", "deprecate", true),
        new(4, "Removed", "remove", true),
        new(5, "Fixed", "fix", false),
        new(6, "Security", "security", true),
        new(7, "Other", ".*", true)
    ];

    public string Generate(string releaseUrl,
                           IVersionOutputs versioning,
                           ContributingCommits contributing,
                           string scribanTemplate)
    {
        var version = versioning.Version!;
        var changes = GetChanges(version, contributing);
        var template = Template.Parse(scribanTemplate);
        var model = new ChangelogModel(version, contributing, changes, releaseUrl);
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
        var changes = new List<CategoryChanges>();

        var remainingCommits = new List<Commit>(contributing.Commits.Where(x => x.MessageMetadata.ApiChangeFlags.Any));
        var orderedCategories = config.Categories.OrderBy(x => x.Order);
        foreach (var category in orderedCategories)
        {
            var categoryChanges = GetCategoryChanges(category, remainingCommits, version!.IsRelease);
            if (categoryChanges != null)
            {
                changes.Add(categoryChanges);
            }
        }

        return changes;
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

    public static string GetDefaultTemplate()
    {
        var assembly = typeof(ChangelogGenerator).Assembly;
        var resourcePath = assembly.GetManifestResourceNames()
                                      .Single(str => str.EndsWith("MarkdownChangelog.scriban"))!;
        using var stream = assembly.GetManifestResourceStream(resourcePath)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}