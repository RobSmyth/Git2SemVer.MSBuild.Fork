﻿using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.Builders;
using NoeticTools.Git2SemVer.Framework.Generation.Builders.Scripting;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Tools.CI;


namespace NoeticTools.Git2SemVer.Framework;

[ExcludeFromCodeCoverage]
public sealed class ProjectVersioningFactory
{
    private readonly Action<string> _buildOutput;
    private readonly ILogger _logger;

    public ProjectVersioningFactory(Action<string> buildOutput, ILogger logger)
    {
        _buildOutput = buildOutput;
        _logger = logger;
    }

    public ProjectVersioning Create(IVersionGeneratorInputs inputs, IMSBuildGlobalProperties msBuildGlobalProperties,
                                    IOutputsJsonIO? outputsJsonIO = null, IConfiguration? config = null)
    {
        if (inputs == null)
        {
            throw new ArgumentNullException(nameof(inputs), "Inputs is required.");
        }

        outputsJsonIO ??= new OutputsJsonFileIO();
        config ??= Git2SemVerConfiguration.Load();

        var host = new BuildHostFactory(config, _buildOutput, _logger).Create(inputs.HostType,
                                                                              inputs.BuildNumber,
                                                                              inputs.BuildContext,
                                                                              inputs.BuildIdFormat);
        var gitTool = new GitTool(new TagParser(inputs.ReleaseTagFormat)) { RepositoryDirectory = inputs.WorkingDirectory };
        var gitPathsFinder = new GitHistoryWalker(gitTool, _logger);

        var defaultBuilderFactory = new DefaultVersionBuilderFactory(_logger);
        var scriptBuilder = new ScriptVersionBuilder(_logger);
        var versionGenerator = new VersionGenerator(inputs,
                                                    host,
                                                    outputsJsonIO,
                                                    gitTool,
                                                    gitPathsFinder,
                                                    defaultBuilderFactory,
                                                    scriptBuilder,
                                                    msBuildGlobalProperties,
                                                    _logger);
        var projectVersioning = new ProjectVersioning(inputs, host,
                                                      outputsJsonIO,
                                                      versionGenerator,
                                                      _logger);
        return projectVersioning;
    }
}