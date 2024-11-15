using System.Diagnostics;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;


#pragma warning disable CA1859

namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;

internal class VersionGenerator
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
        try
        {
            var handlers = new Dictionary<VersioningMode, Func<IVersionOutputs>>
            {
                { VersioningMode.SolutionVersioningProject, PerformSolutionVersioningProjectVersioning },
                { VersioningMode.SolutionClientProject, PerformSolutionClientVersioning },
                { VersioningMode.StandAloneProject, PerformStandAloneProjectVersioning }
            };

            return handlers[_inputs.VersioningMode]();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception);
            throw;
        }
    }

    private IVersionOutputs GenerateVersion()
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

    private IVersionOutputs PerformSolutionClientVersioning()
    {
        _logger.LogTrace("Solution client versioning.");

        var lastBuildNumber = GetClientLastBuildNumber();
        if (lastBuildNumber == _host.BuildNumber)
        {
            return GenerateVersion();
        }

        var output = _generatedOutputsJsonFile.Load(_inputs.SolutionSharedDirectory);
        WriteOutputsToFile(_inputs.IntermediateOutputDirectory, output);
        UpdateHostBuildLabel(output);
        return output;
    }

    private string GetClientLastBuildNumber()
    {
        var lastBuildNumber = _generatedOutputsJsonFile.Load(_inputs.IntermediateOutputDirectory).BuildNumber;
        if (lastBuildNumber.Length != 0)
        {
            return lastBuildNumber;
        }

        var shared = _generatedOutputsJsonFile.Load(_inputs.SolutionSharedDirectory);
        lastBuildNumber = shared.BuildNumber;
        if (lastBuildNumber.Length == 0)
        {
            lastBuildNumber = _host.BuildNumber;
        }
        return lastBuildNumber;
    }

    private IVersionOutputs PerformSolutionVersioningProjectVersioning()
    {
        _logger.LogTrace("Solution versioning project.");

        // do nothing but update build label - solution versioning project depreciated
        var output = _generatedOutputsJsonFile.Load(_inputs.IntermediateOutputDirectory);
        UpdateHostBuildLabel(output);
        return output;
    }

    private IVersionOutputs PerformStandAloneProjectVersioning()
    {
        _logger.LogTrace("Stand alone project versioning.");

        var output = GenerateVersion();
        UpdateHostBuildLabel(output);
        return output;
    }

    private void RunBuilders(VersionOutputs outputs, HistoryPaths historyPaths)
    {
        _defaultVersionBuilderFactory.Create(historyPaths).Build(_host, _gitTool, _inputs, outputs);
        _scriptBuilder.Build(_host, _gitTool, _inputs, outputs);
    }

    private void SaveGeneratedVersions(VersionOutputs outputs)
    {
        WriteOutputsToFile(_inputs.IntermediateOutputDirectory, outputs);
        if (_inputs.VersioningMode != VersioningMode.StandAloneProject)
        {
            WriteOutputsToFile(_inputs.SolutionSharedDirectory, outputs);
        }
    }

    private void UpdateHostBuildLabel(IVersionOutputs output)
    {
        if (_inputs.UpdateHostBuildLabel && output.BuildSystemVersion != null)
        {
            _host.SetBuildLabel(output.BuildSystemVersion.ToString());
        }
    }

    private void WriteOutputsToFile(string outputDirectory, VersionOutputs generatedOutputs)
    {
        _generatedOutputsJsonFile.Write(outputDirectory, generatedOutputs);
    }
}