using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.IntegrationTests.Framework;


namespace NoeticTools.Git2SemVer.IntegrationTests.Building;

[TestFixture]
[NonParallelizable]
public class UncontrolledHostBuildTests
{
    [Test]
    [Explicit("Can only be run on an uncontrolled host (dev box)")]
    public void BuildNumberIncrementsOnRebuildTest()
    {
        using var context = CreateTestContext();

        var firstBuildNumber = RebuildAndRun(context);
        var secondBuildNumber = RebuildAndRun(context);

        Assert.That(secondBuildNumber - firstBuildNumber, Is.EqualTo(1));
    }

    [Test]
    [Explicit("Can only be run on an uncontrolled host (dev box)")]
    public void HostDetectedTest()
    {
        var config = Git2SemVerLocalSettings.Load();
        using var context = CreateTestContext();
        using var logger = new CompositeLogger(context.Logger);
        using var host = new BuildHost(new BuildHostFinder(config, new TeamCityWriterFactoryStub().Create(), logger).Find(""), logger);

        Assert.That(host.HostTypeId, Is.EqualTo(HostTypeIds.Uncontrolled));
        Assert.That(int.Parse(host.BuildNumber), Is.GreaterThan(0));
        Assert.That(host.BuildNumber, Is.EqualTo(config.BuildNumber.ToString()));
        Assert.That(host.BuildContext, Is.EqualTo(Environment.MachineName));
        Assert.That(host.BuildId, Is.EqualTo(new[] { Environment.MachineName, config.BuildNumber.ToString() }));
    }

    private static VersioningBuildTestContext CreateTestContext()
    {
        return new VersioningBuildTestContext(groupName: "UncontrolledHost",
                                              solutionFolderName: "StandAloneTestSolution",
                                              solutionFileName: "StandAloneVersioning.sln",
                                              projectName: "TestApplication");
    }

    private static int GetBuildNumber(string output)
    {
        var regex = new Regex(@"^Informational version:\s+.+-.*\.(?<build_number>\d+)\+", RegexOptions.Multiline);
        var match = regex.Match(output);
        Assert.That(match.Success, Is.True);
        return int.Parse(match.Groups["build_number"].Value);
    }

    private static int RebuildAndRun(VersioningBuildTestContext context)
    {
        context.DotNetCliBuildTestSolution("--no-incremental");
        var output = DotNetProcessHelpers.RunDotnetApp(context.CompiledAppPath, context.Logger);
        return GetBuildNumber(output);
    }
}