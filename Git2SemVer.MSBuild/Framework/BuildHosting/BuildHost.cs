namespace NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;

internal class BuildHost : IBuildHost
{
    private readonly IBuildHost _inner;

    internal BuildHost(IBuildHost inner)
    {
        _inner = inner;
    }

    public string BuildContext
    {
        get => _inner.BuildContext;
        set => _inner.BuildContext = value;
    }

    public IReadOnlyList<string> BuildId => _inner.BuildId;

    public string BuildIdFormat
    {
        get => _inner.BuildIdFormat;
        set => _inner.BuildIdFormat = value;
    }

    public string BuildNumber
    {
        get => _inner.BuildNumber;
        set => _inner.BuildNumber = value;
    }

    public HostTypeIds HostTypeId => _inner.HostTypeId;

    public string Name => _inner.Name;

    public string BumpBuildNumber()
    {
        return _inner.BumpBuildNumber();
    }

    public void ReportBuildStatistic(string key, int value)
    {
        _inner.ReportBuildStatistic(key, value);
    }

    public void ReportBuildStatistic(string key, double value)
    {
        _inner.ReportBuildStatistic(key, value);
    }

    public void SetBuildLabel(string label)
    {
        _inner.SetBuildLabel(label);
    }
}