using NoeticTools.Git2SemVer.Core.Exceptions;
using System.Diagnostics;


namespace NoeticTools.Git2SemVer.Core.FileSystem;

public sealed class Directory(string path)
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
        return IsEmptyPath || System.IO.Directory.Exists(_path);
    }

    public void Create()
    {
        if (Exists())
        {
            return;
        }
        System.IO.Directory.CreateDirectory(_path);
    }

    public static File operator +(Directory left, File right)
    {
        return right.IsAbsolute ? new File(right.ToString()) : new File(left.ToString() + right);
    }

    /// <summary>
    ///     Append subdirectory to the directory path.
    /// </summary>
    public static Directory operator +(Directory left, Directory right)
    {
        return right.IsAbsolute ? new Directory(right.ToString()) : new Directory(left.ToString() + right.ToString());
    }

    /// <summary>
    ///     Append subdirectory to the directory path.
    /// </summary>
    public static Directory operator +(Directory left, string right)
    {
        Git2SemVerArgumentException.ThrowIfNull(left, $"The {nameof(left)} argument must not be null.");
        var leftPathString = left.ToString();
        // ReSharper disable once MergeIntoPattern
        if (right.Length == 1 && right[0] is PreferredDirectoryDelimiter or AlternativeDirectoryDelimiter)
        {
            throw new Git2SemVerArgumentException($"The '{right}' argument must be a subdirectory.");
        }

        return new Directory(leftPathString);
    }

    public static implicit operator Directory(string path)
    {
        Git2SemVerArgumentException.ThrowIfNull(path, $"The {nameof(path)} argument must be a non-empty string.");
        return new Directory(path);
    }

    public static implicit operator string(Directory directory)
    {
        return directory?.ToString() ?? string.Empty;
    }

    public Directory ToAbsolute(Directory defaultPath, Directory workingDirectory)
    {
        var directoryPath = _path.Length > 0 ? new Directory(_path) : defaultPath;
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
        System.IO.Directory.Delete(_path, recursive);
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

    public IReadOnlyList<File> GetFiles(string pattern, bool recursive)
    {
        if (!Exists())
        {
            return [];
        }
        var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        return System.IO.Directory.GetFiles(_path, pattern, searchOption).Select(x => new File(x)).ToList();
    }
}