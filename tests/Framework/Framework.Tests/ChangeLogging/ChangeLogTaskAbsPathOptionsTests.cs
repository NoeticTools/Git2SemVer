using Moq;
using NoeticTools.Git2SemVer.Framework.ChangeLogging.Task;


namespace NoeticTools.Git2SemVer.Framework.Tests.ChangeLogging;

[TestFixture]
public class ChangeLogTaskAbsPathOptionsTests
{
    [Test]
    public void ExpandsNonRootedPathsTest()
    {
        var dirSeparator = Path.DirectorySeparatorChar;
        var taskOptions = new Mock<IChangeLogGeneratorTaskOptions>();
        taskOptions.Setup(x => x.WorkingDirectory).Returns(@$"{dirSeparator}working_directory");
        taskOptions.Setup(x => x.ChangelogDataDirectory).Returns(".data_directory");
        taskOptions.Setup(x => x.ChangelogOutputFilePath).Returns(@"MyChangelog.md");
        taskOptions.Setup(x => x.ChangelogEnable).Returns(true);
        taskOptions.Setup(x => x.ChangelogReleaseAs).Returns("");

        var target = new ChangeLogTaskAbsPathOptions(taskOptions.Object);

        Assert.That(target.ChangelogDataDirectory, Is.EqualTo($@"{dirSeparator}working_directory{dirSeparator}.data_directory"));
        Assert.That(target.ChangelogOutputFilePath, Is.EqualTo($@"{dirSeparator}working_directory{dirSeparator}MyChangelog.md"));
        Assert.That(target.ChangelogEnable, Is.True);
        Assert.That(target.ChangelogReleaseAs, Is.Empty);
    }

    [Test]
    public void DoesNotExpandRootedPathsTest()
    {
        var dirSeparator = Path.DirectorySeparatorChar;
        var taskOptions = new Mock<IChangeLogGeneratorTaskOptions>();
        taskOptions.Setup(x => x.WorkingDirectory).Returns($@"{dirSeparator}working_directory");
        taskOptions.Setup(x => x.ChangelogDataDirectory).Returns($@"{dirSeparator}my_directory/.data_directory");
        taskOptions.Setup(x => x.ChangelogOutputFilePath).Returns($@"{dirSeparator}my_directory/output/MyChangelog.md");
        taskOptions.Setup(x => x.ChangelogEnable).Returns(true);
        taskOptions.Setup(x => x.ChangelogReleaseAs).Returns("");

        var target = new ChangeLogTaskAbsPathOptions(taskOptions.Object);

        Assert.That(target.ChangelogDataDirectory, Is.EqualTo($@"{dirSeparator}my_directory/.data_directory"));
        Assert.That(target.ChangelogOutputFilePath, Is.EqualTo($@"{dirSeparator}my_directory/output/MyChangelog.md"));
        Assert.That(target.ChangelogEnable, Is.True);
        Assert.That(target.ChangelogReleaseAs, Is.Empty);
    }
}