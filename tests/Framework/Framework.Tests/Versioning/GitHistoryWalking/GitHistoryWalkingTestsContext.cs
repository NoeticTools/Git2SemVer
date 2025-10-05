using Moq;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Testing.Core;


namespace NoeticTools.Git2SemVer.Framework.Tests.Versioning.GitHistoryWalking;

internal class GitHistoryWalkingTestsContext : IDisposable
{
    private readonly GitResponseParser _logParser;

    public void Dispose()
    {
        Logger.Dispose();
    }

    public void SetupGitRepository(LoggedScenario scenario)
    {
        var commits = GetCommits(scenario.ActualGitLog).ToDictionary(k => k.CommitId.Sha, v => v);
        GitTool.Setup(x => x.Get(It.IsAny<CommitId>())).Returns<CommitId>(id => commits[id.Sha]);
        GitTool.Setup(x => x.Head).Returns(commits[scenario.HeadCommitId]);
    }

    private List<Commit> GetCommits(string gitLog)
    {
        var lines = gitLog.Split('\n');
        return lines.Select(line => _logParser.ParseGitLogLine(line)).OfType<Commit>().ToList();
    }

    public GitHistoryWalkingTestsContext()
    {
        Logger = new NUnitLogger(false) { Level = LoggingLevel.Trace };
        _logParser = new GitResponseParser(new CommitsCache(), new ConventionalCommitsParser(new ConventionalCommitsSettings()));
        GitTool = new Mock<IGitTool>();
        GitTool.Setup(x => x.BranchName).Returns("BranchName");
    }

    public Mock<IGitTool> GitTool { get; }

    public NUnitLogger Logger { get; }
}