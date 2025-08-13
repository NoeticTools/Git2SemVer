using NoeticTools.Git2SemVer.Core.FileSystem;


namespace NoeticTools.Git2SemVer.Core.Tests.FileSystem;

[TestFixture]
public class DirectoryPathTests
{
    [TestCase("..", false)]
    [TestCase("../", false)]
    [TestCase(".data_directory", false)]
    [TestCase("/dir", true)]
    [TestCase("/test/../.dir", true)]
    [TestCase("/.data_directory/", true)]
    public void IsAbsoluteTest(string path, bool expected)
    {
        var directoryPath = new DirectoryPath(path);
        Assert.That(directoryPath.IsAbsolute, Is.EqualTo(expected));
    }

    [Test]
    public void ToAbsolute_DoesNotExpandRootedDefaultPathsTest()
    {
        var directoryPath = new DirectoryPath("");

        var result = directoryPath.ToAbsolute("/default_path/dir", "/working_directory");

        Assert.That(result.ToString(), Is.EqualTo("/default_path/dir/"));
    }

    [TestCase("/dir")]
    [TestCase("/test/../dir/")]
    [TestCase("/.data_directory/../dir")]
    public void ToAbsolute_DoesNotExpandRootedPathsTest(string path)
    {
        var directoryPath = new DirectoryPath(path);

        var result = directoryPath.ToAbsolute("default_path/dir", "/working_directory");

        Assert.That(path.TrimEnd('/') + '/', Is.EqualTo(result.ToString()));
    }

    [Test]
    public void ToAbsolute_ExpandsNonRootedDefaultPathTest()
    {
        var directoryPath = new DirectoryPath("");

        var result = directoryPath.ToAbsolute("default_path/dir", "/working_directory");

        Assert.That(result.ToString(), Is.EqualTo("/working_directory/default_path/dir/"));
    }

    [TestCase("dir", "/working_directory/dir/")]
    [TestCase("../dir", "/working_directory/../dir/")]
    [TestCase(".data_directory/dir/", "/working_directory/.data_directory/dir/")]
    public void ToAbsolute_ExpandsNonRootedPathsTest(string path, string expected)
    {
        var directoryPath = new DirectoryPath(path);

        var result = directoryPath.ToAbsolute("default_path/dir", "/working_directory");

        Assert.That(result.ToString(), Is.EqualTo(expected));
    }
}