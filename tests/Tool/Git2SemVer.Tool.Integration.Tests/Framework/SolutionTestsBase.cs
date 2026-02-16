using System.Diagnostics;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Testing.Core;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Tool.Integration.Tests.Framework;

internal abstract class SolutionTestsBase
{
    private const int MaximumTestDataFolders = 20;
    private string _currentDirectory = "";
    private string _git2SemVerToolPath = "";
    private string _solutionDirectory = "";
    private static int _testDataFolderId; // avoid locks on folders not release quickly between tests
    private string _testFolderPath = "";

    protected void OneTimeSetUpBase()
    {
        Logger = new NUnitLogger();
        DotNetCli = new DotNetTool(new ProcessCli(Logger));
        _currentDirectory = Directory.GetCurrentDirectory();
        _solutionDirectory = DotNetProcessHelpers.GetSolutionDirectory();
        TestSolutionDirectory = Path.Combine(_solutionDirectory, "tests", "Tool", "TestSolutions", SolutionFolderName);
        BuildConfiguration = new DirectoryInfo(_currentDirectory).Parent!.Name;
        TestSolutionPath = Path.Combine(TestSolutionDirectory, SolutionName);
        _git2SemVerToolPath =
            Path.Combine(_solutionDirectory, "src", "Tool", "Git2SemVer.Tool/bin", BuildConfiguration, "net8.0", "NoeticTools.Git2SemVer.Tool.dll");
    }

    protected void SetUpBase()
    {
        Logger = new NUnitLogger { Level = LoggingLevel.Trace };

        if (_testDataFolderId > MaximumTestDataFolders)
        {
            _testDataFolderId = 0;
        }

        var dataFolderId = ++_testDataFolderId;
        _testFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                       "Git2SemVer",
                                       $"TestData{dataFolderId}");
        if (Directory.Exists(_testFolderPath))
        {
            Directory.Delete(_testFolderPath, true);
            if (!WaitUntil(() => !Directory.Exists(_testFolderPath)))
            {
                Assert.Fail($"Unable to deleted folder '{_testFolderPath}'.");
            }
        }

        Directory.CreateDirectory(_testFolderPath);
        if (!WaitUntil(() => Directory.Exists(_testFolderPath)))
        {
            Assert.Fail($"Unable to create folder '{_testFolderPath}'.");
        }
    }

    protected string BuildConfiguration { get; private set; } = "";

    protected DotNetTool DotNetCli { get; private set; } = null!;

    protected ILogger Logger { get; private set; } = null!;

    protected abstract string SolutionFolderName { get; }

    protected abstract string SolutionName { get; }

    protected string TestSolutionDirectory { get; private set; } = "";

    protected string TestSolutionPath { get; private set; } = "";

    protected static void DeleteAllNuGetPackages(string packageOutputDir)
    {
        if (string.IsNullOrWhiteSpace(packageOutputDir) || !Directory.Exists(packageOutputDir))
        {
            return;
        }

        foreach (var filePath in Directory.EnumerateFiles(packageOutputDir, "*.nupkg"))
        {
            File.Delete(filePath);
        }
    }

    protected string DeployScript(string scriptFilename)
    {
        var scriptPath = Path.Combine(_testFolderPath, scriptFilename);
        GetType().Assembly.WriteResourceFile(scriptFilename, scriptPath);
        return scriptPath;
    }

    protected (int returnCode, string stdOutput) ExecuteGit2SemVerTool(string commandLineArguments)
    {
        var process = new ProcessCli(Logger)
        {
            WorkingDirectory = TestSolutionDirectory
        };
        var returnCode = process.Run("dotnet", $"{_git2SemVerToolPath} {commandLineArguments}", out var standardOutput);
        return (returnCode, standardOutput);
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