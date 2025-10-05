using System.Text.Json;
using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Framework.Versioning;
using NoeticTools.Git2SemVer.Framework.Versioning.Builders;


namespace NoeticTools.Git2SemVer.Framework.Persistence;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class VersioningOutputsJsonFileIO : IOutputsJsonIO
{
    // ReSharper disable once MemberCanBePrivate.Global
    public static VersionOutputs FromJson(string json)
    {
        return JsonSerializer.Deserialize<VersioningInfo>(json, JsonConstants.SerialiseOptions)!.Git2SemVerVersionInfo!;
    }

    public IVersionOutputs Read(string directory)
    {
        return ReadFromFile(directory);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static IVersionOutputs ReadFromFile(string directory)
    {
        var propertiesFilePath = GetFilePath(directory);
        if (!File.Exists(propertiesFilePath))
        {
            return new VersionOutputs();
        }

        var json = File.ReadAllText(propertiesFilePath);
        return FromJson(json);
    }

    public static string ToJson(IVersionOutputs outputs)
    {
        Git2SemVerArgumentException.ThrowIfNull(outputs, nameof(outputs));

        var versionInfo = new VersioningInfo { Git2SemVerVersionInfo = (VersionOutputs)outputs };
        return JsonSerializer.Serialize(versionInfo, JsonConstants.SerialiseOptions);
    }

    public void Write(string directory, IVersionOutputs outputs)
    {
        Git2SemVerArgumentException.ThrowIfNullOrEmpty(directory, nameof(directory));
        Git2SemVerArgumentException.ThrowIfNull(outputs, nameof(outputs));
        Git2SemVerArgumentException.ThrowIf(outputs, nameof(outputs), o => o is not VersionOutputs);

        if (directory.Length > 0 && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = ToJson(outputs);
        var existingJson = LoadJson(directory);
        if (json.Equals(existingJson, StringComparison.Ordinal))
        {
            return;
        }

        File.WriteAllText(GetFilePath(directory), json);
    }

    private static string GetFilePath(string directory)
    {
        return Path.Combine(directory, VersioningConstants.SharedVersionJsonPropertiesFilename);
    }

    private static string LoadJson(string directory)
    {
        var propertiesFilePath = GetFilePath(directory);
        return !File.Exists(propertiesFilePath) ? "" : File.ReadAllText(propertiesFilePath);
    }

    private sealed class VersioningInfo
    {
        [JsonPropertyOrder(2)]
        public VersionOutputs? Git2SemVerVersionInfo { get; set; }

        /// <summary>
        ///     This version info's schema version.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <strong>Rev 2</strong>
        ///         <list type="bullet">
        ///             <item>Generated JSON - CommitId `Id` property renamed to `Sha`</item>
        ///             <item>Generated JSON - CommitChangeTypeId.None added, type numbers bumped.</item>
        ///         </list>
        ///     </para>
        /// </remarks>
        [JsonPropertyOrder(1)]
        // ReSharper disable once PropertyCanBeMadeInitOnly.Local
        public int Rev { get; set; } = 3;
    }
}