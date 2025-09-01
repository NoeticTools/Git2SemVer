using System.Diagnostics;
using System.Net.NetworkInformation;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using NoeticTools.Git2SemVer.Testing.Core;


namespace NoeticTools.Git2SemVer.IntegrationTests.Framework;

[NonParallelizable]
internal abstract class ScriptingTestsBase
{
    private const int MaximumTestDataFolders = 20;
    private static int _testDataFolderId; // avoid locks on folders not release quickly between tests
    private static object _sync = new();

    protected string TestFolderPath = "";

    protected GitTool Git { get; private set; } = null!;

    protected ILogger Logger { get; private set; } = null!;

    protected void OneTimeSetUpBase()
    {
        Logger = new NUnitLogger(); // todo - Logger is set here and in the SetUpBase method
        Git = new GitTool(new TagParser(), new ConventionalCommitsParser(new ConventionalCommitsSettings()));
    }

    protected void OneTimeTearDownBase()
    {
        Git.Dispose();
    }

    protected void SetUpBase()
    {
        Logger = new NUnitLogger { Level = LoggingLevel.Trace };

        int dataFolderId;
        lock (_sync)
        {
            if (_testDataFolderId > MaximumTestDataFolders)
            {
                _testDataFolderId = 0;
            }
            dataFolderId = ++_testDataFolderId;
        }

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
}