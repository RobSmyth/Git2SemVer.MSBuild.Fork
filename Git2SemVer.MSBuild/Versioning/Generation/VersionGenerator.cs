using System.Diagnostics;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;

internal sealed class VersionGenerator(
    IVersionGeneratorInputs inputs,
    IBuildHost host,
    IGeneratedOutputsJsonFile generatedOutputsJsonFile,
    IGitTool gitTool,
    IGitHistoryPathsFinder gitPathsFinder,
    IDefaultVersionBuilderFactory defaultVersionBuilderFactory,
    IVersionBuilder scriptBuilder,
    ILogger logger)
    : IVersionGenerator
{
    public IVersionOutputs Run()
    {
        logger.LogTrace("Generating new versioning.");
        var stopwatch = Stopwatch.StartNew();

        host.BumpBuildNumber();
        var historyPaths = gitPathsFinder.FindPathsToHead();
        var outputs = new VersionOutputs(new GitOutputs(gitTool, historyPaths));

        RunBuilders(outputs, historyPaths);
        SaveGeneratedVersions(outputs);

        stopwatch.Stop();
        host.ReportBuildStatistic("Git2SemVerRunTime_sec", stopwatch.Elapsed.TotalSeconds);
        logger.LogInfo($"Git2SemVer generated version: {outputs.InformationalVersion}  ({stopwatch.Elapsed.TotalSeconds:F1} sec))");
        return outputs;
    }

    private void RunBuilders(VersionOutputs outputs, HistoryPaths historyPaths)
    {
        defaultVersionBuilderFactory.Create(historyPaths).Build(host, gitTool, inputs, outputs);
        scriptBuilder.Build(host, gitTool, inputs, outputs);
    }

    private void SaveGeneratedVersions(VersionOutputs outputs)
    {
        generatedOutputsJsonFile.Write(inputs.IntermediateOutputDirectory, outputs);
        if (inputs.VersioningMode != VersioningMode.StandAloneProject)
        {
            generatedOutputsJsonFile.Write(inputs.SolutionSharedDirectory, outputs);
        }
    }
}