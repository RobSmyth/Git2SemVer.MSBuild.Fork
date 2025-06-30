using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;


// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public sealed class ChangelogConfiguration : IEquatable<ChangelogConfiguration>
{
    [JsonIgnore]
    private static readonly Mutex FileMutex = new(false, "G2SemVerChangelogConfigFileMutex");

    [JsonIgnore]
    private static readonly JsonSerializerOptions SerialiseOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    [JsonIgnore]
    private int _onLoadHash;

    /// <summary>
    ///     Categories to include in the changelog.
    /// </summary>
    public ChangelogCategory[] Categories { get; set; } = [];

    /// <summary>
    ///     Configuration file schema revision.
    /// </summary>
    public string Rev { get; set; } = "1.0.0";

    public bool Equals(ChangelogConfiguration? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Rev == other.Rev && Categories.Equals(other.Categories);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is ChangelogConfiguration other && Equals(other));
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        return HashCode.Combine(Rev, Categories);
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }

    /// <summary>
    ///     Load the configuration. May return cached configuration.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Loads the user's Git2SemVer configuration file.
    ///         If the file does not exist it is created.
    ///     </para>
    /// </remarks>
    public static ChangelogConfiguration Load(string filePath)
    {
        ChangelogConfiguration instance;

        FileMutex.WaitOne(TimeSpan.FromSeconds(10));
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                instance = JsonSerializer.Deserialize<ChangelogConfiguration>(json)!;
            }
            else
            {
                instance = new ChangelogConfiguration();
            }
        }
        finally
        {
            FileMutex.ReleaseMutex();
        }

        instance._onLoadHash = instance.GetHashCode();
        return instance;
    }

    /// <summary>
    ///     Save configuration to file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Saves the user's Git2SemVer configuration file.
    ///     </para>
    /// </remarks>
    public void Save(string filePath)
    {
        var currentHashCode = GetHashCode();
        if (_onLoadHash == currentHashCode)
        {
            return;
        }

        _onLoadHash = currentHashCode;

        var json = JsonSerializer.Serialize(this, SerialiseOptions);
        json = Regex.Unescape(json);

        FileMutex.WaitOne(TimeSpan.FromSeconds(10));
        try
        {
            File.WriteAllText(filePath, json);
        }
        finally
        {
            FileMutex.ReleaseMutex();
        }
    }
}