using System.Xml.Linq;
using Injectio.Attributes;
using NoeticTools.Common.Logging;


namespace NoeticTools.Git2SemVer.Tool.MSBuild.Projects;

[RegisterSingleton]
internal sealed class ProjectDocumentReader : IProjectDocumentReader
{
    private readonly ILogger _logger;

    public ProjectDocumentReader(ILogger logger)
    {
        _logger = logger;
    }

    public ProjectDocument Read(FileInfo file)
    {
        var xml = XDocument.Load(file.FullName);
        return new ProjectDocument(xml, file);
    }
}