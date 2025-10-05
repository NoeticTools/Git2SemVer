using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.Exceptions;


namespace NoeticTools.Git2SemVer.Core.FileSystem;

[JsonConverter(typeof(FileJsonConverter))]
public sealed class File(string path) : IEquatable<File>
{
    private readonly string _path = Normalise(path);

    /// <summary>
    ///     The filename component of the path.
    /// </summary>
    public string FileName => Path.GetFileName(_path)
                              ?? throw new Git2SemVerArgumentException($"The '{_path}' path does not have a file name component.");

    public bool IsAbsolute => _path.Length > 0 && Path.IsPathRooted(_path);

    /// <summary>
    ///     True if the path is empty, i.e. it has no components.
    /// </summary>
    public bool IsEmptyPath => _path.Length == 0;

    public static File Null => new(string.Empty);

    public bool Equals(File? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return _path == other._path;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is File other && Equals(other));
    }

    public bool Exists()
    {
        return System.IO.File.Exists(_path);
    }

    /// <summary>
    ///     Get the directory component of the file path.
    /// </summary>
    public Directory GetDirectory()
    {
        return Path.GetDirectoryName(_path)
               ?? throw new Git2SemVerArgumentException($"The '{_path}' path does not have a directory component.");
    }

    public override int GetHashCode()
    {
        return _path.GetHashCode();
    }

    public static Directory operator +(File left, char right)
    {
        Git2SemVerArgumentException.ThrowIfNull(left, $"The {nameof(left)} argument must not be null.");

        var leftPathString = left.ToString();

        if (right is not (Directory.PreferredDirectoryDelimiter or Directory.AlternativeDirectoryDelimiter))
        {
            throw new Git2SemVerArgumentException($"The '{right}' argument must be '\\' or '/' character.");
        }

        return new Directory(leftPathString);
    }

    public static implicit operator File(string path)
    {
        Git2SemVerArgumentException.ThrowIfNull(path, $"The {nameof(path)} argument must be a non-empty string.");

        return new File(path);
    }

    public static implicit operator string(File file)
    {
        return file?.ToString() ?? string.Empty;
    }

    /// <summary>
    ///     Reads all text from the file at the specified path.
    /// </summary>
    /// <returns>A string containing the entire contents of the file.</returns>
    public string ReadAllText()
    {
        return System.IO.File.ReadAllText(_path);
    }

    /// <summary>
    ///     Converts the current relative file path to an absolute file path.
    /// </summary>
    /// <param name="defaultPath">The default file path to use if the current path is empty.</param>
    /// <param name="workingDirectory">The working directory to use as the base for resolving relative paths.</param>
    /// <returns>
    ///     An absolute <see cref="File" />. If the current path is empty, the <paramref name="defaultPath" /> is used. If
    ///     the current path is already absolute, it is returned as-is.
    /// </returns>
    public File ToAbsolute(File defaultPath, Directory workingDirectory)
    {
        var filePath = _path.Length > 0 ? new File(_path) : defaultPath;
        if (filePath.IsAbsolute)
        {
            return filePath;
        }

        return workingDirectory + new File(filePath);
    }

    public override string ToString()
    {
        return _path;
    }

    /// <summary>
    ///     Writes the specified content to a file at the configured path, optionally creating the directory if it does not
    ///     exist.
    /// </summary>
    /// <remarks>
    ///     If <paramref name="createDirectory" /> is <see langword="true" /> and the directory does not
    ///     exist,  it will be created before writing the file. The file is overwritten if it already exists.
    /// </remarks>
    /// <param name="content">The content to write to the file. Cannot be <see langword="null" />.</param>
    /// <param name="createDirectory">
    ///     A value indicating whether to create the directory if it does not exist.  <see langword="true" /> to create the
    ///     directory; otherwise, <see langword="false" />.
    /// </param>
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

        System.IO.File.WriteAllText(_path, content);
    }

    private static string Normalise(string path)
    {
        Git2SemVerArgumentException.ThrowIfNull(path, $"The {nameof(path)} argument must not be null or empty.");

        path = path.Replace(Directory.AlternativeDirectoryDelimiter, Directory.PreferredDirectoryDelimiter);
        if (path.LastOrDefault() == Directory.PreferredDirectoryDelimiter)
        {
            throw new Git2SemVerArgumentException($"The '{path}' argument must not end with a directory delimiter.");
        }

        return path;
    }
}