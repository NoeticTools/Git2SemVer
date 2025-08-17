using NoeticTools.Git2SemVer.Core.FileSystem;


namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public abstract class JsonSettingsFileBase<T>
    where T : new()
{
    public static T Load(DirectoryPath dataDirectory, FilePath filename)
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

    private static T Load(FilePath filePath)
    {
        return Git2SemVerJsonSerializer.Read<T>(filePath);
    }

    private void Save(DirectoryPath dataDirectory, FilePath filename)
    {
        var filePath = dataDirectory + filename;
        Git2SemVerJsonSerializer.Write(filePath, this);
    }

    public string ToJson()
    {
        return Git2SemVerJsonSerializer.Serialise(this);
    }
}