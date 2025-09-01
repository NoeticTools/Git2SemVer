using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.ChangeLog.Exceptions;
using Scriban;
using Semver;
using Directory = NoeticTools.Git2SemVer.Core.FileSystem.Directory;
using File = NoeticTools.Git2SemVer.Core.FileSystem.File;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

[RegisterTransient]
public class ChangelogGenerator(ILogger logger)
{
    /// <summary>
    ///     Generate or update changelog document.
    /// </summary>
    /// <param name="versioning">Versioning information for the changelog generator to use.</param>
    /// <param name="dataDirectoryOption"></param>
    /// <param name="workingDirectory"></param>
    /// <param name="noFileWrites"></param>
    /// <param name="releaseUrlOption"></param>
    /// <param name="outputFilePathOption"></param>
    /// <param name="releaseAs"></param>
    /// <returns>
    ///     Created or updated changelog content.
    /// </returns>
    public string Execute(VersioningOutputs versioning,
                          Directory dataDirectoryOption,
                          Directory workingDirectory,
                          bool noFileWrites,
                          string releaseUrlOption = "",
                          File? outputFilePathOption = null,
                          string releaseAs = "")
    {
        var dataDirectory = dataDirectoryOption.ToAbsolute(ChangelogConstants.DefaultDataDirectory, workingDirectory);
        var settings = ChangelogSettings.Load(dataDirectory, ChangelogConstants.ProjectSettingsFilename);
        if (!settings.Enabled)
        {
            logger.LogDebug("Changelog generation skipped as it is not enabled in the settings.");
            return "";
        }

        releaseUrlOption = GetFirstNonEmptyOption(releaseUrlOption,
                                                  settings.ArtifactLinkPattern,
                                                  ChangelogConstants.DefaultArtifactLinkPattern);

        outputFilePathOption ??= File.Null;
        outputFilePathOption = outputFilePathOption.ToAbsolute(settings.OutputFilePath, workingDirectory);
        var createNewChangelog = !outputFilePathOption.Exists();
        var changelogToUpdate = createNewChangelog ? "" : outputFilePathOption.ReadAllText();

        var lastRunData = createNewChangelog ? new LastRunData() : LastRunData.Load(dataDirectory, outputFilePathOption, logger);
        var scribanTemplate = new ChangelogTemplateReader(logger).Load(dataDirectory);

        var conventionalCommitsVersionInfo = new ConventionalCommitsVersionInfo(versioning.Versions,
                                                                                versioning.Metadata.Contributing);
        var changelog = BuildChangelogContent(conventionalCommitsVersionInfo,
                                              scribanTemplate,
                                              releaseUrlOption,
                                              releaseAs,
                                              lastRunData,
                                              changelogToUpdate,
                                              settings);

        if (noFileWrites)
        {
            return changelog;
        }

        lastRunData.Update(conventionalCommitsVersionInfo);
        lastRunData.ForcedReleasedTitle = releaseAs;
        lastRunData.Save(dataDirectory, outputFilePathOption);

        outputFilePathOption.WriteAllText(changelog);

        return changelog;
    }

    private string BuildChangelogContent(ConventionalCommitsVersionInfo convCommits,
                                         string scribanTemplate,
                                         string releaseUrl,
                                         string releaseAs,
                                         LastRunData lastRunData,
                                         string changelogToUpdate,
                                         ChangelogSettings settings)
    {
        var contributingReleases = convCommits.ContributingReleases.Select(x => SemVersion.Parse(x, SemVersionStyles.Strict)).ToArray();
        var addNewRelease = lastRunData.ContributingReleasesChanged(contributingReleases);
        if (addNewRelease)
        {
            logger.LogInfo("New release.");
            lastRunData = new LastRunData();
        }

        var unhandledChanges = GetUnhandledChanges(convCommits.ContributingCommits, lastRunData.HandledChanges);

        var categorisedUnhandledChanges = Categorise(unhandledChanges, settings);
        if (changelogToUpdate.Length > 0 && categorisedUnhandledChanges.Count == 0)
        {
            return changelogToUpdate;
        }

        var newChangesContent = RenderContent(convCommits, scribanTemplate, releaseUrl, releaseAs, categorisedUnhandledChanges);
        if (changelogToUpdate.Length == 0)
        {
            return newChangesContent;
        }

        var newChanges = new ChangelogDocument("new_changes", newChangesContent, logger);

        var destinationDocument = new ChangelogDocument("existing", changelogToUpdate, logger);
        addNewRelease |= lastRunData.ForcedReleasedTitle.Length > 0 &&
                         !lastRunData.ForcedReleasedTitle.Equals(releaseAs, StringComparison.InvariantCulture);
        if (addNewRelease)
        {
            destinationDocument.AddNewRelease(newChanges);
        }
        else
        {
            destinationDocument.AppendChanges(categorisedUnhandledChanges, newChanges);
        }

        return destinationDocument.Content;
    }

