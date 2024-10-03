using Microsoft.Extensions.DependencyInjection;
using NoeticTools.Git2SemVer.Tool.Commands;
using NoeticTools.Git2SemVer.Tool.Framework;
using Spectre.Console.Cli;


// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine;

internal class AddCliCommand : Command<CommonCommandSettings>
{
    public override int Execute(CommandContext context, CommonCommandSettings settings)
    {
        var serviceProvider = (IServiceProvider)context.Data!;
        var console = serviceProvider.GetService<IConsoleIO>()!;
        var commandFactory = serviceProvider.GetService<ICommandFactory>()!;

        console.Unattended = settings.Unattended;
        var runner = commandFactory.CreateAddCommand();
        runner.Execute(settings.SolutionName, settings.Unattended);
        return (int)(runner.HasError ? ReturnCodes.CommandError : ReturnCodes.Succeeded);
    }
}