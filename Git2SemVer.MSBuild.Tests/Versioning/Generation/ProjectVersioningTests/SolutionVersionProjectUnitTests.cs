﻿using Moq;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using Semver;


// ReSharper disable InconsistentNaming


namespace NoeticTools.Git2SemVer.MSBuild.Tests.Versioning.Generation.ProjectVersioningTests;

internal class SolutionVersionProjectUnitTests : ProjectVersioningUnitTestsBase
{
    [SetUp]
    public void SetUp()
    {
        ModeIs(VersioningMode.SolutionVersioningProject);
        SharedCachedOutputs.Setup(x => x.BuildNumber).Returns("42");
    }

    [Test]
    public void DoesGenerate_WhenCachedOutputsNotAvailable()
    {
        SharedCachedOutputs.Setup(x => x.IsValid).Returns(false);

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Once);
        Assert.That(result, Is.SameAs(GeneratedOutputs.Object));
        OutputsCacheJsonFile.Verify(x => x.Load("IntermediateOutputDirectory"), Times.Never);
    }

    [Test]
    public void DoesNotGenerate_WhenCachedOutputsAvailable()
    {
        SharedCachedOutputs.Setup(x => x.IsValid).Returns(true);

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Never);
        Assert.That(result, Is.SameAs(SharedCachedOutputs.Object));
        OutputsCacheJsonFile.Verify(x => x.Load("IntermediateOutputDirectory"), Times.Never);
    }
}