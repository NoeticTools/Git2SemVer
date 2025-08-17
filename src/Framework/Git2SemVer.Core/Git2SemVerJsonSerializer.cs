using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using NoeticTools.Git2SemVer.Core.FileSystem;


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

    public static T Read<T>(FilePath filePath) where T : new()
    {
        FileMutex.WaitOne(TimeSpan.FromSeconds(10));
        try
        {
            if (filePath.Exists())
            {
                var json = filePath.ReadAllText();
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

    public static void Write(FilePath filePath, object target)
    {
        var json = Serialise(target);

        FileMutex.WaitOne(TimeSpan.FromSeconds(10));
        try
        {
            filePath.WriteAllText(json, true);
        }
        finally
        {
            FileMutex.ReleaseMutex();
        }
    }
}