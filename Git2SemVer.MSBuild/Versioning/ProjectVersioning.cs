using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Persistence;


#pragma warning disable CA1859

namespace NoeticTools.Git2SemVer.MSBuild.Versioning;

internal sealed class ProjectVersioning(
    IVersionGeneratorInputs inputs,
    IBuildHost host,
    IGeneratedOutputsJsonFile outputsCacheJsonFile,
    IVersionGenerator versionGenerator,
    ILogger logger)
{
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

            var outputs = handlers[inputs.VersioningMode]();
            UpdateHostBuildLabel(outputs);
            return outputs;
        }
        catch (Exception exception)
        {
            logger.LogError(exception);
            throw;
        }
    }

    private string GetClientLastBuildNumber()
    {
        var localCache = outputsCacheJsonFile.Load(inputs.IntermediateOutputDirectory);
        if (localCache.IsValid)
        {
            return localCache.BuildNumber;
        }

        var shared = outputsCacheJsonFile.Load(inputs.SolutionSharedDirectory);
        return !shared.IsValid ? host.BuildNumber : shared.BuildNumber;
    }

    private IVersionOutputs PerformSolutionClientVersioning()
    {
        logger.LogTrace("Solution versioning client project.");

        var lastBuildNumber = GetClientLastBuildNumber();
        if (lastBuildNumber == host.BuildNumber)
        {
            return versionGenerator.Run();
        }

        var output = outputsCacheJsonFile.Load(inputs.SolutionSharedDirectory);
        outputsCacheJsonFile.Write(inputs.IntermediateOutputDirectory, output);
        return output;
    }

    private IVersionOutputs PerformSolutionVersioningProjectVersioning()
    {
        logger.LogTrace("Solution versioning project.");
        var output = outputsCacheJsonFile.Load(inputs.SolutionSharedDirectory);
        return !output.IsValid ? versionGenerator.Run() : output;
    }

    private IVersionOutputs PerformStandAloneProjectVersioning()
    {
        logger.LogTrace("Stand alone project versioning.");
        return versionGenerator.Run();
    }

    private void UpdateHostBuildLabel(IVersionOutputs output)
    {
        if (inputs.UpdateHostBuildLabel && output.BuildSystemVersion != null)
        {
            host.SetBuildLabel(output.BuildSystemVersion.ToString());
        }
    }
}