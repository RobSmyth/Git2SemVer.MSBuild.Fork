using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public sealed class ApiChanges
{
    public ApiChangeFlags Flags { get; } = new();
}