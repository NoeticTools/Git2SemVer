using Injectio.Attributes;
using Microsoft.Extensions.DependencyInjection;
using NoeticTools.Common.Logging;


// ReSharper disable ClassNeverInstantiated.Global

namespace NoeticTools.Git2SemVer.Tool;

[RegisterSingleton]
internal sealed class Services
{
    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var logger = new FileLogger(GetLogFilePath());
        services.AddSingleton<ILogger>(logger);

        services.AddNoeticToolsGit2SemVerTool();
        services.AddNoeticToolsCommon();

        return services.BuildServiceProvider();
    }

    private static string GetLogFilePath()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Git2SemVer");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return Path.Combine(folderPath, "Git2SemVer.Tool.log");
    }

}