using NoeticTools.Git2SemVer.Framework.Framework.Semver;
using NoeticTools.Git2SemVer.IntegrationTests.Framework;
using Semver;


namespace NoeticTools.Git2SemVer.IntegrationTests.Building;

internal abstract class VersioningBuildTestsBase
{
    [Test]
    [CancelAfter(60000)]
    public void BuildAndThenPackWithoutRebuildTest()
    {
        using var context = CreateTestContext();

        context.DotNetCliBuildTestSolution();
        context.PackTestSolution();

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        var infoVersion = context.GetInfoVersionFromAppOutput(output);
        AssertOutputVersionVariations(infoVersion, output);
        AssertNugetPackageExists(infoVersion, context);
    }

    [Test]
    [CancelAfter(60000)]
    public void BuildOnlyTest()
    {
        using var context = CreateTestContext();

        context.DotNetCliBuildTestSolution();

        context.ShowVersioningReport();

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        var infoVersion = context.GetInfoVersionFromAppOutput(output);
        AssertOutputVersionVariations(infoVersion, output);
    }

    [Test]
    [CancelAfter(60000)]
    public void PackTest()
    {
        using var context = CreateTestContext();

        var returnCode = context.DotNetCli.Pack(context.TestSolutionPath, context.BuildConfiguration,
                                                "-fileLogger");
        Assert.That(returnCode, Is.Zero);

        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        var infoVersion = context.GetInfoVersionFromAppOutput(output);
        AssertOutputVersionVariations(infoVersion, output);
        AssertNugetPackageExists(infoVersion, context);
    }

    protected void AssertNugetPackageExists(SemVersion infoVersion, VersioningBuildTestContext context)
    {
        var fileVersion = $"{infoVersion.Major}.{infoVersion.Minor}.{infoVersion.Patch}";
        if (infoVersion.IsPrerelease)
        {
            fileVersion += $"-{infoVersion.Prerelease}";
        }

        VersioningBuildTestContext.AssertFileExists(context.PackageOutputDir, $"NoeticTools.TestApplication.{fileVersion}.nupkg");
    }

    protected static void AssertOutputVersionVariations(SemVersion infoVersion, string output)
    {
        Assert.That(output, Contains.Substring($"""
                                                Assembly version:       {infoVersion.ToAssemblyVersion()}
                                                File version:           {infoVersion.ToFileVersion()}
                                                Informational version:  {infoVersion}
                                                Product version:        {infoVersion}
                                                """));
    }

    protected abstract VersioningBuildTestContext CreateTestContext();
}