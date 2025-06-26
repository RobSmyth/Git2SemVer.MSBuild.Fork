using System.Text.Json.Serialization;

namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public interface ICommitMessageMetadata
{
    [JsonPropertyOrder(1)]
    ApiChangeFlags ApiChangeFlags { get; }

    [JsonPropertyOrder(2)]
    string Body { get; }

    [JsonPropertyOrder(3)]
    string ChangeDescription { get; }

    [JsonPropertyOrder(4)]
    CommitChangeTypeId ChangeType { get; }

    /// <summary>
    /// The raw change type text found in the commit message.
    /// Useful if the <c>ChangeType</c> is <c>CommitChangeTypeId.Custom</c>.
    /// </summary>
    [JsonPropertyOrder(5)]
    string ChangeTypeText { get; }

    [JsonPropertyOrder(6)]
    ILookup<string, string> FooterKeyValues { get; }
}