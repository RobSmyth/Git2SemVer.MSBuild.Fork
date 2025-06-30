using System.ComponentModel;
using Spectre.Console.Cli;


// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine;

public class ChangelogCommandSettings : CommonCommandSettings
{
    [CommandOption("--host-type <TYPE>")]
    [Description("Force the host type. Use for testing expected behaviour on other hosts. Valid values are 'Custom', 'Uncontrolled', 'TeamCity', or 'GitHub'.")]
    public string? HostType { get; set; } = null;

    [CommandOption("-m|--meta-directory <DIRECTORY>")]
    [DefaultValue("")]
    [Description("Directory in which to place the generators metadata file.")]
    public string OutputDirectory { get; set; } = "";

    [CommandOption("-o|--output <FILEPATH>")]
    [DefaultValue("CHANGELOG.md")]
    [Description("Generated changelog file path. May be a relative or absolute path. Set to empty string to disable file write.")]
    public string OutputFilePath { get; set; } = "";

    [CommandOption("-v|--verbosity <LEVEL>")]
    [DefaultValue("info")]
    [Description("Sets output verbosity. Valid values are 'trace', 'debug', 'info', 'warning', or 'error'.")]
    public string Verbosity { get; set; } = "";

    [CommandOption("-a|--artifact-url <URL>")]
    [DefaultValue("")]
    [Description("Optional url to a verions's artifacts. Must contain version placeholder '%VERSION%'. e.g: https://www.nuget.org/packages/user.project/%VERSION%")]
    public string ArtifactUrl { get; set; } = "";

    [CommandOption("-c|--console-out")]
    [DefaultValue(true)]
    [Description("Enable writing generated changelog to the console.")]
    public bool WriteToConsole { get; set; }
}