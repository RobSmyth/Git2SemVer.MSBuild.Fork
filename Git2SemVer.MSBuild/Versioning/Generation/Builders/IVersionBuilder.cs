using NoeticTools.Git2SemVer.MSBuild.Scripting;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders;

/// <summary>
///     An output builder that sets or updates any of the task's MSBuild output properties.
/// </summary>
public interface IVersionBuilder
{
    void Build(VersioningContext context);
}