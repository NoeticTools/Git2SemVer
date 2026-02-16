using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using NoeticTools.Git2SemVer.IntegrationTests.Framework;
using Semver;


#pragma warning disable NUnit2045

namespace NoeticTools.Git2SemVer.IntegrationTests.Building;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class StandAloneBuildTests : VersioningBuildTestsBase
{
    [Test]
    public void BuildAndPackTest()
    {
        using var context = CreateTestContext();

        var returnCode = context.DotNetCli.Pack(context.TestSolutionPath, context.BuildConfiguration);
        Assert.That(returnCode, Is.Zero);

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        Assert.That(File.Exists(context.CompiledAppPath), Is.True, $"File '{context.CompiledAppPath}' does not exist after build and pack.");

        var infoVersion = context.GetInfoVersionFromAppOutput(output);
        AssertNugetPackageExists(infoVersion, context);
        AssertOutputVersionVariations(infoVersion, output);
    }

    [Test]
    public void BuildTest()
    {
        using var context = CreateTestContext();

        context.DotNetCli.Build(context.TestSolutionPath, context.BuildConfiguration);
        context.ShowVersioningReport();
        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);

        var infoVersion = context.GetInfoVersionFromAppOutput(output);
        AssertOutputVersionVariations(infoVersion, output);
    }

    protected override VersioningBuildTestContext CreateTestContext()
    {
        return new VersioningBuildTestContext("StandAlone", "StandAloneTestSolution",
                                              "StandAloneVersioning.sln", "TestApplication");
    }
}