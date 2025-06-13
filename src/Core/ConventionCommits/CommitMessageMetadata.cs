namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public sealed class CommitMessageMetadata
{
    private static readonly Dictionary<string, CommitChangeTypeId> ChangeTypeIdLookup = new()
    {
        { "feat", CommitChangeTypeId.Feature },
        { "fix", CommitChangeTypeId.Fix },
        { "build", CommitChangeTypeId.Build },
        { "chore", CommitChangeTypeId.Chore },
        { "ci", CommitChangeTypeId.ContinuousIntegration },
        { "docs", CommitChangeTypeId.Documentation },
        { "style", CommitChangeTypeId.Style },
        { "refactor", CommitChangeTypeId.Refactoring },
        { "perf", CommitChangeTypeId.Performance },
        { "test", CommitChangeTypeId.Testing }
    };

    public CommitMessageMetadata(string changeType, bool breakingChangeFlagged, string changeDescription, string body,
                                 List<(string key, string value)> footerKeyValues)
    {
        ChangeType = ToChangeTypeId(changeType.ToLower());
        ChangeDescription = changeDescription;
        Body = body;
        FooterKeyValues = footerKeyValues.ToLookup(k => k.key, v => v.value);

        var apiChanges = new ApiChangeFlags
        {
            FunctionalityChange = ChangeType == CommitChangeTypeId.Feature,
            Fix = ChangeType == CommitChangeTypeId.Fix,
            BreakingChange = breakingChangeFlagged ||
                             FooterKeyValues.Contains("BREAKING-CHANGE") ||
                             FooterKeyValues.Contains("BREAKING CHANGE")
        };
        ApiChangeFlags = apiChanges;
    }

    public CommitMessageMetadata() : this("", false, "", "", [])
    {
    }

    public ApiChangeFlags ApiChangeFlags { get; }

    public string Body { get; }

    public string ChangeDescription { get; }

    public CommitChangeTypeId ChangeType { get; }

    public ILookup<string, string> FooterKeyValues { get; }

    private static CommitChangeTypeId ToChangeTypeId(string value)
    {
        // ReSharper disable once CanSimplifyDictionaryTryGetValueWithGetValueOrDefault
        if (ChangeTypeIdLookup.TryGetValue(value, out var changeTypeId))
        {
            return changeTypeId;
        }

        return CommitChangeTypeId.None;
    }
}