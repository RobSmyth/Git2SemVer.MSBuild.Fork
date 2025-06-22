using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation;

#pragma warning disable CS1591
public sealed class SemanticVersionCalcResult
{
    public SemVersion Version { get; set; } = new SemVersion(0, 0, 0);

    public CommitId PriorReleaseCommitId { get; set; } = new CommitId("Null commit");

    public SemVersion PriorReleaseVersion { get; set; } = new SemVersion(0, 1, 0);

    public ApiChangeFlags ChangeFlags { get; set; } = new ApiChangeFlags();

    public CommitId HeadCommitId { get; set; } = new CommitId("Head commit");

    public override string ToString()
    {
        return $"{PriorReleaseCommitId.ShortSha} -> {HeadCommitId.ShortSha}  {PriorReleaseVersion} -> {Version}  bumps: {ChangeFlags}";
    }
}