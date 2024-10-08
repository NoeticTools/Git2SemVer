using Moq;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework;
using NoeticTools.Git2SemVer.MSBuild.Tests.TestScenarios;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Testing.Common;


namespace NoeticTools.Git2SemVer.MSBuild.Tests.Versioning.Generation;

[TestFixture]
internal class PathsFromLastReleasesFinderTests
{
    private Dictionary<string, Commit> _commitsLookup;
    private Mock<ICommitsRepository> _commitsRepo;
    private Mock<IGitTool> _gitTool;
    private NUnitTaskLogger _logger;

    [SetUp]
    public void SetUp()
    {
        _logger = new NUnitTaskLogger(false)
        {
            Level = LoggingLevel.Debug
        };
        _commitsLookup = new Dictionary<string, Commit>();
        _commitsRepo = new Mock<ICommitsRepository>();
        _commitsRepo.Setup(x => x.Get(It.IsAny<CommitId>()))
                    .Returns((CommitId commitId) => _commitsLookup[commitId.Id]);
        _gitTool = new Mock<IGitTool>();
        _gitTool.Setup(x => x.BranchName)
                .Returns("master");
        _gitTool.Setup(x => x.GetCommits(0, It.IsAny<int>()))
                .Returns((int skip, int take) =>
                             _commitsLookup.Values
                                           .Skip(skip)
                                           .Take(take)
                                           .ToReadOnlyList());
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }

    [TestCaseSource(typeof(GitTestRepositories))]
    public void FindPathsToHead(string name, GitTestRepository scenario)
    {
        _logger.LogInfo(scenario.Description + "\n");
        LoadRepository(scenario.Commits, scenario.HeadCommitId);
        var target = new PathsFromLastReleasesFinder(_commitsRepo.Object,
                                                     _gitTool.Object,
                                                     _logger);

        var historyPaths = target.FindPathsToHead();

        Assert.That(historyPaths!.Paths.Count, Is.EqualTo(scenario.ExpectedPathCount));
        Assert.That(historyPaths.BestPath.Version.ToString(), Is.EqualTo(scenario.ExpectedVersion));
    }

    private void LoadRepository(IEnumerable<Commit> commits, string headCommitId)
    {
        foreach (var commit in commits)
        {
            _commitsLookup.Add(commit.CommitId.Id, commit);
        }

        _commitsRepo.Setup(x => x.Head).Returns(_commitsLookup[headCommitId]);
    }
}