using NoeticTools.Git2SemVer.Tool.Integration.Tests.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.Tool.Integration.Tests;

[TestFixture]
[NonParallelizable]
internal class ToolIntegrationTests : SolutionTestsBase
{
    private string _packageOutputDir;

    [TestCase("ver setup")]
    [TestCase("versioning setup")]
    public void AddCommandTest(string commandPrefix)
    {
        var result = ExecuteGit2SemVerTool(commandPrefix + " add -u");

        Console.WriteLine(result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
    }

    [Test]
    public void ChangelogCommandTest()
    {
        var dataFolderPath = Path.Combine(TestSolutionDirectory, ".git2semver");
        var changelogFilePath = Path.Combine(TestSolutionDirectory, "CHANGELOG.md");
        if (File.Exists(changelogFilePath))
        {
            File.Delete(changelogFilePath);
        }

        if (Directory.Exists(dataFolderPath))
        {
            Directory.Delete(dataFolderPath, true);
        }

        try
        {
            var result = ExecuteGit2SemVerTool("changelog run -u");

            Console.WriteLine(result.stdOutput);
            Assert.That(Logger.HasError, Is.False);
            Assert.That(result.returnCode, Is.Zero);
            Assert.That(File.Exists(changelogFilePath));
            Assert.That(Directory.Exists(dataFolderPath));
        }
        finally
        {
            File.Delete(changelogFilePath);
            Directory.Delete(Path.Combine(TestSolutionDirectory, ".git2semver"), true);
        }
    }

    [Test]
    public void DepreciatedAddCommandTest()
    {
        var result = ExecuteGit2SemVerTool("add -u");

        Console.WriteLine(result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
    }

    [Test]
    public void DepreciatedRemoveCommandTest()
    {
        var result = ExecuteGit2SemVerTool("remove -u");

        Console.WriteLine(result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
    }

    [Test]
    public void DepreciatedToolHelpAddCommand()
    {
        var result = ExecuteGit2SemVerTool("add --help");
        TestContext.Out.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
        Assert.That(result.stdOutput, Does.Contain("-h, --help"));
        Assert.That(result.stdOutput, Does.Contain("-u, --unattended"));
    }

    [Test]
    public void RemoveCommandTest()
    {
        var result = ExecuteGit2SemVerTool("versioning setup remove -u");

        Console.WriteLine(result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
    }

    /// <summary>
    ///     This test requires manual inspection of output to see that logging levels are correct.
    /// </summary>
    [TestCase("info")]
    [TestCase("debug")]
    [TestCase("trace")]
    public void RunCommandTest(string verbosity)
    {
        var result = ExecuteGit2SemVerTool($"ver run -u --verbosity {verbosity}");

        Console.WriteLine(result.stdOutput);
        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
    }

    [Test]
    public void ToolHelpAddCommand()
    {
        var result = ExecuteGit2SemVerTool("versioning setup add --help");
        TestContext.Out.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
        Assert.That(result.stdOutput, Does.Contain("-h, --help"));
        Assert.That(result.stdOutput, Does.Contain("-u, --unattended"));
    }

    [Test]
    public void ToolHelpCommand()
    {
        var result = ExecuteGit2SemVerTool("--help");
        TestContext.Out.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
        Assert.That(result.stdOutput, Does.Contain("-h, --help"));
        Assert.That(result.stdOutput, Does.Not.Contain("-u, --unattended"));
    }

    [Test]
    public void ToolHelpRunCommand()
    {
        var result = ExecuteGit2SemVerTool("ver run --help");
        TestContext.Out.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
        Assert.That(result.stdOutput, Does.Contain("-h, --help"));
        Assert.That(result.stdOutput, Does.Contain("-u, --unattended"));
        Assert.That(result.stdOutput, Does.Contain("-v, --verbosity"));
        Assert.That(result.stdOutput, Does.Contain("--host-type"));
        Assert.That(result.stdOutput, Does.Contain("--output"));
        Assert.That(result.stdOutput, Does.Contain("--enable-json-write"));
    }

    [Test]
    public void ToolInvalidArgumentHandling()
    {
        var result = ExecuteGit2SemVerTool("--unknown");

        TestContext.Out.WriteLine(Logger.Errors);
        Assert.That(result.returnCode, Is.Not.Zero);
    }

    [Test]
    public void ToolVersionCommand()
    {
        var result = ExecuteGit2SemVerTool("--version");
        Console.WriteLine(result.stdOutput);

        Assert.That(Logger.HasError, Is.False);
        Assert.That(result.returnCode, Is.Zero);
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        OneTimeSetUpBase();

        var testProjectBinDirectory = Path.Combine(TestSolutionDirectory, "TestApplication/bin/", BuildConfiguration);
        _packageOutputDir = testProjectBinDirectory;
    }

    [SetUp]
    public void SetUp()
    {
        SetUpBase();
        DeleteAllNuGetPackages(_packageOutputDir);
    }

    [TearDown]
    public void TearDown()
    {
        Console.WriteLine("-- Test Tear Down --");
        ExecuteGit2SemVerTool("remove -u");
    }

    protected override string SolutionFolderName => "SolutionVersioning";

    protected override string SolutionName => "StandAloneVersioning.sln";
}