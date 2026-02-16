using System.Diagnostics;
using NoeticTools.Git2SemVer.Core.Console;
using NoeticTools.Git2SemVer.Core.Diagnostics;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.ChangeLogging;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;


namespace NoeticTools.Git2SemVer.Tool.CommandLine.Changelog;

[RegisterSingleton]
internal sealed class ChangelogCommand(IConsoleIO console, ILogger logger) : CommandBase(console), IChangelogCommand
{
    public void Execute(ChangelogCommandSettings cmdLineSettings)
    {
        var proceed = WriteConsolePreamble(cmdLineSettings);
        if (!proceed)
        {
            Console.WriteErrorLine("Aborted.");
        }

        var stopwatch = Stopwatch.StartNew();

        EnsureDataDirectoryExists(cmdLineSettings);
        var projectSettings = GetProjectSettings(cmdLineSettings);
        var changeLogInputs = RunVersionGenerator(cmdLineSettings, projectSettings);
        var createNewChangelog = !File.Exists(cmdLineSettings.OutputFilePath);
        var lastRunData = createNewChangelog ? new LastRunData() : GetLastRunData(cmdLineSettings);
        var template = GetTemplate(cmdLineSettings.DataDirectory);
        var releaseUrl = cmdLineSettings.ArtifactLinkPattern;
        var changelogGenerator = new ChangelogGenerator(projectSettings, logger);
        var existingChangelog = createNewChangelog ? "" : File.ReadAllText(cmdLineSettings.OutputFilePath);

        var changelog = changelogGenerator.Execute(changeLogInputs,
                                                   lastRunData,
                                                   template,
                                                   existingChangelog,
                                                   releaseUrl,
                                                   cmdLineSettings.ReleaseAs);

        if (string.Equals(existingChangelog, changelog, StringComparison.Ordinal))
        {
            Console.WriteMarkupInfoLine("No updates found.");
        }

        var verb = !createNewChangelog ? "Updated" : "Created";
        if (cmdLineSettings.WriteToConsole)
        {
            Console.WriteLine($"\n{verb} changelog:");
            Console.WriteHorizontalLine();
            Console.WriteCodeLine(changelog.TrimEnd());
            Console.WriteHorizontalLine();
        }

        if (cmdLineSettings.OutputFilePath.Length == 0)
        {
            Console.WriteLine();
            Console.WriteMarkupDebugLine("Write changelog to file is disabled as the file output path is an empty string.");
            return;
        }

        lastRunData.Update(changeLogInputs);
        lastRunData.ForcedReleasedTitle = cmdLineSettings.ReleaseAs;
        lastRunData.Save(cmdLineSettings.DataDirectory, cmdLineSettings.OutputFilePath);

        Console.WriteLine();
        Console.WriteMarkupInfoLine($"{verb} changelog file: {cmdLineSettings.OutputFilePath}");
        File.WriteAllText(cmdLineSettings.OutputFilePath, changelog);

        projectSettings.Save(Path.Combine(cmdLineSettings.DataDirectory, ChangelogConstants.ProjectSettingsFilename));

        stopwatch.Stop();

        Console.WriteLine("");
        Console.WriteMarkupLine($"[good]Completed[/] (in {stopwatch.ElapsedMilliseconds:D0} ms)");
    }

    private void EnsureDataDirectoryExists(ChangelogCommandSettings cmdLineSettings)
    {
        var dataDirectory = cmdLineSettings.DataDirectory;
        // ReSharper disable once InvertIf
        if (dataDirectory.Length > 0)
        {
            if (Directory.Exists(dataDirectory))
            {
                return;
            }

            logger.LogDebug("Creating data directory '{0}'", dataDirectory);
            Directory.CreateDirectory(dataDirectory);
        }
    }

    private LastRunData GetLastRunData(ChangelogCommandSettings cmdLineSettings)
    {
        var data = LastRunData.Load(cmdLineSettings.DataDirectory, cmdLineSettings.OutputFilePath);
        if (data.NoData)
        {
            logger.LogWarning(new GSV201(cmdLineSettings.DataDirectory, cmdLineSettings.OutputFilePath));
        }

        return data;
    }

    private static ChangelogLocalSettings GetProjectSettings(ChangelogCommandSettings commandSettings)
    {
        var filePath = Path.Combine(commandSettings.DataDirectory, ChangelogConstants.ProjectSettingsFilename);

        if (File.Exists(filePath))
        {
            return ChangelogLocalSettings.Load(filePath);
        }

        var config = new ChangelogLocalSettings
        {
            Categories = ChangelogConstants.DefaultCategories
        };
        config.Save(filePath);
        return config;
    }

    private string GetTemplate(string directory)
    {
        var templatePath = Path.Combine(directory, ChangelogConstants.DefaultMarkdownTemplateFilename);
        if (File.Exists(templatePath))
        {
            return File.ReadAllText(templatePath);
        }

        Console.WriteMarkupDebugLine($"Creating default template file: {templatePath}");
        var defaultTemplate = ChangelogConstants.GetDefaultTemplate();
        File.WriteAllText(templatePath, defaultTemplate);
        return defaultTemplate;
    }

    private ConventionalCommitsVersionInfo RunVersionGenerator(ChangelogCommandSettings cmdLineSettings, ChangelogLocalSettings projectSettings)
    {
        var inputs = new VersionGeneratorInputs
        {
            VersioningMode = VersioningMode.StandAloneProject,
            IntermediateOutputDirectory = cmdLineSettings.DataDirectory,
            HostType = cmdLineSettings.HostType ?? "",
            WriteConventionalCommitsInfo = false
        };

        using var logger = CreateLogger(cmdLineSettings.Verbosity);
        var host = GetBuildHost(logger, inputs);
        var versionGenerator = new VersioningEngineFactory(logger).Create(inputs,
                                                                          new NullJsonFileIO(),
                                                                          host,
                                                                          projectSettings.ConvCommits);
        return versionGenerator.GetConventionalCommitsInfo();
    }

    private bool WriteConsolePreamble(ChangelogCommandSettings cmdLineSettings)
    {
        Console.WriteMarkupInfoLine($"Generating Changelog {(cmdLineSettings.Unattended ? " (unattended)" : "")}.");
        Console.WriteLine("");

        var proceed = Console.PromptYesNo("Proceed?");
        Console.WriteLine();
        return proceed;
    }
}