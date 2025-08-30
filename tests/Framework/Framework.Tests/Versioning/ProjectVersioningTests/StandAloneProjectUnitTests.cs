using Moq;
using NoeticTools.Git2SemVer.Framework.Versioning;


// ReSharper disable InconsistentNaming

namespace NoeticTools.Git2SemVer.Framework.Tests.Versioning.ProjectVersioningTests;

internal class StandAloneProjectUnitTests : ProjectVersioningUnitTestsBase
{
    [SetUp]
    public void SetUp()
    {
    }

    [TestCase]
    public void AlwaysGeneratesVersionTest()
    {
        var result = Target.Run(VersioningMode.StandAloneProject);

        VersionGenerator.Verify(x => x.PrebuildRun(VersioningMode.StandAloneProject), Times.Once);
        Assert.That(result.Versions, Is.SameAs(GeneratedOutputs.Object));
        OutputsCacheJsonFile.Verify(x => x.Read(It.IsAny<string>()), Times.Never);
        OutputsCacheJsonFile.Verify(x => x.Write(It.IsAny<string>(), It.IsAny<IVersionOutputs>()), Times.Never);
    }
}