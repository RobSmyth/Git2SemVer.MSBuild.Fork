using System.Text.Json.Serialization;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

/// <summary>
/// Configuration that defines changelog categories that are included in the changelog.
/// </summary>
public sealed class ChangelogCategorySettings
{
    [JsonConstructor]
    public ChangelogCategorySettings()
    {
    }

    public ChangelogCategorySettings(int order, string name, string changeType, bool skipIfNone, bool skipIfRelease = false)
    {
        ChangeType = changeType;
        Name = name;
        SkipIfNone = skipIfNone;
        SkipIfRelease = skipIfRelease;
        Order = order;
    }

    /// <summary>
    /// Regular expression pattern matched against Git summary Conventional Commits change type like "feat" or "fix".
    /// </summary>
    public string ChangeType { get; set; } = "";

    /// <summary>
    /// The category name to show in the changelog. e.g: "Added".
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The relative order in which the category will appear in the changelog.
    /// Lower number appears before higher numbers.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Set to <c>true</c> to skip this category in the changelog if no changes found.
    /// </summary>
    public bool SkipIfNone { get; set; }

    /// <summary>
    ///     If true the category will not be excluded from a release's changelog.
    /// </summary>
    public bool SkipIfRelease { get; set; }
}