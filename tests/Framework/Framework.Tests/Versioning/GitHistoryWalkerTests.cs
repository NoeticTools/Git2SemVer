using Moq;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Tests.TestScenarios;
using NoeticTools.Git2SemVer.Framework.Versioning.GitHistoryWalking;
using NoeticTools.Git2SemVer.Testing.Core;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Framework.Tests.Versioning;

[TestFixture]
internal class GitHistoryWalkerTests
{
    private Dictionary<string, Commit> _commitsLookup;
    private Mock<IGitTool> _gitTool;
    private NUnitLogger _logger;
    private Mock<ICommitsCache> _repository;

    [TestCaseSource(typeof(ManufacturedGitRepositoriesTestSource))]
    public void FindPathsToHead(string name, GitTestRepository scenario)
    {
        _logger.LogInfo(scenario.Description + "\n");
        LoadRepository(scenario.Commits, scenario.HeadCommitId);
        var target = new GitHistoryWalker(_gitTool.Object, _logger);

        var result = target.CalculateSemanticVersion();

        Assert.That(result.Version.ToString(), Is.EqualTo(scenario.ExpectedVersion));
    }

    [SetUp]
    public void SetUp()
    {
        _logger = new NUnitLogger(false)
        {
            Level = LoggingLevel.Debug
        };
        _commitsLookup = new Dictionary<string, Commit>();
        _repository = new Mock<ICommitsCache>();
        _gitTool = new Mock<IGitTool>();
        _gitTool.Setup(x => x.Get(It.IsAny<CommitId>()))
                .Returns((CommitId commitId) => _commitsLookup[commitId.Sha]);
        _gitTool.Setup(x => x.BranchName)
                .Returns("master");
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }

    private void LoadRepository(IEnumerable<Commit> commits, string headCommitId)
    {
        foreach (var commit in commits)
        {
            _commitsLookup.Add(commit.CommitId.Sha, commit);
        }

        _repository.Setup(x => x.Get(It.IsAny<CommitId>())).Returns<CommitId>(commitId => _commitsLookup[commitId.Sha]);
        _gitTool.Setup(x => x.Head).Returns(_commitsLookup[headCommitId]);
    }
}