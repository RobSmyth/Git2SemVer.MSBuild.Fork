using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using NoeticTools.Git2SemVer.MSBuild.Tools.CI;


namespace NoeticTools.Git2SemVer.MSBuild.Framework.Config;

/// <summary>
///     User's local Git2SemVer configuration.
/// </summary>
internal class Git2SemVerConfiguration : IConfiguration
{
    [JsonIgnore]
    private static Git2SemVerConfiguration? _instance;

    [JsonIgnore]
    private static string _filePath = "";

    public List<Git2SemVerBuildLogEntry> BuildLog { get; set; } = [];

    /// <summary>
    ///     The local build log size.
    ///     If zero, the local build log is cleared and disabled.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is 0 (disabled).
    ///     </para>
    /// </remarks>
    public int BuildLogSizeLimit { get; set; }

    /// <summary>
    ///     The next local build number. Default is 1.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <b>Not recommended for use on build system (controlled) build hosts.</b>
    ///     </para>
    ///     <para>
    ///         Used by <a cref="UncontrolledHost">UncontrolledHost.BuildNumber</a>/
    ///     </para>
    /// </remarks>
    public int BuildNumber { get; set; } = 1;

    /// <summary>
    ///     <see href="https://www.conventionalcommits.org/en/v1.0.0/">Conventional commit</see> regular expression pattern
    ///     to detect fix (patch), feature (minor), or breaking changes (major).
    /// </summary>
    public string ConventionalCommitsPattern { get; set; } =
        @"^((?<fix>fix)|(?<feature>feat(ure)?)|(?<breakingChange>BREAKING CHANGE))(?<break>!)?:";

    /// <summary>
    ///     Path the configuration file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default file path is:
    ///     </para>
    ///     <code>
    ///         &lt;SpecialFolder.LocalApplicationData&gt;/Git2SemVer/Configuration.json
    ///     </code>
    /// </remarks>
    [JsonIgnore]
    public static string FilePath
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                _filePath = GetFilePath();
            }

            return _filePath;
        }
        set => _filePath = value;
    }

    /// <summary>
    ///     This configuration's schema version.
    /// </summary>
    public string Version { get; set; } = "1.0.0";

    public Git2SemVerBuildLogEntry AddLogEntry(string buildNumber, bool hasLocalChanges, string branch, string lastCommitId, string path)
    {
        if (BuildLog.Count >= BuildLogSizeLimit)
        {
            BuildLog.RemoveRange(BuildLogSizeLimit, BuildLog.Count - BuildLogSizeLimit);
        }

        var entry = new Git2SemVerBuildLogEntry(buildNumber, hasLocalChanges, branch, lastCommitId, path);
        if (BuildLogSizeLimit > 0)
        {
            BuildLog.Add(entry);
        }

        return entry;
    }

    /// <summary>
    ///     Load the configuration. May return cached configuration.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Loads the user's Git2SemVer configuration file.
    ///         If the file does not exist it is created.
    ///     </para>
    ///     <para>
    ///         See <a cref="FilePath">FilePath</a> for details of the file's path.
    ///     </para>
    /// </remarks>
    public static Git2SemVerConfiguration Load()
    {
        if (_instance != null)
        {
            return _instance;
        }

        var filePath = FilePath;
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _instance = JsonSerializer.Deserialize<Git2SemVerConfiguration>(json);
        }
        else
        {
            _instance = new Git2SemVerConfiguration();
        }

        return _instance!;
    }

    /// <summary>
    ///     Save configuration to file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Saves the user's Git2SemVer configuration file.
    ///     </para>
    ///     <para>
    ///         See <a cref="FilePath">FilePath</a> for details of the file's path.
    ///     </para>
    /// </remarks>
    public void Save()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        var json = JsonSerializer.Serialize(this, options);
        json = Regex.Unescape(json);
        File.WriteAllText(FilePath, json);
    }

    private static string GetFilePath()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Git2SemVer");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return Path.Combine(folderPath, "Configuration.json");
    }
}