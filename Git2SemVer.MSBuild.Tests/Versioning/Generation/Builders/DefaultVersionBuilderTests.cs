﻿using Moq;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Framework.Semver;
using NoeticTools.Git2SemVer.MSBuild.Versioning;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.Builders;
using NoeticTools.Git2SemVer.MSBuild.Versioning.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.Testing.Core;
using Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Tests.Versioning.Generation.Builders;

[TestFixture]
internal class DefaultVersionBuilderTests
{
    private Mock<IVersionHistoryPath> _bestPath;
    private Mock<IGitTool> _git;
    private Mock<IGitOutputs> _gitOutputs;
    private Mock<ICommit> _headCommit;
    private Mock<IBuildHost> _host;
    private Mock<IVersionGeneratorInputs> _inputs;
    private NUnitLogger _logger;
    private Mock<IVersionOutputs> _outputs;
    private Mock<IHistoryPaths> _paths;
    private DefaultVersionBuilder _target;
    private SemVersion _version = null!;
    private CommitObfuscator _obfuscator;

    [SetUp]
    public void SetUp()
    {
        _logger = new NUnitLogger(false)
        {
            Level = LoggingLevel.Debug
        };
        _bestPath = new Mock<IVersionHistoryPath>();
        _paths = new Mock<IHistoryPaths>();
        _paths.Setup(x => x.BestPath).Returns(_bestPath.Object);

        _target = new DefaultVersionBuilder(_paths.Object, _logger);

        _host = new Mock<IBuildHost>();
        _inputs = new Mock<IVersionGeneratorInputs>();
        _headCommit = new Mock<ICommit>();
        _gitOutputs = new Mock<IGitOutputs>();
        _outputs = new Mock<IVersionOutputs>();
        _git = new Mock<IGitTool>();
        _obfuscator = new CommitObfuscator();

        _host.Setup(x => x.BuildNumber).Returns("BUILD_NUMBER");
        _host.Setup(x => x.BuildContext).Returns("BUILD_CONTEXT");
        _host.Setup(x => x.BuildId).Returns(["77"]);
        _inputs.Setup(x => x.WorkingDirectory).Returns("WorkingDirectory");
        _headCommit.Setup(x => x.CommitId).Returns(new CommitId("001", _obfuscator));
        _gitOutputs.Setup(x => x.HeadCommit).Returns(_headCommit.Object);
        _outputs.Setup(x => x.Git).Returns(_gitOutputs.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }

    [TestCase("0.1.0", "main", "InitialDev")]
    [TestCase("0.5.1", "release", "InitialDev")]
    [TestCase("0.5.1", "release/anything", "InitialDev")]
    [TestCase("0.5.1", "Release/anything", "InitialDev")]
    [TestCase("0.5.1", "JohnsOwnBranch", "alpha-InitialDev")]
    [TestCase("0.5.1", "devs/JohnsOwnBranch", "alpha-InitialDev")]
    [TestCase("0.999.0", "feature", "beta-InitialDev")]
    [TestCase("0.5.1", "Feature", "beta-InitialDev")]
    [TestCase("99.5.1", "Feature", "beta")]
    [TestCase("0.1.0", "features/mine", "beta-InitialDev")]
    [TestCase("1.0.0", "features/mine", "beta")]
    [TestCase("1.0.0", "release/rc", "rc")]
    [TestCase("1.0.0", "release/gold/rc", "rc")]
    [TestCase("1.0.0", "release/gold/rc1", "rc")]
    [TestCase("1.0.0", "release/gold/rc_one", "rc")]
    public void PrereleaseBuildTest(string version, string branchName, string expectedPrereleaseLabel)
    {
        SetupInputs(version, branchName);

        _target.Build(_host.Object, _git.Object, _inputs.Object, _outputs.Object);

        var expectedVersion = _version.WithPrerelease(expectedPrereleaseLabel, "77")
                                      .WithMetadata(branchName.ToNormalisedSemVerIdentifier(), "001");
        _outputs.VerifySet(x => x.BuildSystemVersion = expectedVersion.WithoutMetadata(), Times.Once);
        _outputs.Verify(x => x.SetAllVersionPropertiesFrom(expectedVersion,
                                                           "BUILD_NUMBER",
                                                           "BUILD_CONTEXT"));
    }

    [TestCase("1.0.0", "main")]
    [TestCase("1.0.1", "release")]
    [TestCase("9999.999.999", "release/anything")]
    [TestCase("1.2.3", "Release/anything")]
    public void ReleaseBuildTest(string version, string branchName)
    {
        SetupInputs(version, branchName);

        _target.Build(_host.Object, _git.Object, _inputs.Object, _outputs.Object);

        _outputs.VerifySet(x => x.BuildSystemVersion = _version, Times.Once);
    }

    private void SetupInputs(string version, string branchName)
    {
        _version = SemVersion.Parse(version, SemVersionStyles.Strict);
        _bestPath.Setup(x => x.Version).Returns(_version);
        _gitOutputs.Setup(x => x.BranchName).Returns(branchName);
    }
}