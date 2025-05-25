using System.Text;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;

#pragma warning disable CS1591

internal sealed class VersionHistoryPath : IVersionHistoryPath
{
    private readonly List<VersionHistorySegment> _segments = [];
    private ApiChanges _apiChanges = new();
    private int? _commitsCount;

    public VersionHistoryPath(params VersionHistorySegment[] segments)
    {
        _segments.AddRange(segments);
        LastReleasedVersion = segments.First().TaggedReleasedVersion;
        Version = GetNextReleaseVersion();
    }

    public int CommitsSinceLastRelease
    {
        get
        {
            _commitsCount ??= _segments.Sum(x => x.Commits.Count);
            return _commitsCount.Value;
        }
    }

    public Commit FirstCommit => _segments.First().OldestCommit;

    public Commit HeadCommit => _segments.Last().YoungestCommit;

    public int Id { get; internal set; }

    public SemVersion? LastReleasedVersion { get; }

    public SemVersion Version { get; }

    public string GetVersioningReport()
    {
        var firstCommitSha = FirstCommit.CommitId.ShortSha;
        var stringBuilder = new StringBuilder();
        var version = new SemVersion(0, 1, 0);

        stringBuilder.AppendLine($"Path {Id} report:");

        if (FirstCommit.ReleasedVersion != null)
        {
            version = FirstCommit.ReleasedVersion!;

            stringBuilder.AppendLine($"""
                                        - Starts at prior release {FirstCommit.ReleasedVersion} commit {firstCommitSha}.
                                      """);
        }
        else
        {
            stringBuilder.AppendLine($"""
                                        - Starts at Commit {firstCommitSha} version 0.1.0. This is the first commit in the repository.
                                          No releases found on this path. Project is in initial development phase.
                                          See:
                                            - https://semver.org/#spec-item-4
                                            - https://semver.org/#how-should-i-deal-with-revisions-in-the-0yz-initial-development-phase

                                      """);
        }

        if (_apiChanges.Flags.BreakingChange)
        {
            version = new SemVersion(version.Major + 1, 0, 0);

            stringBuilder.AppendLine($"""
                                        - Version bumped to {version} as one or more breaking changes made.
                                          Adding features or fixes will not impact the version.
                                          See:
                                            - https://semver.org/#spec-item-8
                                            - https://www.conventionalcommits.org/en/v1.0.0/
                                      """);
        }
        else if (_apiChanges.Flags.FunctionalityChange)
        {
            version = new SemVersion(version.Major, version.Minor + 1, 0);

            stringBuilder.AppendLine($"""
                                        - Version bumped to {version} as one or more features added.
                                          Adding fixes will not impact the version.
                                          See:
                                            - https://semver.org/#spec-item-7
                                            - https://www.conventionalcommits.org/en/v1.0.0/
                                      """);
        }
        else if (_apiChanges.Flags.Fix)
        {
            version = new SemVersion(version.Major, version.Minor, version.Patch + 1);

            stringBuilder.AppendLine($"""
                                        - Version bumped to {version} as one or more fixes added.
                                          See:
                                            - https://semver.org/#spec-item-6
                                            - https://www.conventionalcommits.org/en/v1.0.0/
                                      """);
        }
        else if (FirstCommit.ReleasedVersion != null)
        {
            version = new SemVersion(version.Major, version.Minor, version.Patch + 1);

            stringBuilder.AppendLine($"""
                                        - Version bumped to {version} as no change commit messages found and prior release version cannot be reused.
                                          See:
                                            - https://semver.org/#spec-item-3
                                            - https://www.conventionalcommits.org/en/v1.0.0/
                                      """);
        }
        else
        {
            stringBuilder.AppendLine("""
                                       - Version not bumped as there have no been any releases and no change commit messages found.
                                         See:
                                           - https://semver.org/#how-should-i-deal-with-revisions-in-the-0yz-initial-development-phase
                                           - https://www.conventionalcommits.org/en/v1.0.0/
                                     """);
        }

        return stringBuilder.ToString().TrimEnd();
    }

    public override string ToString()
    {
        var commitsCount = $"{CommitsSinceLastRelease}";
        var segmentIdsString = string.Join("-", _segments.Select(x => x.Id));
        return
            $"Path {Id,-3} {segmentIdsString,-50}  {commitsCount,5}     {_apiChanges.Flags}    {LastReleasedVersion?.ToString() ?? " none",8} -> {GetNextReleaseVersion(),-8}";
    }

    public VersionHistoryPath With(VersionHistorySegment segment)
    {
        var segmentIds = new List<VersionHistorySegment>(_segments) { segment };
        return new VersionHistoryPath(segmentIds.ToArray());
    }

    private ApiChanges AggregateBumps()
    {
        var bumps = new ApiChanges();
        foreach (var segment in _segments)
        {
            bumps.Aggregate(segment.ApiChanges);
            //bumps.BreakingChange |= segmentBumps.Flags.BreakingChange;
            //bumps.FunctionalityChange |= segmentBumps.Flags.FunctionalityChange;
            //bumps.Fix |= segmentBumps.Flags.Fix;
        }

        return bumps;
    }

    private SemVersion GetNextReleaseVersion()
    {
        _apiChanges = AggregateBumps();

        if (HeadCommit.HasReleaseTag)
        {
            return HeadCommit.ReleasedVersion!;
        }

        var startingVersion = LastReleasedVersion ?? new SemVersion(0, 1, 0);

        if (_apiChanges.Flags.BreakingChange)
        {
            return new SemVersion(startingVersion.Major + 1, 0, 0);
        }

        if (_apiChanges.Flags.FunctionalityChange)
        {
            return new SemVersion(startingVersion.Major, startingVersion.Minor + 1, 0);
        }

        return LastReleasedVersion == null
            ? startingVersion
            : new SemVersion(startingVersion.Major, startingVersion.Minor, startingVersion.Patch + 1);
    }
}