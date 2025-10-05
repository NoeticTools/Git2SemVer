using Moq;
using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Core.Tools;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Testing.Core;
using NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Remove;
using NoeticTools.Git2SemVer.Tool.MSBuild.Solutions;


namespace NoeticTools.Git2SemVer.Tool.Tests.Commands.Remove;

[TestFixture]
internal class RemoveCommandTests
{
    private Mock<IConsoleIO> _consoleIO;
    private Mock<IContentEditor> _contentEditor;
    private Mock<IDotNetTool> _dotNetTool;
    private NUnitLogger _logger;
    private Mock<ISolutionFinder> _solutionFinder;
    private RemoveCommand _target;

    [Test]
    public void CanConstructTest()
    {
        Assert.That(_target, Is.Not.Null);
    }

    [SetUp]
    public void SetUp()
    {
        _logger = new NUnitLogger();
        _solutionFinder = new Mock<ISolutionFinder>();
        _dotNetTool = new Mock<IDotNetTool>();
        _consoleIO = new Mock<IConsoleIO>();
        _contentEditor = new Mock<IContentEditor>();

        _target = new RemoveCommand(_solutionFinder.Object,
                                    _dotNetTool.Object,
                                    _consoleIO.Object,
                                    _contentEditor.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }
}