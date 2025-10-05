using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;


namespace NoeticTools.Git2SemVer.Core.IntegrationTests.Logging;

[TestFixture]
internal class FileLoggerIntegrationTests
{
    private string _outputFilePath;

    [Test]
    public void LogErrorException()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Debug })
        {
            logger.LogError(new InvalidOperationException("Exception 1"));
        }

        ValidateFileContents("""
                             ERROR | Exception - Exception 1
                                     Stack trace: 

                             """);
    }

    [Test]
    public void LoggerThrowsIOExceptionAfter3SecondsIfFileInUse()
    {
        using (File.OpenWrite(_outputFilePath))
        {
            var stopwatch = Stopwatch.StartNew();
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<IOException>(() => new FileLogger(_outputFilePath));
            stopwatch.Stop();
            Assert.That(stopwatch.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(3)));
        }
    }

    [Test]
    public void LogMessageGeneratorWithDebugThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Debug })
        {
            logger.LogDebug(() => "Line 1");
            logger.LogTrace(() => "Line 2");
        }

        ValidateFileContents("""
                             DEBUG | Line 1

                             """);
    }

    [Test]
    public void LogMessageGeneratorWithInfoThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Info })
        {
            logger.LogDebug(() => "Line 1");
            logger.LogTrace(() => "Line 2");
        }

        ValidateFileContents("""

                             """);
    }

    [Test]
    public void LogMessageGeneratorWithTraceThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Trace })
        {
            logger.LogDebug(() => "Line 1");
            logger.LogTrace(() => "Line 2");
        }

        ValidateFileContents("""
                             DEBUG | Line 1
                             TRACE | Line 2

                             """);
    }

    [Test]
    public void LogWarningExceptionWithErrorThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Error })
        {
            logger.LogWarning(new InvalidOperationException("Exception 1"));
        }

        ValidateFileContents("");
    }

    [Test]
    public void LogWarningExceptionWithWarnThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Warning })
        {
            logger.LogWarning(new InvalidOperationException("Exception 1"));
        }

        ValidateFileContents("""
                             WARN  | Exception - Exception 1

                             """);
    }

    [Test]
    public void LogWithArgumentsWithErrorThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Error })
        {
            logger.LogError("Line {0}", 1);
            logger.LogWarning("Line {0}", 2);
            logger.LogInfo("Line {0}", 3);
            logger.LogDebug("Line {0}", 4);
            logger.LogTrace("Line {0}", 5);
        }

        ValidateFileContents("""
                             ERROR | Line 1

                             """);
    }

    [Test]
    public void LogWithArgumentsWithInfoThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Info })
        {
            logger.LogError("Line {0}", 1);
            logger.LogWarning("Line {0}", 2);
            logger.LogInfo("Line {0}", 3);
            logger.LogDebug("Line {0}", 4);
            logger.LogTrace("Line {0}", 5);
        }

        ValidateFileContents("""
                             ERROR | Line 1
                             WARN  | Line 2
                             INFO  | Line 3

                             """);
    }

    [Test]
    public void LogWithArgumentsWithTraceThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Trace })
        {
            logger.LogError("Line {0}", 1);
            logger.LogWarning("Line {0}", 2);
            logger.LogInfo("Line {0}", 3);
            logger.LogDebug("Line {0}", 4);
            logger.LogTrace("Line {0}", 5);
        }

        ValidateFileContents("""
                             ERROR | Line 1
                             WARN  | Line 2
                             INFO  | Line 3
                             DEBUG | Line 4
                             TRACE | Line 5

                             """);
    }

    [Test]
    public void LogWithArgumentsWithWarningThresholdTest()
    {
        using (var logger = new FileLogger(_outputFilePath) { Level = LoggingLevel.Warning })
        {
            logger.LogError("Line {0}", 1);
            logger.LogWarning("Line {0}", 2);
            logger.LogInfo("Line {0}", 3);
            logger.LogDebug("Line {0}", 4);
            logger.LogTrace("Line {0}", 5);
        }

        ValidateFileContents("""
                             ERROR | Line 1
                             WARN  | Line 2

                             """);
    }

    [Test]
    public void ScopeIndentingIntegrationTest()
    {
        using (var logger = new FileLogger(_outputFilePath))
        {
            logger.LogInfo("First line");
            logger.LogInfo("Multi-line A:\nB\r\nC\rD");
            using (logger.EnterLogScope())
            {
                logger.LogDebug("Multi-line D:\n - E\r\n - F\r - G");
            }

            logger.LogError("   Leading spaces");
            logger.LogTrace("Last line");
        }

        ValidateFileContents("""
                             INFO  | First line
                             INFO  | Multi-line A:
                                     B
                                     C
                                     D
                             DEBUG |   Multi-line D:
                                        - E
                                        - F
                                        - G
                             ERROR |    Leading spaces
                             TRACE | Last line

                             """);
    }

    [SetUp]
    public void SetUp()
    {
        _outputFilePath = Path.GetTempFileName();
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_outputFilePath))
        {
            File.Delete(_outputFilePath);
        }
    }

    private void ValidateFileContents(string expected)
    {
        var result = File.ReadAllText(_outputFilePath);
        Assert.That(result, Is.EqualTo(expected));
    }
}