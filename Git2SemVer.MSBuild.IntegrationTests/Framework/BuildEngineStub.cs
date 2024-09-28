using System.Collections;
using Microsoft.Build.Framework;


namespace NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;

internal class BuildEngineStub : IBuildEngine
{
    public int ColumnNumberOfTaskNode { get; } = 0;

    public bool ContinueOnError { get; } = false;

    public int LineNumberOfTaskNode { get; } = 0;

    public string ProjectFileOfTaskNode { get; } = "";

    public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
    {
        throw new NotImplementedException();
    }

    public void LogCustomEvent(CustomBuildEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void LogErrorEvent(BuildErrorEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void LogMessageEvent(BuildMessageEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void LogWarningEvent(BuildWarningEventArgs e)
    {
        throw new NotImplementedException();
    }
}