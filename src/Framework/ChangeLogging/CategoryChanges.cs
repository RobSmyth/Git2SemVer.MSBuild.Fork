// ReSharper disable UnusedMember.Global

namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public sealed class CategoryChanges(ChangelogCategorySettings settings)
{
    private readonly List<ChangeLogEntry> _changes = [];

    public IReadOnlyList<ChangeLogEntry> Changes => _changes;

    public ChangelogCategorySettings Settings { get; } = settings;

    public void AddRange(IReadOnlyList<ChangeLogEntry> changes)
    {
        _changes.AddRange(changes);
    }
}