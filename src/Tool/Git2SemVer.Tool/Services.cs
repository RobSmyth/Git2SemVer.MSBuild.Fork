using Microsoft.Extensions.DependencyInjection;
using NoeticTools.Git2SemVer.Core.Logging;


// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool;

[RegisterSingleton]
internal sealed class Services
{
    public static IServiceProvider ConfigureServices(ILogger logger)
    {
        var services = new ServiceCollection();

        services.AddSingleton(logger);

        services.AddNoeticToolsGit2SemVerTool();
        services.AddNoeticToolsGit2SemVerCore();
        services.AddNoeticToolsGit2SemVerFramework();

        return services.BuildServiceProvider();
    }
}