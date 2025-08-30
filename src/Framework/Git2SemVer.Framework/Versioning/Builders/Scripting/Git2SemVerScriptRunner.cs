using System.Diagnostics;
using Microsoft.Build.Utilities;
using NoeticTools.Git2SemVer.Core.Tools.DotnetCli;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Tools.CI;
using NuGet.Versioning;
using Semver;
using ILogger = NoeticTools.Git2SemVer.Core.Logging.ILogger;
using Task = System.Threading.Tasks.Task;


namespace NoeticTools.Git2SemVer.Framework.Versioning.Builders.Scripting;

public sealed class Git2SemVerScriptRunner
{
    private readonly CSharpScriptRunner _innerScriptRunner;
    private readonly ILogger _logger;

    internal Git2SemVerScriptRunner(CSharpScriptRunner innerScriptRunner,
                                    ILogger logger)
    {
        _innerScriptRunner = innerScriptRunner;
        _logger = logger;
    }

    internal static IReadOnlyList<Type> MetadataReferences { get; } =
    [
        typeof(TaskLoggingHelper),
        typeof(DotNetTool),
        typeof(UncontrolledHost),
        typeof(IBuildHost),
        typeof(GitTool),
        typeof(SemVersion),
        typeof(NuGetVersion),
        typeof(VersioningContext),
        typeof(ILogger)
    ];

    internal async Task RunScript(object globalContext, string scriptPath)
    {
        var stopwatch = Stopwatch.StartNew();

        var inMemoryTypes = new List<Type>(
        [
            typeof(VersioningContext),
            typeof(ILogger),
            typeof(SemVersion)
        ]);

        await _innerScriptRunner.RunScript(globalContext, scriptPath, MetadataReferences, inMemoryTypes)
                                .ConfigureAwait(true);

        stopwatch.Stop();
        _logger.LogDebug($"Script run completed (in {stopwatch.Elapsed.TotalSeconds:F1} seconds).");
    }
}