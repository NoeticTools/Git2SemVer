using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;


namespace NoeticTools.Git2SemVer.Testing.Core;

[NonParallelizable]
public abstract class ScriptingTestsBase : TestFixtureBase
{
    private const int MaximumTestDataFolders = 20;
    private static int _testDataFolderId; // avoid locks on folders not release quickly between tests

    // ReSharper disable once ChangeFieldTypeToSystemThreadingLock
    private static readonly object Sync = new();

    protected void SetUpBase()
    {
        Logger = new NUnitLogger { Level = LoggingLevel.Trace };

        var dataFolderId = GetNextId();

        TestFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                      "Git2SemVer",
                                      $"TestData{dataFolderId}");
        if (Directory.Exists(TestFolderPath))
        {
            Directory.Delete(TestFolderPath, true);
            if (!WaitUntil(() => !Directory.Exists(TestFolderPath)))
            {
                Assert.Fail($"Unable to deleted folder '{TestFolderPath}'.");
            }
        }

        Directory.CreateDirectory(TestFolderPath);
        if (!WaitUntil(() => Directory.Exists(TestFolderPath)))
        {
            Assert.Fail($"Unable to create folder '{TestFolderPath}'.");
        }
    }

    protected virtual void OneTimeSetUpBase()
    {
        Logger = new NUnitLogger(); // todo - Logger is set here and in the SetUpBase method
        DotNetCli = new DotNetTool(new ProcessCli(Logger));
        Git = new GitTool(new TagParser(), new ConventionalCommitsParser(new ConventionalCommitsSettings()));
    }

    protected void OneTimeTearDownBase()
    {
        Git.Dispose();
    }

    protected DotNetTool DotNetCli { get; private set; } = null!;

    protected GitTool Git { get; private set; } = null!;

    protected ILogger Logger { get; private set; } = null!;

    private static int GetNextId()
    {
        int dataFolderId;
        lock (Sync)
        {
            if (_testDataFolderId > MaximumTestDataFolders)
            {
                _testDataFolderId = 0;
            }

            dataFolderId = ++_testDataFolderId;
        }

        return dataFolderId;
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

    protected string TestFolderPath = "";
}