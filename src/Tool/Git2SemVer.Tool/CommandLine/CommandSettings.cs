using System.ComponentModel;
using Spectre.Console.Cli;


// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool.CommandLine;

public class CommonCommandSettings : CommandSettings
{
    [CommandOption("-u|--unattended")]
    [DefaultValue(false)]
    [Description("Unattened execution. Accepts all defaults.")]
    public bool Unattended { get; set; }
}

public class SolutionCommandSettings : CommonCommandSettings
{
    [CommandOption("-s|--solution")]
    [DefaultValue("")]
    [Description("Solution name. Optional, only required when there are multiple solutions in the working directory.")]
    public string SolutionName { get; set; } = "";
}

public class RunCommandSettings : CommonCommandSettings
{
    [CommandOption("--enable-json-write")]
    [DefaultValue(false)]
    [Description("Enables writing generated versions to file 'Git2SemVer.VersionInfo.g.json'.")]
    public bool EnableJsonFileWrite { get; set; }

    [CommandOption("--host-type <TYPE>")]
    [Description("Force the host type. Use for testing expected behaviour on other hosts. Valid values are 'Custom', 'Uncontrolled', 'TeamCity', or 'GitHub'.")]
    public string? HostType { get; set; } = null;

    [CommandOption("-o|--output <DIRECTORY>")]
    [DefaultValue("")]
    [Description("Directory in which to place the generated version JSON file and the build log.")]
    public string OutputDirectory { get; set; } = "";

    [CommandOption("-v|--verbosity <LEVEL>")]
    [DefaultValue("info")]
    [Description("Sets output verbosity. Valid values are 'trace', 'debug', 'info', 'warning', or 'error'.")]
    public string Verbosity { get; set; } = "";
}

public class ChangelogCommandSettings : CommonCommandSettings
{
    [CommandOption("--host-type <TYPE>")]
    [Description("Force the host type. Use for testing expected behaviour on other hosts. Valid values are 'Custom', 'Uncontrolled', 'TeamCity', or 'GitHub'.")]
    public string? HostType { get; set; } = null;

    [CommandOption("-o|--output <DIRECTORY>")]
    [DefaultValue("")]
    [Description("Directory in which to place the generated changelog file.")]
    public string OutputDirectory { get; set; } = "";

    [CommandOption("-v|--verbosity <LEVEL>")]
    [DefaultValue("info")]
    [Description("Sets output verbosity. Valid values are 'trace', 'debug', 'info', 'warning', or 'error'.")]
    public string Verbosity { get; set; } = "";
}