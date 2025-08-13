using Moq;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Versioning;
using NoeticTools.Git2SemVer.Testing.Core;


namespace NoeticTools.Git2SemVer.Framework.Tests.Versioning.ProjectVersioningTests;

internal abstract class ProjectVersioningUnitTestsBase
{
    private NUnitLogger _logger;

    [SetUp]
    public void SetUpBase()
    {
        Inputs = new Mock<IVersionGeneratorInputs>();
        Host = new Mock<IBuildHost>();
        OutputsCacheJsonFile = new Mock<IOutputsJsonIO>();
        VersionGenerator = new Mock<IVersioningEngine>();
        _logger = new NUnitLogger();

        Target = new ProjectVersioning(Inputs.Object, Host.Object, VersioningMode.StandAloneProject, OutputsCacheJsonFile.Object, VersionGenerator.Object, _logger);

        Inputs.Setup(x => x.SolutionSharedDirectory).Returns("SolutionSharedDirectory");
        Inputs.Setup(x => x.IntermediateOutputDirectory).Returns("IntermediateOutputDirectory");
        Inputs.Setup(x => x.BuildNumber).Returns("");

        LocalCachedOutputs = new Mock<IVersionOutputs>();
        SharedCachedOutputs = new Mock<IVersionOutputs>();

        OutputsCacheJsonFile.Setup(x => x.Load("IntermediateOutputDirectory")).Returns(LocalCachedOutputs.Object);
        OutputsCacheJsonFile.Setup(x => x.Load("SolutionSharedDirectory")).Returns(SharedCachedOutputs.Object);

        GeneratedOutputs = new Mock<IVersionOutputs>();
        VersionGenerator.Setup(x => x.PrebuildRun()).Returns(new VersioningOutputs(GeneratedOutputs.Object, null));
    }

    [TearDown]
    public void TearDown()
    {
        Target.Dispose();
        _logger.Dispose();
    }

    protected Mock<IVersionOutputs> GeneratedOutputs { get; private set; }

    protected Mock<IBuildHost> Host { get; private set; }

    protected Mock<IVersionGeneratorInputs> Inputs { get; private set; }

    protected Mock<IVersionOutputs> LocalCachedOutputs { get; private set; }

    protected Mock<IOutputsJsonIO> OutputsCacheJsonFile { get; private set; }

    protected Mock<IVersionOutputs> SharedCachedOutputs { get; private set; }

    protected ProjectVersioning Target { get; private set; }

    protected Mock<IVersioningEngine> VersionGenerator { get; private set; }

    protected void ModeIs(VersioningMode mode)
    {
        Inputs.Setup(x => x.VersioningMode).Returns(mode);
    }
}