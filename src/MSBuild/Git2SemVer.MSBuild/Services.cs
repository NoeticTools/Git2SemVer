using Microsoft.Build.Utilities;
using Microsoft.Extensions.DependencyInjection;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.ChangeLogging.Task;
using NoeticTools.Git2SemVer.Framework.Framework.Config;
using NoeticTools.Git2SemVer.Framework.Versioning;
using NoeticTools.Git2SemVer.Framework.Versioning.Builders.Scripting;
using NoeticTools.Git2SemVer.MSBuild.Tasks;


// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.MSBuild;

[RegisterSingleton]
internal sealed class Services
{
    public static IServiceProvider ConfigureServices(Git2SemVerGenerateVersionTask task,
                                                     ILogger logger,
                                                     TaskLoggingHelper taskLogging)
    {
        var services = new ServiceCollection();

        services.AddSingleton(logger);
        services.AddSingleton<IVersionGeneratorInputs>(_ => task);
        services.AddSingleton<IChangeLogGeneratorTaskOptions>(_ => task);
        services.AddSingleton<IMSBuildGlobalProperties>(_ => new MSBuildGlobalProperties(task.BuildEngine6));
        services.AddSingleton(_ => Git2SemVerLocalSettings.Load());
        services.AddSingleton(_ => new MSBuildTeamCityWriterFactory(taskLogging).Create());

        services.AddNoeticToolsGit2SemVerCore();
        services.AddNoeticToolsGit2SemVerFramework();

        return services.BuildServiceProvider();
    }
}