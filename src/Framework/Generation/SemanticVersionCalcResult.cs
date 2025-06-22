using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation;

#pragma warning disable CS1591
public sealed class SemanticVersionCalcResult
{
    public ApiChangeFlags ChangeFlags { get; set; } = new();

    public CommitId HeadCommitId { get; set; } = new("Head commit");

    public CommitId PriorReleaseCommitId { get; set; } = new("Null commit");

    public SemVersion PriorReleaseVersion { get; set; } = new(0, 1, 0);

    public SemVersion Version { get; set; } = new(0, 0, 0);

    public override string ToString()
    {
        return $"{PriorReleaseCommitId.ShortSha} -> {HeadCommitId.ShortSha}  {PriorReleaseVersion} -> {Version}  bumps: {ChangeFlags}";
    }
}