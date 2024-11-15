using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;


#pragma warning disable CA1859

namespace NoeticTools.Git2SemVer.MSBuild.Versioning;

internal sealed class ProjectVersioning
{
    private readonly IGeneratedOutputsJsonFile _generatedOutputsJsonFile;
    private readonly IBuildHost _host;
    private readonly IVersionGeneratorInputs _inputs;
    private readonly ILogger _logger;
    private readonly IVersionGenerator _versionGenerator;

    public ProjectVersioning(IVersionGeneratorInputs inputs,
                             IBuildHost host,
                             IGeneratedOutputsJsonFile generatedOutputsJsonFile,
                             IVersionGenerator versionGenerator,
                             ILogger logger)
    {
        _inputs = inputs;
        _host = host;
        _generatedOutputsJsonFile = generatedOutputsJsonFile;
        _versionGenerator = versionGenerator;
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

            var outputs = handlers[_inputs.VersioningMode]();
            UpdateHostBuildLabel(outputs);
            return outputs;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception);
            throw;
        }
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

    private IVersionOutputs PerformSolutionClientVersioning()
    {
        _logger.LogTrace("Solution versioning client project.");

        var lastBuildNumber = GetClientLastBuildNumber();
        if (lastBuildNumber == _host.BuildNumber)
        {
            return _versionGenerator.Run();
        }

        var output = _generatedOutputsJsonFile.Load(_inputs.SolutionSharedDirectory);
        _generatedOutputsJsonFile.Write(_inputs.IntermediateOutputDirectory, output);
        return output;
    }

    private IVersionOutputs PerformSolutionVersioningProjectVersioning()
    {
        _logger.LogTrace("Solution versioning project.");
        var output = _generatedOutputsJsonFile.Load(_inputs.SolutionSharedDirectory);
        return output.BuildNumber.Length == 0 ? _versionGenerator.Run() : output;
    }

    private IVersionOutputs PerformStandAloneProjectVersioning()
    {
        _logger.LogTrace("Stand alone project versioning.");
        return _versionGenerator.Run();
    }

    private void UpdateHostBuildLabel(IVersionOutputs output)
    {
        if (_inputs.UpdateHostBuildLabel && output.BuildSystemVersion != null)
        {
            _host.SetBuildLabel(output.BuildSystemVersion.ToString());
        }
    }
}