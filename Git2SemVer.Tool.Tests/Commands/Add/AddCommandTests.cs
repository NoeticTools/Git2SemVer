using NoeticTools.Git2SemVer.Tool.Commands.Add;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NoeticTools.Common;
using NoeticTools.Common.Logging;
using NoeticTools.Common.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Tool.Framework;
using NoeticTools.Git2SemVer.Tool.MSBuild.Projects;
using NoeticTools.Git2SemVer.Tool.MSBuild.Solutions;
using NoeticTools.Testing.Common;


namespace NoeticTools.Git2SemVer.Tool.Tests.Commands.Add;

[TestFixture]
internal class AddCommandTests
{
    private NUnitTaskLogger _logger;
    private Mock<ISolutionFinder> _solutionFinder;
    private Mock<IUserOptionsPrompt> _userOptionsPrompt;
    private Mock<IDotNetTool> _dotNetTool;
    private Mock<IEmbeddedResources<Git2SemverEmbeddedResources>> _embeddedResources;
    private Mock<IProjectDocumentReader> _projectReader;
    private Mock<IAddPreconditionValidator> _addPreconditionValidator;
    private Mock<IConsoleIO> _consoleIO;
    private AddCommand _target;

    [SetUp]
    public void SetUp()
    {
        _logger = new NUnitTaskLogger() { Level = LoggingLevel.Trace };
        _solutionFinder = new Mock<ISolutionFinder>();
        _userOptionsPrompt = new Mock<IUserOptionsPrompt>();
        _dotNetTool = new Mock<IDotNetTool>();
        _embeddedResources = new Mock<IEmbeddedResources<Git2SemverEmbeddedResources>>();
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

    [Test]
    public void Test()
    {
    }
}

