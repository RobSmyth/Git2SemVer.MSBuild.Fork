using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.MSBuild.Tasking;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders.Scripting;

public sealed class ScriptVersionBuilder : IVersionBuilder
{
    private readonly ILogger _logger;

    public ScriptVersionBuilder(ILogger logger)
    {
        _logger = logger;
    }

    public void Build(IBuildHost host, IGitTool gitTool, IVersionGeneratorInputs inputs, IVersionOutputs outputs)
    {
        if (inputs == null)
        {
            throw new ArgumentException("Build requires non-null inputs.", nameof(inputs));
        }

        if (inputs.RunScript is false)
        {
            _logger.LogDebug("User C# script is disabled. Not run.");
            return;
        }

        var scriptRunner = new Git2SemVerScriptRunner(new CSharpScriptRunner(_logger),
                                                      host,
                                                      gitTool,
                                                      inputs,
                                                      outputs,
                                                      _logger);
        // ReSharper disable once UnusedVariable
        var task = scriptRunner.RunScript();
    }
}