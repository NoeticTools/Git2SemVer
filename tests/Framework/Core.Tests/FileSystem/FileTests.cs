using NoeticTools.Git2SemVer.Core.FileSystem;
using Directory = NoeticTools.Git2SemVer.Core.FileSystem.Directory;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


namespace NoeticTools.Git2SemVer.Core.Tests.FileSystem;

[TestFixture]
public class FileTests
{
    [TestCase(Directory.PreferredDirectoryDelimiter)]
    [TestCase('\\')]
    public void AddDirectoryDelimiterTest(char delimiter)
    {
        var filePath = new File("/test/dir");

        var result = filePath + delimiter;

        Assert.That(result, Is.TypeOf<Directory>());
        Assert.That(result.ToString(), Is.EqualTo("/test/dir/"));
    }

    [Test]
    public void AddStringReturnsStringTest()
    {
        // ReSharper disable once StringLiteralTypo
        var filePath = new File("/test/dir") + "ectory";

        Assert.That(filePath, Is.TypeOf<string>());
        Assert.That(filePath, Is.EqualTo("/test/directory"));
    }

    [Test]
    public void FilenameTest()
    {
        var filePath = new File("/test/dir/file.txt");

        var result = filePath.FileName;

        Assert.That(result, Is.EqualTo("file.txt"));
    }

    [TestCase("file.txt", false)]
    [TestCase("../file.txt", false)]
    [TestCase(".data_directory/file.txt", false)]
    [TestCase("/file.txt", true)]
    [TestCase("/test/../file.txt", true)]
    [TestCase("/.data_directory/file.txt", true)]
    public void IsAbsoluteTest(string path, bool expected)
    {
        var filePath = new File(path);
        Assert.That(filePath.IsAbsolute, Is.EqualTo(expected));
    }

    [TestCase("", true)]
    [TestCase("a", false)]
    [TestCase("/a", false)]
    public void IsEmptyTest(string input, bool expected)
    {
        var filePath = new File(input);

        var result = filePath.IsEmptyPath;

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ToAbsolute_DoesNotExpandRootedDefaultPathsTest()
    {
        var filePath = new File("");

        var result = filePath.ToAbsolute("/default_path/file", "/working_directory");

        Assert.That(result.ToString(), Is.EqualTo("/default_path/file"));
    }

    [TestCase("/file.txt")]
    [TestCase("/test/../file.txt")]
    [TestCase("/.data_directory/file.txt")]
    public void ToAbsolute_DoesNotExpandRootedPathsTest(string path)
    {
        var filePath = new File(path);

        var result = filePath.ToAbsolute("default_path/file", "/working_directory");

        Assert.That(result.ToString(), Is.EqualTo(path));
    }

    [Test]
    public void ToAbsolute_ExpandsNonRootedDefaultPathTest()
    {
        var filePath = new File("");

        var result = filePath.ToAbsolute("default_path/file", "/working_directory");

        Assert.That(result.ToString(), Is.EqualTo("/working_directory/default_path/file"));
    }

    [TestCase("file.txt", "/working_directory/file.txt")]
    [TestCase("../file.txt", "/working_directory/../file.txt")]
    [TestCase(".data_directory/file.txt", "/working_directory/.data_directory/file.txt")]
    public void ToAbsolute_ExpandsNonRootedPathsTest(string path, string expected)
    {
        var filePath = new File(path);

        var result = filePath.ToAbsolute("default_path/file", "/working_directory");

        Assert.That(result.ToString(), Is.EqualTo(expected));
    }
}