using Injectio.Attributes;
using NoeticTools.Git2SemVer.Tool.CommandLine;


namespace NoeticTools.Git2SemVer.Tool;

[RegisterSingleton]
internal class Program
{
    public static int Main(string[] args)
    {
        return new Git2SemVerCommandApp().Execute(args);
    }
}