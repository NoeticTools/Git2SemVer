using NoeticTools.Git2SemVer.Core.Exceptions;


namespace NoeticTools.Git2SemVer.Core.FileSystem;

public sealed class FilePath(string path)
{
    private readonly string _path = Normalise(path);

    public bool IsAbsolute => _path.Length > 0 && Path.IsPathRooted(_path);

    public bool IsEmptyPath => _path.Length == 0;

    public string FileName => Path.GetFileName(_path)
        ?? throw new Git2SemVerArgumentException($"The '{_path}' path does not have a file name component.");

    public bool Exists()
    {
        return File.Exists(_path);
    }

    public static DirectoryPath operator +(FilePath left, char right)
    {
        Git2SemVerArgumentException.ThrowIfNull(left, $"The {nameof(left)} argument must not be null.");

        var leftPathString = left.ToString();

        if (right is not (DirectoryPath.PreferredDirectoryDelimiter or '\\'))
        {
            throw new Git2SemVerArgumentException($"The '{right}' argument must be '\\' or '/' character.");
        }

        return new DirectoryPath(leftPathString);
    }

    public static implicit operator FilePath(string path)
    {
        Git2SemVerArgumentException.ThrowIfNull(path, $"The {nameof(path)} argument must be a non-empty string.");

        return new FilePath(path);
    }

    public static implicit operator string(FilePath filePath)
    {
        return filePath?.ToString() ?? string.Empty;
    }

    public string ReadAllText()
    {
        return File.ReadAllText(_path);
    }

    public FilePath ToAbsolute(FilePath defaultPath, DirectoryPath workingDirectory)
    {
        var filePath = _path.Length > 0 ? new FilePath(_path) : defaultPath;
        if (filePath.IsAbsolute)
        {
            return filePath;
        }

        return workingDirectory + new FilePath(filePath);
    }

    public override string ToString()
    {
        return _path;
    }

    public DirectoryPath GetDirectory()
    {
        return Path.GetDirectoryName(_path)
            ?? throw new Git2SemVerArgumentException($"The '{_path}' path does not have a directory component.");
    }

    public void WriteAllText(string content, bool createDirectory = true)
    {
        Git2SemVerArgumentException.ThrowIfNull(content, $"The {nameof(content)} argument must not be null.");

        if (createDirectory)
        {
            var directory = GetDirectory();
            if (!directory.Exists())
            {
                directory.Create();
            }
        }

        File.WriteAllText(_path, content);
    }

    private static string Normalise(string path)
    {
        Git2SemVerArgumentException.ThrowIfNull(path, $"The {nameof(path)} argument must not be null or empty.");

        path = path.Replace('\\', DirectoryPath.PreferredDirectoryDelimiter);
        if (path.LastOrDefault() == DirectoryPath.PreferredDirectoryDelimiter)
        {
            throw new Git2SemVerArgumentException($"The '{path}' argument must not end with a directory delimiter.");
        }

        return path;
    }
}