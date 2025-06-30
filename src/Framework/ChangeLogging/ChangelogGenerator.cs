using System.Text;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public sealed class ChangelogGenerator
{
    public void GenerateChangelog(IVersionOutputs versioning, ContributingCommits contributing)
    {
        // WIP
        var stringBuilder = new StringBuilder();
        using var writer = new StringWriter(stringBuilder);
        writer.WriteLine();

        ChangelogWriter.Write(writer, versioning, contributing);

        writer.WriteLine();
        Console.WriteLine(stringBuilder.ToString()); // >>> temp
    }
}