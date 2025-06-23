using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Generation.Builders;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.Framework.Persistence;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Generation;

internal sealed class VersionGenerator : IVersionGenerator
{
    private readonly IDefaultVersionBuilderFactory _defaultVersionBuilderFactory;
    private readonly IOutputsJsonIO _generatedOutputsJsonFile;
    private readonly IGitTool _gitTool;
    private readonly IGitHistoryWalker _gitWalker;
    private readonly IBuildHost _host;
    private readonly IVersionGeneratorInputs _inputs;
    private readonly ILogger _logger;
    private readonly IMSBuildGlobalProperties _msBuildGlobalProperties;
    private readonly IVersionBuilder _scriptBuilder;

    public VersionGenerator(IVersionGeneratorInputs inputs,
                            IBuildHost host,
                            IOutputsJsonIO generatedOutputsJsonFile,
                            IGitTool gitTool,
                            IGitHistoryWalker gitWalker,
                            IDefaultVersionBuilderFactory defaultVersionBuilderFactory,
                            IVersionBuilder scriptBuilder,
                            IMSBuildGlobalProperties msBuildGlobalProperties,
                            ILogger logger)
    {
        _inputs = inputs;
        _host = host;
        _generatedOutputsJsonFile = generatedOutputsJsonFile;
        _gitTool = gitTool;
        _gitWalker = gitWalker;
        _defaultVersionBuilderFactory = defaultVersionBuilderFactory;
        _scriptBuilder = scriptBuilder;
        _msBuildGlobalProperties = msBuildGlobalProperties;
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

        var result = _gitWalker.CalculateSemanticVersion();
        var outputs = new VersionOutputs(new GitOutputs(_gitTool,
                                                        result.PriorReleaseVersion,
                                                        result.PriorReleaseCommitId));
        RunBuilders(outputs, result.Version);
        SaveGeneratedVersions(outputs);

        stopwatch.Stop();

        _logger.LogInfo($"Informational version: {outputs.InformationalVersion}");
        _logger.LogDebug($"Version generation completed (in {stopwatch.Elapsed.TotalSeconds:F1} seconds).");

        _host.ReportBuildStatistic("git2semver.runtime.seconds", stopwatch.Elapsed.TotalSeconds);

        return outputs;
    }

    private void RunBuilders(VersionOutputs outputs, SemVersion version)
    {
        _logger.LogDebug("Running version builders.");
        using (_logger.EnterLogScope())
        {
            var stopwatch = Stopwatch.StartNew();

            var defaultBuilder = _defaultVersionBuilderFactory.Create(version);
            defaultBuilder.Build(_host, _gitTool, _inputs, outputs, _msBuildGlobalProperties);

            _scriptBuilder.Build(_host, _gitTool, _inputs, outputs, _msBuildGlobalProperties);

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