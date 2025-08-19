using System.IO.Compression;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.FileSystem;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Testing.Core;
using NUnit.Framework.Interfaces;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.IntegrationTests.Building;

internal sealed class VersioningBuildTestContext : IDisposable
{
    private const int ConcurrentContextsLimit = 100;
    private static int _activeContexts;
    private readonly TestDirectoryResource _testDirectoryResource;

    public VersioningBuildTestContext(string groupName, string solutionFolderName, string solutionFileName, string projectName)
    {
        if (_activeContexts > ConcurrentContextsLimit)
        {
            Assert.Fail($"Exceeded number of active contexts limit of {ConcurrentContextsLimit}.");
        }

        _activeContexts++;
        _testDirectoryResource = new TestDirectoryResource(groupName);

        Logger = new NUnitLogger(false) { Level = LoggingLevel.Trace };

        TestDirectory = _testDirectoryResource.Create();
        Logger.LogInfo("Created test directory {0}.", TestDirectory.FullName);

        var processCli = new ProcessCli(Logger) { WorkingDirectory = TestDirectory.FullName };
        DotNetCli = new DotNetTool(processCli);

        var currentDirectory = Directory.GetCurrentDirectory();
        BuildConfiguration = new DirectoryInfo(currentDirectory).Parent!.Name;

        TestSolutionDirectory = Path.Combine(TestDirectory.FullName, solutionFolderName);
        TestSolutionPath = Path.Combine(TestSolutionDirectory, solutionFileName);
        TestProjectDirectory = Path.Combine(TestSolutionDirectory, projectName);
        var testProjectBinDirectory = Path.Combine(TestProjectDirectory, "bin", BuildConfiguration);
        CompiledAppPath = Path.Combine(testProjectBinDirectory, "net8.0", "NoeticTools.TestApplication.dll");
        PackageOutputDir = testProjectBinDirectory;
        ExtractResourceToDirectory(solutionFolderName + ".zip", TestDirectory.FullName);
    }

    public string BuildConfiguration { get; }

    public string CompiledAppPath { get; }

    public DotNetTool DotNetCli { get; }

    public NUnitLogger Logger { get; }

    public string PackageOutputDir { get; }

    public string TestProjectDirectory { get; }

    public string TestSolutionDirectory { get; }

    public string TestSolutionPath { get; }

    public static void AssertFileExists(string packageDirectory, string expectedFilename)
    {
        var directory = new DirectoryInfo(packageDirectory);
        var foundFiles = directory.GetFiles(expectedFilename);
        Assert.That(foundFiles.Length, Is.EqualTo(1), $"File '{expectedFilename}' does not exist.");
    }

    public string DeployScript(string scriptFilename)
    {
        var scriptPath = Path.Combine(TestDirectory.FullName, scriptFilename);
        GetType().Assembly.WriteResourceFile(scriptFilename, scriptPath);
        return scriptPath;
    }

    public void Dispose()
    {
        if (NUnit.Framework.TestContext.CurrentContext.Result.Outcome == ResultState.Failure)
        {
            Logger.LogInfo("Test failed. Dumping log files from {0}:\n", TestDirectory.FullName);

            var workingDirectory = new DirectoryPath(TestDirectory.FullName);
            var logFiles = workingDirectory.GetFiles(pattern: "*.log", recursive: true);

            foreach (var logFile in logFiles)
            {
                Logger.LogInfo("Log file: {0}", logFile);
                var content = File.ReadAllText(logFile);
                Logger.LogInfo(content + "\n");
            }
        }

        _activeContexts--;
        _testDirectoryResource.Dispose();
    }

    public void DotNetCliBuildTestSolution(params string[] arguments)
    {
        var returnCode = DotNetCli.Build(TestSolutionPath, BuildConfiguration, arguments);
        Assert.That(returnCode, Is.EqualTo(0));
        Assert.That(Logger.HasError, Is.False);
    }

    public void PackTestSolution()
    {
        var returnCode = DotNetCli.Pack(TestSolutionPath, BuildConfiguration, "--no-restore --no-build");
        Assert.That(returnCode, Is.EqualTo(0));
        Assert.That(Logger.HasError, Is.False);
    }

    public void ShowVersioningReport()
    {
        var report = File.ReadAllText(Path.Combine(TestProjectDirectory, "obj/Git2SemVer.MSBuild.log"));
        Logger.LogInfo(report);
    }

    private DirectoryInfo TestDirectory { get; }

    private void ExtractResourceToDirectory(string filename, DirectoryPath extractDirectory)
    {
        if (extractDirectory.Exists())
        {
            extractDirectory.Delete(true);
        }

        using var stream = GetType().Assembly.GetResourceStream(filename);
        ZipFile.ExtractToDirectory(stream, extractDirectory);
    }
}