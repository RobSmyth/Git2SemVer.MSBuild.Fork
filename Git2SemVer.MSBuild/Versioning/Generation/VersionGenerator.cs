using System.Diagnostics;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;

internal sealed class VersionGenerator : IVersionGenerator
{
    private readonly IDefaultVersionBuilderFactory _defaultVersionBuilderFactory;
    private readonly IGeneratedOutputsJsonFile _generatedOutputsJsonFile;
    private readonly IGitHistoryPathsFinder _gitPathsFinder;
    private readonly IGitTool _gitTool;
    private readonly IBuildHost _host;
    private readonly IVersionGeneratorInputs _inputs;
    private readonly ILogger _logger;
    private readonly IVersionBuilder _scriptBuilder;

    public VersionGenerator(IVersionGeneratorInputs inputs,
                            IBuildHost host,
                            IGeneratedOutputsJsonFile generatedOutputsJsonFile,
                            IGitTool gitTool,
                            IGitHistoryPathsFinder gitPathsFinder,
                            IDefaultVersionBuilderFactory defaultVersionBuilderFactory,
                            IVersionBuilder scriptBuilder,
                            ILogger logger)
    {
        _inputs = inputs;
        _host = host;
        _generatedOutputsJsonFile = generatedOutputsJsonFile;
        _gitTool = gitTool;
        _gitPathsFinder = gitPathsFinder;
        _defaultVersionBuilderFactory = defaultVersionBuilderFactory;
        _scriptBuilder = scriptBuilder;
        _logger = logger;
    }

    public IVersionOutputs Run()
    {
        _logger.LogTrace("Generating new versioning.");
        var stopwatch = Stopwatch.StartNew();

        _host.BumpBuildNumber();
        var historyPaths = _gitPathsFinder.FindPathsToHead();
        var outputs = new VersionOutputs(new GitOutputs(_gitTool, historyPaths));

        RunBuilders(outputs, historyPaths);


        SaveGeneratedVersions(outputs);

        stopwatch.Stop();
        _host.ReportBuildStatistic("Git2SemVerRunTime_sec", stopwatch.Elapsed.TotalSeconds);
        _logger.LogInfo($"Git2SemVer generated version: {outputs.InformationalVersion}  ({stopwatch.Elapsed.TotalSeconds:F1} sec))");
        return outputs;
    }

    private void RunBuilders(VersionOutputs outputs, HistoryPaths historyPaths)
    {
        _defaultVersionBuilderFactory.Create(historyPaths).Build(_host, _gitTool, _inputs, outputs);
        _scriptBuilder.Build(_host, _gitTool, _inputs, outputs);
    }

    private void SaveGeneratedVersions(VersionOutputs outputs)
    {
        _generatedOutputsJsonFile.Write(_inputs.IntermediateOutputDirectory, outputs);
        if (_inputs.VersioningMode != VersioningMode.StandAloneProject)
        {
            _generatedOutputsJsonFile.Write(_inputs.SolutionSharedDirectory, outputs);
        }
    }
}