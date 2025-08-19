using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using NoeticTools.Git2SemVer.Core.FileSystem;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


namespace NoeticTools.Git2SemVer.Core;

public static class Git2SemVerJsonSerializer
{
    private static readonly Mutex FileMutex = new(false, "G2SemVerJsonFileMutex");

    private static readonly JsonSerializerOptions SerialiseOptions = new()
    {
        WriteIndented = true,
        IgnoreReadOnlyFields = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    public static T Read<T>(File file) where T : new()
    {
        FileMutex.WaitOne(TimeSpan.FromSeconds(10));
        try
        {
            if (file.Exists())
            {
                var json = file.ReadAllText();
                return JsonSerializer.Deserialize<T>(json)!;
            }
            else
            {
                return new T();
            }
        }
        finally
        {
            FileMutex.ReleaseMutex();
        }
    }

    public static T Deserialise<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }

    public static string Serialise(object target)
    {
        return JsonSerializer.Serialize(target, SerialiseOptions);
    }

    public static void Write(File file, object target)
    {
        var json = Serialise(target);

        FileMutex.WaitOne(TimeSpan.FromSeconds(10));
        try
        {
            file.WriteAllText(json, true);
        }
        finally
        {
            FileMutex.ReleaseMutex();
        }
    }
}