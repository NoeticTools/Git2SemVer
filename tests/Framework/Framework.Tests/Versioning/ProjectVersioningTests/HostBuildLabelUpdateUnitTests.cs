using Moq;
using NoeticTools.Git2SemVer.Framework.Versioning;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Tests.Versioning.ProjectVersioningTests;

internal class HostBuildLabelUpdateUnitTests : ProjectVersioningUnitTestsBase
{
    [TestCase(VersioningMode.SolutionVersioningProject)]
    [TestCase(VersioningMode.SolutionClientProject)]
    [TestCase(VersioningMode.StandAloneProject)]
    public void DoesNotUpdatesBuildLabel_WhenNoBuildSystemVersion(VersioningMode mode)
    {
        Inputs.Setup(x => x.UpdateHostBuildLabel).Returns(true);

        Target.Run(mode);

        Host.Verify(x => x.SetBuildLabel(It.IsAny<string>()), Times.Never);
    }

    [TestCase(VersioningMode.SolutionVersioningProject)]
    [TestCase(VersioningMode.SolutionClientProject)]
    [TestCase(VersioningMode.StandAloneProject)]
    public void DoesNotUpdatesBuildLabel_WhenNotEnabled(VersioningMode mode)
    {
        Inputs.Setup(x => x.UpdateHostBuildLabel).Returns(false);
        var buildSystemVersion = SemVersion.ParsedFrom(1, 2, 3);
        SharedCachedOutputs.Setup(x => x.BuildSystemVersion).Returns(buildSystemVersion);
        GeneratedOutputs.Setup(x => x.BuildSystemVersion).Returns(buildSystemVersion);

        Target.Run(mode);

        Host.Verify(x => x.SetBuildLabel(It.IsAny<string>()), Times.Never);
    }

    [TestCase(VersioningMode.SolutionVersioningProject)]
    [TestCase(VersioningMode.SolutionClientProject)]
    [TestCase(VersioningMode.StandAloneProject)]
    public void UpdatesBuildLabel_WhenEnabledAndOutputsAvailable(VersioningMode mode)
    {
        Inputs.Setup(x => x.UpdateHostBuildLabel).Returns(true);
        var buildSystemVersion = SemVersion.ParsedFrom(1, 2, 3);
        SharedCachedOutputs.Setup(x => x.BuildSystemVersion).Returns(buildSystemVersion);
        GeneratedOutputs.Setup(x => x.BuildSystemVersion).Returns(buildSystemVersion);

        Target.Run(mode);

        Host.Verify(x => x.SetBuildLabel(buildSystemVersion.ToString()), Times.Once);
    }

    [SetUp]
    public void SetUp()
    {
        SharedCachedOutputs.Setup(x => x.IsValid).Returns(true);
        GeneratedOutputs.Setup(x => x.IsValid).Returns(true);
        VersionGenerator.Setup(x => x.PrebuildRun(It.IsAny<VersioningMode>())).Returns(new VersioningOutputs(GeneratedOutputs.Object, null));
    }
}