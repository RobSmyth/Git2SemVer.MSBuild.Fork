using System.Diagnostics;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.DotnetCli;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Tools.CI;
using NoeticTools.MSBuild.Tasking;
using NuGet.Versioning;
using Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Scripting;

public sealed class Git2SemVerScriptRunner
{
    private readonly MSBuildScriptRunner _innerScriptRunner;
    private readonly ILogger _logger;
    private readonly VersioningContext _scriptContext;

    public Git2SemVerScriptRunner(MSBuildScriptRunner innerScriptRunner,
                                  VersioningContext scriptContext,
                                  ILogger logger)
    {
        _scriptContext = scriptContext;
        _innerScriptRunner = innerScriptRunner;
        _logger = logger;
    }

    internal async Task RunScript()
    {
        var inputs = _scriptContext.Inputs;

        if (inputs.RunScript == false)
        {
            _logger.LogDebug("RunScript option is not false. Script not run.");
            return;
        }

        if (inputs.RunScript == null && !File.Exists(_scriptContext.Inputs.BuildScriptPath))
        {
            _logger.LogDebug($"Script not found. BuildScriptPath is '{_scriptContext.Inputs.BuildScriptPath}'.");
            return;
        }

        if (!inputs.Validate(_logger))
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        var metadataReferences = new List<Type>(new[]
        {
            typeof(DotNetTool),
            typeof(UncontrolledHost),
            typeof(IBuildHost),
            typeof(GitTool),
            typeof(SemVersion),
            typeof(NuGetVersion),
            typeof(VersioningContext),
            typeof(ILogger)
        });

        var inMemoryTypes = new List<Type>(new[]
        {
            typeof(VersioningContext),
            typeof(ILogger),
            typeof(SemVersion)
        });

        await _innerScriptRunner.RunScript(_scriptContext, inputs.BuildScriptPath, metadataReferences, inMemoryTypes);

        stopwatch.Stop();
        _logger.LogInfo($"Script run completed. (in {stopwatch.Elapsed.TotalSeconds:F1} sec)");
        _scriptContext.Host.ReportBuildStatistic("Git2SemVer_Script_Seconds", stopwatch.Elapsed.TotalSeconds);
    }
}