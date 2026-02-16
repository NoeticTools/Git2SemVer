using NoeticTools.Git2SemVer.Tool.Integration.Tests.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Tool.Integration.Tests;

internal abstract class VersioningBuildTestsBase : SolutionTestsBase
{
    private string _compiledAppPath;
    private string _packageOutputDir;

    [Test]
    [CancelAfter(60000)]
    public void BuildOnlyTest()
    {
        DotNetCliBuildTestSolution();

        var output = DotNetProcessHelpers.RunDotnetApp(_compiledAppPath, Logger);
        Assert.That(output, Does.Contain("""
                                         Assembly version:       200.201.202.0
                                         File version:           200.201.212
                                         Informational version:  2.2.2-beta
                                         Product version:        2.2.2-beta
                                         """));
    }

    [Test]
    [CancelAfter(60000)]
    public void PackWithForcingProperties1ScriptTest()
    {
        var scriptPath = DeployScript("ForceProperties1.csx");

        var returnCode = DotNetCli.Pack(TestSolutionPath, BuildConfiguration, $"-p:Git2SemVer_ScriptPath={scriptPath} -fileLogger");
        Assert.That(returnCode, Is.Zero);

        var output = DotNetProcessHelpers.RunDotnetApp(_compiledAppPath, Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       1.2.3.0
                                               File version:           4.5.6
                                               Informational version:  11.12.13-a-prerelease+metadata
                                               Product version:        11.12.13-a-prerelease+metadata
                                               """));
        AssertFileExists(_packageOutputDir, "NoeticTools.TestApplication.5.6.7.nupkg");
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OneTimeSetUpBase();

        var testProjectBinDirectory = Path.Combine(TestSolutionDirectory, "TestApplication/bin/", BuildConfiguration);
        _compiledAppPath = Path.Combine(testProjectBinDirectory, "net8.0", "NoeticTools.TestApplication.dll");
        _packageOutputDir = testProjectBinDirectory;
    }

    [SetUp]
    public void SetUp()
    {
        SetUpBase();
        if (Directory.Exists(_packageOutputDir))
        {
            Directory.Delete(_packageOutputDir, true);
        }
    }

    private static void AssertFileExists(string packageDirectory, string expectedFilename)
    {
        var directory = new DirectoryInfo(packageDirectory);
        var foundFiles = directory.GetFiles(expectedFilename);
        Assert.That(foundFiles.Length, Is.EqualTo(1), $"File '{expectedFilename}' does not exist.");
    }

    private void DotNetCliBuildTestSolution(params string[] arguments)
    {
        var returnCode = DotNetCli.Build(TestSolutionPath, BuildConfiguration, arguments);
        Assert.That(returnCode, Is.Zero);
        Assert.That(Logger.HasError, Is.False);
    }
}