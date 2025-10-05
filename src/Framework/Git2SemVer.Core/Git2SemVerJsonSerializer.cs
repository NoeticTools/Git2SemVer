using System.Text.Json;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


namespace NoeticTools.Git2SemVer.Core;

public static class Git2SemVerJsonSerializer
{
    private static readonly Mutex FileMutex = new(false, "G2SemVerJsonFileMutex");

    public static T Deserialise<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, JsonConstants.SerialiseOptions)!;
    }

    public static T Read<T>(File file) where T : new()
    {
        FileMutex.WaitOne(JsonConstants.ReadTimeLimit);
        try
        {
            if (file.Exists())
            {
                var json = file.ReadAllText();
                return JsonSerializer.Deserialize<T>(json, JsonConstants.SerialiseOptions)!;
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

    public static string Serialise(object target)
    {
        return JsonSerializer.Serialize(target, JsonConstants.SerialiseOptions);
    }

    public static void Write(File file, object target)
    {
        var json = Serialise(target);

        FileMutex.WaitOne(JsonConstants.WriteTimeLimit);
        try
        {
            file.WriteAllText(json, createDirectory: true);
        }
        finally
        {
            FileMutex.ReleaseMutex();
        }
    }
}