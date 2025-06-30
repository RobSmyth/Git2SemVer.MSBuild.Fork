using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public sealed class ChangelogCategory
{
    [JsonConstructor]
    public ChangelogCategory()
    {}

    public ChangelogCategory(int order, string name, CommitChangeTypeId typeId, bool skipIfNone)
    {
        ChangeType = (int)typeId;
        Name = name;
        SkipIfNone = skipIfNone;
        Order = order;
    }

    public int ChangeType { get; set; } = 0;

    public string Name { get; set; } = "";

    public bool SkipIfNone { get; set; } = false;

    public int Order { get; set; } = 0;
}