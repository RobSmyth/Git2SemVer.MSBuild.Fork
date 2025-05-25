using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Generation.Builders;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.Framework.Persistence;


namespace NoeticTools.Git2SemVer.Framework.Generation;

internal sealed class VersionGenerator : IVersionGenerator
{
    private readonly IDefaultVersionBuilderFactory _defaultVersionBuilderFactory;
    private readonly IOutputsJsonIO _generatedOutputsJsonFile;
    private readonly IGitHistoryPathsFinder _gitPathsFinder;
    private readonly IGitTool _gitTool;
    private readonly IBuildHost _host;
    private readonly IVersionGeneratorInputs _inputs;
    private readonly ILogger _logger;
    private readonly IVersionBuilder _scriptBuilder;

    public VersionGenerator(IVersionGeneratorInputs inputs,
                            IBuildHost host,
                            IOutputsJsonIO generatedOutputsJsonFile,
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

    public void Dispose()
    {
        _gitTool.Dispose();
    }

    public IVersionOutputs Run()
    {
        var stopwatch = Stopwatch.StartNew();

        _host.BumpBuildNumber();

        var historyPaths = _gitPathsFinder.FindPathsToHead();

        var outputs = new VersionOutputs(new GitOutputs(_gitTool, historyPaths));
        RunBuilders(outputs, historyPaths);
        SaveGeneratedVersions(outputs);

        stopwatch.Stop();

        _logger.LogInfo($"Informational version: {outputs.InformationalVersion}");
        _logger.LogDebug($"Version generation completed (in {stopwatch.Elapsed.TotalSeconds:F1} seconds).");

        _host.ReportBuildStatistic("Git2SemVerRunTime_sec", stopwatch.Elapsed.TotalSeconds);

        return outputs;
    }

    private void RunBuilders(VersionOutputs outputs, HistoryPaths historyPaths)
    {
        _logger.LogDebug("Running version builders.");
        using (_logger.EnterLogScope())
        {
            var stopwatch = Stopwatch.StartNew();

            _defaultVersionBuilderFactory.Create(historyPaths).Build(_host, _gitTool, _inputs, outputs);

            _scriptBuilder.Build(_host, _gitTool, _inputs, outputs);

            stopwatch.Stop();
            _logger.LogDebug($"Version building completed (in {stopwatch.Elapsed.TotalSeconds:F1} sec).");
        }
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