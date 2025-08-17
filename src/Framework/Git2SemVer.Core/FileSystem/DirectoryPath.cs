using NoeticTools.Git2SemVer.Core.Exceptions;
using System.Diagnostics;


namespace NoeticTools.Git2SemVer.Core.FileSystem;

public sealed class DirectoryPath(string path)
{
    public const char PreferredDirectoryDelimiter = '/';
    public const char AlternativeDirectoryDelimiter = '\\';
    private readonly string _path = path.Replace(AlternativeDirectoryDelimiter, PreferredDirectoryDelimiter).TrimEnd(PreferredDirectoryDelimiter);

    public bool IsAbsolute => _path.Length > 0 && Path.IsPathRooted(_path);

    /// <summary>
    /// Gets a value indicating whether the path is empty.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsEmptyPath => _path.Length == 0;

    public bool Exists()
    {
        return IsEmptyPath || Directory.Exists(_path);
    }

    public void Create()
    {
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
        if (right.Length == 1 && right[0] is PreferredDirectoryDelimiter or AlternativeDirectoryDelimiter)
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

    public void Delete(bool recursive)
    {
        if (!Exists())
        {
            return;
        }
        Directory.Delete(_path, recursive);
        WaitUntil(() => !Exists());
    }

    private static bool WaitUntil(Func<bool> predicate)
    {
        var stopwatch = Stopwatch.StartNew();
        while (!predicate())
        {
            if (stopwatch.Elapsed > TimeSpan.FromSeconds(30))
            {
                return false;
            }

            Thread.Sleep(5);
        }

        return true;
    }
}