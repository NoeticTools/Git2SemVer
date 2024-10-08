using NoeticTools.Common;
using NoeticTools.Common.Tools;
using NoeticTools.Git2SemVer.MSBuild.IntegrationTests.Framework;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.IntegrationTests.Framework;

public abstract class SolutionTestsBase : ScriptingTestsBase
{
    protected string SolutionDirectory = "";
    protected string TestSolutionDirectory = "";
    protected string TestSolutionPath = "";
    protected string BuildConfiguration = "";
    protected string CurrentDirectory = "";
    protected string Git2SemVerToolPath = "";

    protected abstract string SolutionFolderName { get; }

    protected abstract string SolutionName { get; }

    protected override void OneTimeSetUpBase()
    {
        base.OneTimeSetUpBase();
        CurrentDirectory = Directory.GetCurrentDirectory();
        SolutionDirectory = DotNetProcessHelpers.GetSolutionDirectory();
        TestSolutionDirectory = Path.Combine(SolutionDirectory, "TestSolutions", SolutionFolderName);
        BuildConfiguration = new DirectoryInfo(CurrentDirectory).Parent!.Name;
        TestSolutionPath = Path.Combine(TestSolutionDirectory, SolutionName);
        Git2SemVerToolPath = Path.Combine(SolutionDirectory, "Git2SemVer.Tool/bin", BuildConfiguration, "net8.0", "NoeticTools.Git2SemVer.Tool.dll");
    }

    protected static void DeleteAllNuGetPackages(string packageOutputDir)
    {
        if (!string.IsNullOrWhiteSpace(packageOutputDir) && Directory.Exists(packageOutputDir))
        {
            foreach (var filePath in Directory.EnumerateFiles(packageOutputDir, "*.nupkg"))
            {
                File.Delete(filePath);
            }
        }
    }

    protected void BuildGit2SemVerTool()
    {
        var projectPath = Path.Combine(SolutionDirectory, "Git2SemVer.Tool/Git2SemVer.Tool.csproj");
        var result = DotNetCli.Build(projectPath, BuildConfiguration);
        Assert.That(result.returnCode, Is.EqualTo(0));
        Assert.That(Logger.HasError, Is.False);
    }

    protected void BuildGit2SemVerMSBuild()
    {
        var projectPath = Path.Combine(SolutionDirectory, "Git2SemVer.MSBuild/Git2SemVer.MSBuild.csproj");
        var result = DotNetCli.Build(projectPath, BuildConfiguration);
        Assert.That(result.returnCode, Is.EqualTo(0));
        Assert.That(Logger.HasError, Is.False);
    }

    protected string DeployScript(string scriptFilename)
    {
        var scriptPath = Path.Combine(TestFolderPath, scriptFilename);
        GetType().Assembly.WriteResourceFile(scriptFilename, scriptPath);
        return scriptPath;
    }

    protected (int returnCode, string stdOutput) ExecuteGit2SemVerTool(string commandLineArguments)
    {
        var process = new ProcessCli(Logger)
        {
            WorkingDirectory = TestSolutionDirectory
        };
        return process.Run("dotnet", $"{Git2SemVerToolPath} {commandLineArguments}");
    }
}