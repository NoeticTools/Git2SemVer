using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Framework.Tools.CI;


namespace NoeticTools.Git2SemVer.Framework.Framework.Config;

/// <summary>
///     User's local (host) Git2SemVer settings.
/// </summary>
public sealed class Git2SemVerLocalSettings : ILocalSettings
{
    private static readonly Mutex FileMutex = new(false, "G2SemVerConfigFileMutex");

    [JsonIgnore]
    private int _onLoadHash;

    /// <summary>
    ///     The local build log size.
    ///     If zero, the local build log is cleared and disabled.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Default is 0 (disabled).
    ///     </para>
    /// </remarks>
    [JsonPropertyOrder(95)]
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
    [JsonPropertyOrder(10)]
    public int BuildNumber { get; set; } = 1;

    /// <summary>
    ///     This configuration's schema version.
    /// </summary>
    [JsonPropertyOrder(1)]
    public int Rev { get; set; } = 1;

    /// <summary>
    ///     Load the configuration. May return cached configuration.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Loads the user's Git2SemVer configuration file.
    ///         If the file does not exist it is created.
    ///     </para>
    /// </remarks>
    public static ILocalSettings Load()
    {
        Git2SemVerLocalSettings instance;

        var filePath = GetFilePath();

        FileMutex.WaitOne(TimeSpan.FromSeconds(10));
        try
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                instance = Load(json);
            }
            else
            {
                instance = new Git2SemVerLocalSettings();
            }
        }
        finally
        {
            FileMutex.ReleaseMutex();
        }

        instance._onLoadHash = instance.GetCurrentHashCode();
        return instance;
    }

    public static Git2SemVerLocalSettings Load(string json)
    {
        return JsonSerializer.Deserialize<Git2SemVerLocalSettings>(json, JsonConstants.SerialiseOptions)!;
    }

    /// <summary>
    ///     Save configuration to file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Saves the user's Git2SemVer configuration file.
    ///     </para>
    /// </remarks>
    public void Save()
    {
        var currentHashCode = GetCurrentHashCode();
        if (_onLoadHash == currentHashCode)
        {
            return;
        }

        _onLoadHash = currentHashCode;

        var json = JsonSerializer.Serialize(this, JsonConstants.SerialiseOptions);
        json = Regex.Unescape(json);

        FileMutex.WaitOne(TimeSpan.FromSeconds(10));
        try
        {
            File.WriteAllText(GetFilePath(), json);
        }
        finally
        {
            FileMutex.ReleaseMutex();
        }
    }

    private int GetCurrentHashCode()
    {
        return HashCode.Combine(BuildLogSizeLimit, BuildNumber, Rev);
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