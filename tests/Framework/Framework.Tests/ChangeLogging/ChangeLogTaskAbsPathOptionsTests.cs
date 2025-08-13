using Moq;
using NoeticTools.Git2SemVer.Framework.ChangeLogging.Task;


namespace NoeticTools.Git2SemVer.Framework.Tests.ChangeLogging;

[TestFixture]
public class ChangeLogTaskAbsPathOptionsTests
{
    [Test]
    public void ExpandsNonRootedPathsTest()
    {
        var taskOptions = new Mock<IChangeLogGeneratorTaskOptions>();
        taskOptions.Setup(x => x.WorkingDirectory).Returns(@"c:\working_directory");
        taskOptions.Setup(x => x.ChangelogDataDirectory).Returns(".data_directory");
        taskOptions.Setup(x => x.ChangelogOutputFilePath).Returns(@"MyChangelog.md");
        taskOptions.Setup(x => x.ChangelogEnable).Returns(true);
        taskOptions.Setup(x => x.ChangelogReleaseAs).Returns("");

        var target = new ChangeLogTaskAbsPathOptions(taskOptions.Object);

        Assert.That(target.ChangelogDataDirectory, Is.EqualTo(@"c:\working_directory\.data_directory"));
        Assert.That(target.ChangelogOutputFilePath, Is.EqualTo(@"c:\working_directory\MyChangelog.md"));
        Assert.That(target.ChangelogEnable, Is.True);
        Assert.That(target.ChangelogReleaseAs, Is.Empty);
    }

    [Test]
    public void DoesNotExpandRootedPathsTest()
    {
        var taskOptions = new Mock<IChangeLogGeneratorTaskOptions>();
        taskOptions.Setup(x => x.WorkingDirectory).Returns(@"c:\working_directory");
        taskOptions.Setup(x => x.ChangelogDataDirectory).Returns(@"c:\my_directory\.data_directory");
        taskOptions.Setup(x => x.ChangelogOutputFilePath).Returns(@"c:\my_directory\output\MyChangelog.md");
        taskOptions.Setup(x => x.ChangelogEnable).Returns(true);
        taskOptions.Setup(x => x.ChangelogReleaseAs).Returns("");

        var target = new ChangeLogTaskAbsPathOptions(taskOptions.Object);

        Assert.That(target.ChangelogDataDirectory, Is.EqualTo(@"c:\my_directory\.data_directory"));
        Assert.That(target.ChangelogOutputFilePath, Is.EqualTo(@"c:\my_directory\output\MyChangelog.md"));
        Assert.That(target.ChangelogEnable, Is.True);
        Assert.That(target.ChangelogReleaseAs, Is.Empty);
    }
}