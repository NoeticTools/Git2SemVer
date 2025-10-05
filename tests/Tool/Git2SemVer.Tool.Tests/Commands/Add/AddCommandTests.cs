using Moq;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Testing.Core;
using NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Add;
using NoeticTools.Git2SemVer.Tool.MSBuild.Projects;
using NoeticTools.Git2SemVer.Tool.MSBuild.Solutions;


namespace NoeticTools.Git2SemVer.Tool.Tests.Commands.Add;

[TestFixture]
internal class AddCommandTests
{
    private Mock<IAddPreconditionValidator> _addPreconditionValidator;
    private Mock<IConsoleIO> _consoleIO;
    private Mock<IDotNetTool> _dotNetTool;
    private Mock<IEmbeddedResources> _embeddedResources;
    private NUnitLogger _logger;
    private Mock<IProjectDocumentReader> _projectReader;
    private Mock<ISolutionFinder> _solutionFinder;
    private AddCommand _target;
    private Mock<IUserOptionsPrompt> _userOptionsPrompt;

    [Test]
    public void CanConstructTest()
    {
        Assert.That(_target, Is.Not.Null);
    }

    [SetUp]
    public void SetUp()
    {
        _logger = new NUnitLogger { Level = LoggingLevel.Trace };
        _solutionFinder = new Mock<ISolutionFinder>();
        _userOptionsPrompt = new Mock<IUserOptionsPrompt>();
        _dotNetTool = new Mock<IDotNetTool>();
        _embeddedResources = new Mock<IEmbeddedResources>();
        _projectReader = new Mock<IProjectDocumentReader>();
        _addPreconditionValidator = new Mock<IAddPreconditionValidator>();
        _consoleIO = new Mock<IConsoleIO>();

        _target = new AddCommand(_solutionFinder.Object,
                                 _userOptionsPrompt.Object,
                                 _dotNetTool.Object,
                                 _embeddedResources.Object,
                                 _projectReader.Object,
                                 _addPreconditionValidator.Object,
                                 _consoleIO.Object,
                                 _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }
}