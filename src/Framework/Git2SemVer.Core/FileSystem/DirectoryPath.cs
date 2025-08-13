using NoeticTools.Git2SemVer.Core.Exceptions;


namespace NoeticTools.Git2SemVer.Core.FileSystem;

public sealed class DirectoryPath(string path)
{
    public const char PreferredDirectoryDelimiter = '/';
    private readonly string _path = path.Replace('\\', PreferredDirectoryDelimiter).TrimEnd(PreferredDirectoryDelimiter);

    public bool IsAbsolute => _path.Length > 0 && Path.IsPathRooted(_path);

    public bool IsEmpty => _path.Length == 0;

    public bool Exists()
    {
        return Directory.Exists(_path);
    }

    public void Create()
    {
        if (IsEmpty)
        {
            throw new Git2SemVerArgumentException($"The {nameof(DirectoryPath)} cannot be created because the path is empty.");
        }

        if (Exists())
        {
            return;
        }
        Directory.CreateDirectory(_path);
    }

    public static FilePath operator +(DirectoryPath left, FilePath right)
    {
        return right.IsAbsolute ? new FilePath(right.ToString()) : new FilePath(left.ToString() + right);
    }

    /// <summary>
    ///     Append subdirectory to the directory path.
    /// </summary>
    public static DirectoryPath operator +(DirectoryPath left, DirectoryPath right)
    {
        return right.IsAbsolute ? new DirectoryPath(right.ToString()) : new DirectoryPath(left.ToString() + right.ToString());
    }

    /// <summary>
    ///     Append subdirectory to the directory path.
    /// </summary>
    public static DirectoryPath operator +(DirectoryPath left, string right)
    {
        Git2SemVerArgumentException.ThrowIfNull(left, $"The {nameof(left)} argument must not be null.");
        var leftPathString = left.ToString();
        // ReSharper disable once MergeIntoPattern
        if (right.Length == 1 && right[0] is PreferredDirectoryDelimiter or '\\')
        {
            throw new Git2SemVerArgumentException($"The '{right}' argument must be a subdirectory.");
        }

        return new DirectoryPath(leftPathString);
    }

    public static implicit operator DirectoryPath(string path)
    {
        Git2SemVerArgumentException.ThrowIfNull(path, $"The {nameof(path)} argument must be a non-empty string.");
        return new DirectoryPath(path);
    }

    public static implicit operator string(DirectoryPath directoryPath)
    {
        return directoryPath?.ToString() ?? string.Empty;
    }

    public DirectoryPath ToAbsolute(DirectoryPath defaultPath, DirectoryPath workingDirectory)
    {
        var directoryPath = _path.Length > 0 ? new DirectoryPath(_path) : defaultPath;
        if (defaultPath.IsAbsolute)
        {
            return defaultPath;
        }

        return workingDirectory + directoryPath;
    }

    public override string ToString()
    {
        return _path + PreferredDirectoryDelimiter;
    }
}