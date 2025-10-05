using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Core.IntegrationTests;

[TestFixture]
[NonParallelizable]
public class GitToolIntegrationTests
{
    private ConsoleLogger _logger;
    private GitTool _target;

    [Test]
    public void CanInvokeGitTest()
    {
        Assert.That(_target.BranchName, Is.Not.Empty);
    }

    [Test]
    public void GetCommitsFromShaTest()
    {
        var commit = GetCommitAtIndex(_target, 5);

        var commits = _target.GetReachableFrom(commit.CommitId.Sha);

        Assert.That(commits, Has.Count.LessThan(301));
        Assert.That(commits, Has.Count.GreaterThan(100));
        Assert.That(commits[0], Is.SameAs(commit));
    }

    [SetUp]
    public void SetUp()
    {
        _logger = new ConsoleLogger();
        _target = new GitTool(new TagParser(), new ConventionalCommitsParser(new ConventionalCommitsSettings()));
    }

    [TearDown]
    public void TearDown()
    {
        _target.Dispose();
        _logger.Dispose();
    }

    private static Commit GetCommitAtIndex(GitTool target, int index)
    {
        var commit = target.Head;
        for (var count = 0; count < index; count++)
        {
            commit = target.Get(commit.Parents[0]);
        }

        return commit;
    }
}