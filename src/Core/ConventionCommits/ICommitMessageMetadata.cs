namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public interface ICommitMessageMetadata
{
    ApiChangeFlags ApiChangeFlags { get; }

    string Body { get; }

    string ChangeDescription { get; }

    CommitChangeTypeId ChangeType { get; }

    ILookup<string, string> FooterKeyValues { get; }

    /// <summary>
    /// The raw change type text found in the commit message.
    /// Useful if the <c>ChangeType</c> is <c>CommitChangeTypeId.Custom</c>.
    /// </summary>
    string ChangeTypeText { get; }
}