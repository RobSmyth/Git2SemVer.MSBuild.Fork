using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Framework.Config;


namespace NoeticTools.Git2SemVer.Framework.Tools.CI;

internal sealed class BuildHostFactory
{
    private readonly IConfiguration _config;
    private readonly Action<string> _buildOutput;
    private readonly ILogger _logger;

    public BuildHostFactory(IConfiguration config, Action<string> buildOutput, ILogger logger)
    {
        _config = config;
        _buildOutput = buildOutput;
        _logger = logger;
    }

    public IBuildHost Create(string hostType, string buildNumber, string buildContext, string inputsBuildIdFormat)
    {
        var host = new BuildHost(new BuildHostFinder(_config, _buildOutput, _logger).Find(hostType), _logger);

        if (!string.IsNullOrWhiteSpace(buildNumber))
        {
            host.BuildNumber = buildNumber;
        }

        if (!string.IsNullOrWhiteSpace(buildContext))
        {
            host.BuildContext = buildContext;
        }

        if (!string.IsNullOrWhiteSpace(inputsBuildIdFormat))
        {
            host.BuildIdFormat = inputsBuildIdFormat;
        }

        return host;
    }
}