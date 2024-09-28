using NoeticTools.Git2SemVer.MSBuild.Scripting;
using NoeticTools.MSBuild.Tasking;


namespace NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders;

public sealed class ScriptVersionBuilder : IVersionBuilder
{
    public void Build(VersioningContext context)
    {
        if (context.Inputs.RunScript == false)
        {
            return;
        }

        var scriptRunner = new Git2SemVerScriptRunner(new MSBuildScriptRunner(context.Logger),
                                                      context,
                                                      context.Logger);
        var task = scriptRunner!.RunScript();
    }
}