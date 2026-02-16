using System.IO.Compression;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.IntegrationTests.Framework;
using NoeticTools.Git2SemVer.Testing.Core;
using Semver;


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

    public void Dispose()
    {
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

    public SemVersion GetInfoVersionFromAppOutput(string reportOutput)
    {
        var versionLine = reportOutput.Split(Environment.NewLine)
                                      .FirstOrDefault(line => line.StartsWith("Informational version:", StringComparison.Ordinal));
        Assert.That(versionLine, Is.Not.Null, "Informational version line not found in application output.");
        var versionString = versionLine["Informational version:".Length..]!.Trim();
        return SemVersion.Parse(versionString, SemVersionStyles.Any);
    }

    public void ShowVersioningReport()
    {
        var report = File.ReadAllText(Path.Combine(TestProjectDirectory, "obj/Git2SemVer.MSBuild.log"));
        Logger.LogInfo(report);
    }

    private DirectoryInfo TestDirectory { get; }

    private void ExtractResourceToDirectory(string filename, string extractPath)
    {
        if (Directory.Exists(extractPath))
        {
            Directory.Delete(extractPath, true);
            TestHelper.WaitUntil(() => !Directory.Exists(extractPath));
        }

        using var stream = GetType().Assembly.GetResourceStream(filename);
        ZipFile.ExtractToDirectory(stream, extractPath);
    }
}