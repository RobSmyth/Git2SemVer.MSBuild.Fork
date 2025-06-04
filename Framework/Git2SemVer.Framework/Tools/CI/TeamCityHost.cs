using System.Globalization;
using JetBrains.TeamCity.ServiceMessages.Write.Special;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.CI;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;


namespace NoeticTools.Git2SemVer.Framework.Tools.CI;

internal class TeamCityHost : BuildHostBase, IDetectableBuildHost
{
    private readonly ILogger _logger;
    private readonly TeamCityHostSettings _settings;

    public TeamCityHost(ILogger logger) : base(logger)
    {
        _settings = new TeamCityHostSettings();
        _logger = logger;
        Name = "TeamCity";
        var teamCityVersion = _settings.Version;
        if (teamCityVersion.Length > 0)
        {
            BuildNumber = _settings.BuildNumber;
        }

        BuildContext = "0";
        DefaultBuildNumberFunc = () => [BuildNumber];
    }

    public HostTypeIds HostTypeId => HostTypeIds.TeamCity;

    public bool MatchesHostSignature()
    {
        return _settings.IsHost();
    }

    public override void ReportBuildStatistic(string key, int value)
    {
        _logger.LogInfo($"Build statistic {key} = {value}");
        using var writer = new TeamCityServiceMessages().CreateWriter(_logger.LogInfo);
        writer.WriteBuildStatistics(key, value.ToString(CultureInfo.InvariantCulture));
    }

    public override void ReportBuildStatistic(string key, double value)
    {
        _logger.LogInfo($"Build statistic {key} = {value:G13}");
        using var writer = new TeamCityServiceMessages().CreateWriter(_logger.LogInfo);
        writer.WriteBuildStatistics(key, $"{value:G13}");
    }

    public override void SetBuildLabel(string label)
    {
        _logger.LogInfo($"Setting TeamCity Build label to '{label}'.");
        using var writer = new TeamCityServiceMessages().CreateWriter(_logger.LogInfo);
        writer.WriteBuildNumber(label);
    }
}