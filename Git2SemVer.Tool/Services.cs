using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using NoeticTools.Common.Logging;
// ReSharper disable ClassNeverInstantiated.Global


namespace NoeticTools.Git2SemVer.Tool;

[RegisterSingleton]
internal sealed class Services
{
    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var logger = new NullTaskLogger(); // todo - add logger
        services.AddSingleton<ILogger>(logger);

        services.AddNoeticToolsGit2SemVerTool();
        services.AddNoeticToolsCommon();

        return services.BuildServiceProvider();
    }
}