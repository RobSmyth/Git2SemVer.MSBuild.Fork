using System.Text;
using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Scriban;
using Scriban.Runtime;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public class MarkdownGenerator(ILogger logger, ChangelogSettings config)
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

    public void Generate(string releaseUrl,
                         bool writeToConsole,
                         string outputFilePath,
                         IVersionOutputs versioning,
                         ContributingCommits contributing,
                         string template)
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

    private void Write(TextWriter writer,
                       IVersionOutputs versioning,
                       ContributingCommits contributing)
    {
        var version = versioning.Version!;

        var changes = GetChanges(version, contributing);

        var sectionHeader = """
                            # Changelog

                            All notable changes to this project will be documented in this file.

                            The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
                            and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

                            {{~ if IsRelease ~}}
                            ## [{{ SemVersion }}](releaseUrl/{{ SemVersion }}) - _{{ ReleaseDate | date.to_string '%F' }}_
                            {{~ else ~}}

                            ## Unreleased: {{ SemVersion }} - _{{ ReleaseDate | date.to_string '%F' }}_<a id='prerelease-{{ SemVersion }}'></a>
                                
                                Generated metadata - do not edit. This metadata will not appear on a release build.
                                
                                Head commit:           {{ HeadSha }}
                                Branch:                {{ BranchName }}
                                Contributing commits:  {{ NumberOfCommits }}
                                
                                Edits made below will be preserved and changes from new commits
                                will be added to this file. Delete this file to force it to be regenerated.
                                
                                A pre-release's changelog includes changes since last release.
                                On a release all pre-release changes are rolled into the release changelog
                                and no pre-release versions will be shown.
                            {{~ end ~}}

                            {{~ for category in Categories ~}}
                            ### {{ category.Settings.Name }}<a id='change-type-{{ category.Settings.ChangeType }}'></a>
                            {{~ if category.Changes | array.size == 0 ~}}
                            None.
                            {{~ else ~}}
                            {{~ for change in category.Changes ~}}
                            {{~ 
                              if change.Issues | array.size == 0
                                issues = ""
                              else
                                issues = "(" + (change.Issues | array.join ",") + ")"
                              end
                            ~}}
                            * {{ change.Description | string.capitalize }}{{ issues }}.
                            {{~ end ~}}
                            {{~ end ~}}
                            {{~ end ~}}
                            """;
        var template = Template.Parse(sectionHeader);

        var model = new ChangelogModel(version, contributing, changes);
        sectionHeader = template.Render(model, member => member.Name);
        writer.WriteLine(sectionHeader);
    }

    public static string GetDefaultTemplate()
    {
        var assembly = typeof(MarkdownGenerator).Assembly;
        var resourcePath = assembly.GetManifestResourceNames()
                                      .Single(str => str.EndsWith("MarkdownChangelog.scriban"))!;
        using var stream = assembly.GetManifestResourceStream(resourcePath)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}