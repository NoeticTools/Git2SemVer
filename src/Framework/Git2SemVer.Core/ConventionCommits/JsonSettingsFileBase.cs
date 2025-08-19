using NoeticTools.Git2SemVer.Core.FileSystem;
using Directory = NoeticTools.Git2SemVer.Core.FileSystem.Directory;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public abstract class JsonSettingsFileBase<T>
    where T : new()
{
    public static T Load(Directory dataDirectory, File filename)
    {
        var filePath = dataDirectory + filename;
        if (filePath.Exists())
        {
            return Load(filePath);
        }

        var config = new T();
        (config as JsonSettingsFileBase<T>)?.Save(dataDirectory, filename);
        return config;
    }

    private static T Load(File file)
    {
        return Git2SemVerJsonSerializer.Read<T>(file);
    }

    private void Save(Directory dataDirectory, File filename)
    {
        var filePath = dataDirectory + filename;
        Git2SemVerJsonSerializer.Write(filePath, this);
    }

    public string ToJson()
    {
        return Git2SemVerJsonSerializer.Serialise(this);
    }
}