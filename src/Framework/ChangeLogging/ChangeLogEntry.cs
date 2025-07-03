using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public sealed class ChangeLogEntry : IEquatable<ICommitMessageMetadata>
{
    private readonly List<string> _issues = [];
    private readonly HashSet<string> _commitIds = [];
    private readonly ICommitMessageMetadata _messageMetadata;

    public ChangeLogEntry(ICommitMessageMetadata messageMetadata)
    {
        _messageMetadata = messageMetadata;
        AddIssues(messageMetadata.FooterKeyValues["issue"]);
        AddIssues(messageMetadata.FooterKeyValues["refs"]);
    }

    public string Description => _messageMetadata.ChangeDescription;

    public IReadOnlyList<string> Issues => _issues;

    public void AddIssues(IEnumerable<string> issueIds)
    {
        foreach (var issueId in issueIds)
        {
            AddIssue(issueId);
        }
    }

    public bool Equals(ICommitMessageMetadata? other)
    {
        if (other == null)
        {
            return false;
        }

        return _messageMetadata.ChangeTypeText.Equals(other.ChangeTypeText) &&
               _messageMetadata.ChangeDescription.Equals(other.ChangeDescription);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is ChangeLogEntry other && Equals(other));
    }

    public override int GetHashCode()
    {
        return _messageMetadata.GetHashCode();
    }

    private void AddIssue(string issueId)
    {
        if (!_issues.Contains(issueId))
        {
            _issues.Add(issueId);
        }
    }

    private bool Equals(ChangeLogEntry? other)
    {
        return other != null && _messageMetadata.Equals(other._messageMetadata);
    }

    public void AddCommitId(string commitSha)
    {
        _commitIds.Add(commitSha);
    }

    public bool HasCommitId(string commitSha)
    {
        return _commitIds.Contains(commitSha);
    }
}