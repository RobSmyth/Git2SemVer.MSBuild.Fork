using System.Collections.Immutable;
using System.Text;
using NoeticTools.Git2SemVer.Core.Diagnostics;


namespace NoeticTools.DiagnosticCodesDocBuilder.DocFx;

internal sealed class DiagCodesContentBuilder
{
    private readonly string _docsPath;

    public DiagCodesContentBuilder(string docsPath)
    {
        _docsPath = docsPath;
    }

    public void Build(IReadOnlyList<DiagnosticCodeBase> diagCodes)
    {
        CreateContentFiles(diagCodes);
        CreateTocFile(diagCodes);
    }

    private void CreateContentFiles(IReadOnlyList<DiagnosticCodeBase> diagCodes)
    {
        foreach (var diagCode in diagCodes)
        {
            var fileName = $"{diagCode.Code}.md";
            var filePath = Path.Combine(_docsPath, DocFolders.ErrorsAndWarnings, fileName);
            File.WriteAllText(filePath, $"""
                                         ---
                                         uid: {diagCode.Code}
                                         ---

                                         <div style="background-color:#944248;padding:0px;margin-bottom:0.5em">
                                           <img src="https://noetictools.github.io/Git2SemVer.MSBuild/Images/Git2SemVer_banner_840x70.png"/>
                                         </div>
                                         <br/>

                                         # {diagCode.Code} diagnostic code

                                         ## Message text

                                         ``{diagCode.Message}``

                                         ## Description

                                         {diagCode.Description}

                                         ## Resolution

                                         {diagCode.Resolution}
                                         """);
            Console.WriteLine($"  Created file: {Path.Combine(DocFolders.ErrorsAndWarnings, fileName)}");
        }
    }

    private void CreateTocFile(IReadOnlyList<DiagnosticCodeBase> diagCodes)
    {
        var stringBuilder = new StringBuilder();
        foreach (var diagCode in diagCodes.OrderBy(x => x.Code))
        {
            stringBuilder.AppendLine($"- name: {diagCode.Code}");
            stringBuilder.AppendLine($"  href: {diagCode.Code}.md");
        }

        var filePath = Path.Combine(_docsPath, DocFolders.ErrorsAndWarnings, "toc.yml");
        File.WriteAllText(filePath, stringBuilder.ToString());
        Console.WriteLine($"  Created file: {Path.Combine(DocFolders.ErrorsAndWarnings, "toc.yml")}");
    }
}