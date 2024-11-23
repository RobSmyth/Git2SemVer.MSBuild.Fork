using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Tools.CI;
using NoeticTools.MSBuild.Tasking;
using NuGet.Versioning;
using Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders.Scripting;

public sealed class Git2SemVerScriptRunner
{
    private readonly IBuildHost _host;
    private readonly MSBuildScriptRunner _innerScriptRunner;
    private readonly IVersionGeneratorInputs _inputs;
    private readonly ILogger _logger;
    private readonly IVersionOutputs _outputs;
    private readonly IGitTool _gitTool;

    public Git2SemVerScriptRunner(MSBuildScriptRunner innerScriptRunner,
                                  IBuildHost host,
                                  IGitTool gitTool,
                                  IVersionGeneratorInputs inputs,
                                  IVersionOutputs outputs,
                                  ILogger logger)
    {
        _inputs = inputs;
        _outputs = outputs;
        _innerScriptRunner = innerScriptRunner;
        _host = host;
        _gitTool = gitTool;
        _logger = logger;
    }

    internal static IReadOnlyList<Type> MetadataReferences { get; } =
    [
        typeof(DotNetTool),
        typeof(UncontrolledHost),
        typeof(IBuildHost),
        typeof(GitTool),
        typeof(SemVersion),
        typeof(NuGetVersion),
        typeof(VersioningContext),
        typeof(ILogger)
    ];

    internal async Task RunScript()
    {
        if (_inputs.RunScript == false)
        {
            _logger.LogDebug("RunScript option is not false. Script not run.");
            return;
        }

        if (!File.Exists(_inputs.BuildScriptPath))
        {
            if (_inputs.RunScript == null)
            {
                _logger.LogWarning($"RunScript is null and script '{_inputs.BuildScriptPath}' not found. Ignoring.");
                return;
            }

            if (_inputs.RunScript == true)
            {
                _logger.LogError($"RunScript is true and C# script '{_inputs.BuildScriptPath}' not found.");
                return;
            }
        }

        if (!_inputs.ValidateScriptInputs(_logger))
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        var inMemoryTypes = new List<Type>(
        [
            typeof(VersioningContext),
            typeof(ILogger),
            typeof(SemVersion)
        ]);

        var context = new VersioningContext(_inputs, _outputs, _host, _gitTool, _logger);
        await _innerScriptRunner.RunScript(context, _inputs.BuildScriptPath, MetadataReferences, inMemoryTypes).ConfigureAwait(true);

        stopwatch.Stop();
        _logger.LogInfo($"Script run completed. (in {stopwatch.Elapsed.TotalSeconds:F1} sec)");
        _host.ReportBuildStatistic("Git2SemVer_Script_Seconds", stopwatch.Elapsed.TotalSeconds);
    }
}