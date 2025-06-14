using NoeticTools.Git2SemVer.Core.Tools.Git;


namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public sealed class ApiChanges
{
    public IList<Commit> BreakingChangeCommits { get; } = [];

    public IList<Commit> FixCommits { get; } = [];

    public ApiChangeFlags Flags { get; } = new();

    public IList<Commit> FunctionalityChangeCommits { get; } = [];

    public void Aggregate(Commit commit)
    {
        var changeFlags = commit.Metadata.ApiChangeFlags;
        if (!changeFlags.Any || commit.HasReleaseTag)
        {
            return;
        }

        Flags.Aggregate(changeFlags);

        if (changeFlags.BreakingChange)
        {
            BreakingChangeCommits.Add(commit);
        }
        else if (changeFlags.FunctionalityChange)
        {
            FunctionalityChangeCommits.Add(commit);
        }
        else if (changeFlags.Fix)
        {
            FixCommits.Add(commit);
        }
    }

    public void Aggregate(ApiChanges apiChanges)
    {
        foreach (var commit in apiChanges.BreakingChangeCommits)
        {
            Aggregate(commit);
        }

        foreach (var commit in apiChanges.FunctionalityChangeCommits)
        {
            Aggregate(commit);
        }

        foreach (var commit in apiChanges.FixCommits)
        {
            Aggregate(commit);
        }
    }
}