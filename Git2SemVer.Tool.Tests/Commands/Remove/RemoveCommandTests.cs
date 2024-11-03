﻿using Moq;
using NoeticTools.Common.Tools;
using NoeticTools.Common.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Tool.Commands.Add;
using NoeticTools.Git2SemVer.Tool.Commands.Remove;
using NoeticTools.Git2SemVer.Tool.Framework;
using NoeticTools.Git2SemVer.Tool.MSBuild.Solutions;
using NoeticTools.Testing.Common;


namespace NoeticTools.Git2SemVer.Tool.Tests.Commands.Remove;

[TestFixture]
internal class RemoveCommandTests
{
    private Mock<IConsoleIO> _consoleIO;
    private Mock<IContentEditor> _contentEditor;
    private Mock<IDotNetTool> _dotNetTool;
    private NUnitTaskLogger _logger;
    private Mock<ISolutionFinder> _solutionFinder;
    private RemoveCommand _target;
    private Mock<IUserOptionsPrompt> _userOptionsPrompt;

    [SetUp]
    public void SetUp()
    {
        _logger = new NUnitTaskLogger();
        _solutionFinder = new Mock<ISolutionFinder>();
        _userOptionsPrompt = new Mock<IUserOptionsPrompt>();
        _dotNetTool = new Mock<IDotNetTool>();
        _consoleIO = new Mock<IConsoleIO>();
        _contentEditor = new Mock<IContentEditor>();

        _target = new RemoveCommand(_solutionFinder.Object,
                                    _userOptionsPrompt.Object,
                                    _dotNetTool.Object,
                                    _consoleIO.Object,
                                    _contentEditor.Object,
                                    _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }
}