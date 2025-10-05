using NoeticTools.Git2SemVer.Tool.Integration.Tests.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Tool.Integration.Tests;

internal abstract class VersioningBuildTestsBase : SolutionTestsBase
{
    //[Test]
    //[CancelAfter(60000)]
    //public void BuildAndThenPackWithoutRebuildTest()
    //{
    //    var scriptPath = DeployScript("ForceProperties3.csx");
    //    DotNetCliBuildTestSolution($"-p:Git2SemVer_ScriptPath={scriptPath}");
    //    PackTestSolution();
    //    AssertFileExists(PackageOutputDir, "NoeticTools.TestApplication.1.2.3-alpha.nupkg");

    //    var output = DotNetProcessHelpers.RunDotnetApp(CompiledAppPath, Logger);
    //    Assert.That(output, Does.Contain("""
    //                                     Assembly version:       200.201.202.0
    //                                     File version:           200.201.212
    //                                     Informational version:  2.2.2-beta
    //                                     Product version:        2.2.2-beta
    //                                     """));
    //}

    [Test]
    [CancelAfter(60000)]
    public void BuildOnlyTest()
    {
        var scriptPath = DeployScript("ForceProperties3.csx");
        DotNetCliBuildTestSolution($"-p:Git2SemVer_ScriptPath={scriptPath}");

        var output = DotNetProcessHelpers.RunDotnetApp(CompiledAppPath, Logger);
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
        Assert.That(returnCode, Is.EqualTo(0));

        var output = DotNetProcessHelpers.RunDotnetApp(CompiledAppPath, Logger);
        Assert.That(output, Contains.Substring("""
                                               Assembly version:       1.2.3.0
                                               File version:           4.5.6
                                               Informational version:  11.12.13-a-prerelease+metadata
                                               Product version:        11.12.13-a-prerelease+metadata
                                               """));
        AssertFileExists(PackageOutputDir, "NoeticTools.TestApplication.5.6.7.nupkg");
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OneTimeSetUpBase();

        //BuildGit2SemVerMSBuild();
        //BuildGit2SemVerTool();

        var testProjectBinDirectory = Path.Combine(TestSolutionDirectory, "TestApplication/bin/", BuildConfiguration);
        CompiledAppPath = Path.Combine(testProjectBinDirectory, "net8.0", "NoeticTools.TestApplication.dll");
        PackageOutputDir = testProjectBinDirectory;
    }

    [SetUp]
    public void SetUp()
    {
        SetUpBase();
        if (Directory.Exists(PackageOutputDir))
        {
            Directory.Delete(PackageOutputDir, true);
        }
    }

    protected static void AssertFileExists(string packageDirectory, string expectedFilename)
    {
        var directory = new DirectoryInfo(packageDirectory);
        var foundFiles = directory.GetFiles(expectedFilename);
        Assert.That(foundFiles.Length, Is.EqualTo(1), $"File '{expectedFilename}' does not exist.");
    }

    protected void DotNetCliBuildTestSolution(params string[] arguments)
    {
        var returnCode = DotNetCli.Build(TestSolutionPath, BuildConfiguration, arguments);
        Assert.That(returnCode, Is.EqualTo(0));
        Assert.That(Logger.HasError, Is.False);
    }

    private void BuildTestSolution(string scriptName)
    {
        var scriptPath = DeployScript(scriptName);
        DotNetCliBuildTestSolution($"-p:Git2SemVer_ScriptPath={scriptPath}");
    }

    private void BuildTestSolutionAndRemovePackage()
    {
        DotNetCliBuildTestSolution();
        DeleteAllNuGetPackages(PackageOutputDir);
    }

    //private void PackTestSolution()
    //{
    //    var result = DotNetCli.Pack(TestSolutionPath, BuildConfiguration, "--no-restore --no-build");
    //    TestContext.Out.WriteLine(result.stdOutput);
    //    Assert.That(result.returnCode, Is.EqualTo(0));
    //    Assert.That(Logger.HasError, Is.False);
    //}

    protected string CompiledAppPath;
    protected string PackageOutputDir;
}