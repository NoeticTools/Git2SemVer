﻿using Moq;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using Semver;


// ReSharper disable InconsistentNaming


namespace NoeticTools.Git2SemVer.MSBuild.Tests.Versioning.Generation.ProjectVersioningTests;

internal class SolutionClientProjectUnitTests : ProjectVersioningUnitTestsBase
{
    [SetUp]
    public void SetUp()
    {
        ModeIs(VersioningMode.SolutionClientProject);
        Host.Setup(x => x.BuildNumber).Returns("42");
    }

    [Test]
    public void DoesGenerate_WhenCachedOutputsNotAvailable()
    {
        LocalCachedOutputs.Setup(x => x.IsValid).Returns(false);
        SharedCachedOutputs.Setup(x => x.IsValid).Returns(false);

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Once);
        Assert.That(result, Is.SameAs(GeneratedOutputs.Object));
    }

    [Test]
    public void DoesGenerate_WhenLocalCacheHasSameBuildNumber()
    {
        LocalCachedOutputs.Setup(x => x.IsValid).Returns(true);
        LocalCachedOutputs.Setup(x => x.BuildNumber).Returns("42");
        SharedCachedOutputs.Setup(x => x.IsValid).Returns(false);

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Once);
        Assert.That(result, Is.SameAs(GeneratedOutputs.Object));
    }

    [Test]
    public void DoesGenerate_WhenNoLocalCacheButSharedCacheHasSameBuildNumber()
    {
        LocalCachedOutputs.Setup(x => x.IsValid).Returns(false);
        SharedCachedOutputs.Setup(x => x.IsValid).Returns(true);
        SharedCachedOutputs.Setup(x => x.BuildNumber).Returns("42");

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Once);
        Assert.That(result, Is.SameAs(GeneratedOutputs.Object));
    }

    [Test]
    public void DoesNotGenerate_WhenLocalCacheHasDifferentBuildNumber()
    {
        LocalCachedOutputs.Setup(x => x.IsValid).Returns(true);
        LocalCachedOutputs.Setup(x => x.BuildNumber).Returns("41");
        SharedCachedOutputs.Setup(x => x.IsValid).Returns(false);

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Never);
        Assert.That(result, Is.SameAs(SharedCachedOutputs.Object));
    }

    [Test]
    public void DoesNotGenerate_WhenNoLocalCacheAndSharedCacheHasDifferentBuildNumber()
    {
        LocalCachedOutputs.Setup(x => x.IsValid).Returns(false);
        SharedCachedOutputs.Setup(x => x.IsValid).Returns(true);
        SharedCachedOutputs.Setup(x => x.BuildNumber).Returns("43");

        var result = Target.Run();

        VersionGenerator.Verify(x => x.Run(), Times.Never);
        Assert.That(result, Is.SameAs(SharedCachedOutputs.Object));
    }
}