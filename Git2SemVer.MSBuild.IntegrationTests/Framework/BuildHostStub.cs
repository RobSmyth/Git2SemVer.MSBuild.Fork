using NoeticTools.Common.Logging;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;


namespace NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;

internal class BuildHostStub : IBuildHost
{
    public BuildHostStub(ILogger logger)
    {
        Logger = logger;
    }

    public string BuildContext { get; set; } = "";

    public IReadOnlyList<string> BuildId { get; } = [];

    public string BuildIdFormat { get; set; } = "";

    public string BuildNumber { get; set; } = "42";

    public HostTypeIds HostTypeId { get; set; } = HostTypeIds.Unknown;

    public bool IsControlled { get; set; } = false;

    public ILogger Logger { get; }

    public string Name { get; set; } = "Test Host";

    public string BumpBuildNumber()
    {
        return BuildNumber;
    }

    public void ReportBuildStatistic(string key, int value)
    {
        Logger.LogTrace($"Build statistic {key} = {value}");
    }

    public void ReportBuildStatistic(string key, double value)
    {
        Logger.LogTrace($"Build statistic {key} = {value:G13}");
    }

    public void SetBuildLabel(string label)
    {
        Logger.LogInfo($"SetBuildLabel: {label}");
    }
}