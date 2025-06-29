using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Semver;


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
