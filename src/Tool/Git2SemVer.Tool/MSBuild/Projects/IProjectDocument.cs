using NoeticTools.Git2SemVer.Tool.MSBuild.Projects.GroupElements;


namespace NoeticTools.Git2SemVer.Tool.MSBuild.Projects;

public interface IProjectDocument
{
    PropertyGroup Properties { get; }

    void Save();
}