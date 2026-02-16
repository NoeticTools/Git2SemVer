

namespace NoeticTools.Git2SemVer.Core.IntegrationTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class DotNetToolIntegrationTests
{
    [Test]
    public void NewClasslibProjectInSolutionTest()
    {
        using var context = new DotNetToolIntegrationTestContext();
        context.DotNetCli.Solution.New();
        var solutionFiles = context.TestDirectory.GetFiles();
        Assert.That(solutionFiles.Length, Is.EqualTo(1));

        context.DotNetCli.Projects.NewClassLib("Project1");

        var expectedProjectPath = Path.Combine(context.TestDirectory.FullName, "Project1", "Project1.csproj");
        Assert.That(expectedProjectPath, Does.Exist);
    }

    [Test]
    public void NewNamedSolutionTest()
    {
        using var context = new DotNetToolIntegrationTestContext();
        context.Logger.Level = Core.Logging.LoggingLevel.Trace;//>>>

        context.DotNetCli.Solution.New("MyName");

        var solutionFiles = context.TestDirectory.GetFiles();
        Assert.That(solutionFiles.Length, Is.EqualTo(1));
        Assert.That(solutionFiles[0].Name, Is.EqualTo("MyName.sln"));
    }

    [Test]
    public void NewProjectInSolutionTest()
    {
        using var context = new DotNetToolIntegrationTestContext();
        context.DotNetCli.Solution.New();
        var solutionFiles = context.TestDirectory.GetFiles();
        Assert.That(solutionFiles.Length, Is.EqualTo(1));

        context.DotNetCli.Projects.New("classlib", "Project1");

        var expectedProjectPath = Path.Combine(context.TestDirectory.FullName, "Project1", "Project1.csproj");
        Assert.That(expectedProjectPath, Does.Exist);
    }

    [Test]
    public void NewSolutionTest()
    {
        using var context = new DotNetToolIntegrationTestContext();

        context.DotNetCli.Solution.New();

        var solutionFiles = context.TestDirectory.GetFiles();
        Assert.That(solutionFiles.Length, Is.EqualTo(1));
        Assert.That(solutionFiles[0].Name, Is.EqualTo(context.TestFolderName + ".sln"));
    }
}