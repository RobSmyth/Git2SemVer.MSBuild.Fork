namespace NoeticTools.Git2SemVer.Tool.MSBuild.Projects;

internal interface IProjectDocumentReader
{
    ProjectDocument Read(FileInfo file);
}