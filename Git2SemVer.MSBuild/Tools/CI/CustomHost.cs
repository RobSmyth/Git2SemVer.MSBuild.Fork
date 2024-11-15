using NoeticTools.Common.Logging;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;


namespace NoeticTools.Git2SemVer.MSBuild.Tools.CI;

internal class CustomHost : BuildHostBase, IDetectableBuildHost
{
    private readonly ILogger _logger;

    public CustomHost(ILogger logger) : base(logger)
    {
        _logger = logger;
        Name = "Custom";
        BuildNumber = "UNKNOWN";
        BuildContext = "UNKNOWN";
        DefaultBuildNumberFunc = () => [BuildContext, BuildNumber];
    }

    public HostTypeIds HostTypeId => HostTypeIds.Custom;

    public bool MatchesHostSignature()
    {
        // Not detectable
        return false;
    }
}