    private static List<ChangeCategory> Categorise(IReadOnlyList<ConventionalCommit> unhandledChanges,
                                                   ChangelogSettings settings)
    {
        var remainingUnhandledChanges = new List<ConventionalCommit>(unhandledChanges);
        var issueFormatter = new MarkdownLinkFormatter(settings.IssueLinkFormat);
        var orderedCategories = settings.Categories.OrderBy(x => x.Order);
        return orderedCategories.Select(categorySettings =>
                                {
                                    var category = new ChangeCategory(categorySettings);
                                    var matchingChanges = remainingUnhandledChanges.Where(category.Matches).ToList();
                                    category.AddRange(GetUniqueChangelogEntries(matchingChanges, issueFormatter));
                                    foreach (var change in category.Changes)
                                    {
                                        remainingUnhandledChanges.Remove(change.MessageMetadata);
                                    }

                                    return category;
                                })
                                .ToList();
    }

    private static string GetFirstNonEmptyOption(params string[] prioritisedValues)
    {
        foreach (var prioritisedValue in prioritisedValues)
        {
            if (!string.IsNullOrEmpty(prioritisedValue))
            {
                return prioritisedValue;
            }
        }

        return "";
    }

    /// <summary>
    ///     Method to reduce metadata down to new change metadata only, and update the handled changes collection.
    /// </summary>
    /// <param name="changeMessages"></param>
    /// <param name="handledChanges"></param>
    private static IReadOnlyList<ConventionalCommit> GetUnhandledChanges(IReadOnlyList<ConventionalCommit> changeMessages,
                                                                         List<HandledChange> handledChanges)
    {
        var unhandledMessages = new List<ConventionalCommit>(changeMessages);
        var handledChangesLookup = new HandledChangeLookup(handledChanges);
        foreach (var changeMessage in changeMessages)
        {
            if (handledChangesLookup.TryGet(changeMessage, out var handledChange))
            {
                if (!handledChange!.TryAddIssues(changeMessage.Issues))
                {
                    unhandledMessages.Remove(changeMessage); // only unhandled if it contributes new issues
                }
            }
            else //xxxx // THIS METHOD IS DOING 2 THINGS - FINDING UNHANDLED CHANGES AND UPDATING HANDLED CHANGES
            {
                var newHandledChange = new HandledChange
                {
                    ChangeType = changeMessage.ChangeType,
                    Description = changeMessage.Description,
                    Issues = changeMessage.Issues.ToList()
                };
                handledChangesLookup.Add(newHandledChange);
                handledChanges.Add(newHandledChange);
            }
        }

        return unhandledMessages.OrderBy(x => x.Description).ToList();
    }

    private static IReadOnlyList<ChangeLogEntry> GetUniqueChangelogEntries(IReadOnlyList<ConventionalCommit> metadata,
                                                                           ITextFormatter markdownIssueFormatter)
    {
        var changeLogEntries = new ChangeLogEntryLookup();
        foreach (var metadataDatum in metadata)
        {
            if (!changeLogEntries.TryGet(metadataDatum, out var logEntry))
            {
                logEntry = new ChangeLogEntry(metadataDatum, markdownIssueFormatter);
                changeLogEntries.Add(logEntry);
            }

            logEntry!.TryAddIssues(metadataDatum.Issues);
        }

        return changeLogEntries.ToList();
    }

    private static string RenderContent(ConventionalCommitsVersionInfo inputs,
                                        string scribanTemplate,
                                        string releaseUrl,
                                        string releaseAs,
                                        IReadOnlyList<ChangeCategory> changeCategories)
    {
        var newChangesContent = "";
        try
        {
            var model = new ChangelogScribanModel(inputs,
                                                  changeCategories,
                                                  releaseUrl,
                                                  releaseAs);
            var template = Template.Parse(scribanTemplate);
            newChangesContent = template.Render(model, member => member.Name);
        }
        catch (Exception exception)
        {
            throw new Git2SemVerScribanFileParsingException("There was a problem parsing or rendering the Scriban template file.", exception);
        }

        if (newChangesContent.Trim().Length == 0)
        {
            throw new Git2SemVerScribanFileParsingException("The Scriban template must render content.");
        }

        return newChangesContent;
    }
}