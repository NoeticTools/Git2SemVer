using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.FileSystem;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Framework.ChangeLogging.Exceptions;
using Scriban;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

[RegisterTransient]
public class ChangelogGenerator(IChangelogSettings settings, ILogger logger)
{
    /// <summary>
    ///     Generate or update changelog document.
    /// </summary>
    /// <param name="versioning"></param>
    /// <param name="releaseUrl"></param>
    /// <param name="releaseAs"></param>
    /// <param name="dataDirectory"></param>
    /// <param name="outputFilePath"></param>
    /// <param name="workingDirectory"></param>
    /// <returns>
    ///     Created or updated changelog content.
    /// </returns>
    public string Execute(VersioningOutputs versioning,
                          string releaseUrl,
                          string releaseAs,
                          DirectoryPath dataDirectory,
                          FilePath outputFilePath,
                          DirectoryPath workingDirectory)
    {
        releaseUrl = GetFirstNonEmptyOption(releaseUrl,
                                            settings.ArtifactLinkPattern,
                                            ChangelogConstants.DefaultArtifactLinkPattern);

        dataDirectory = dataDirectory.ToAbsolute(settings.DataDirectory, workingDirectory);
        outputFilePath = outputFilePath.ToAbsolute(settings.OutputFilePath, workingDirectory);

        var createNewChangelog = !outputFilePath.Exists();
        var changelogToUpdate = createNewChangelog ? "" : outputFilePath.ReadAllText();

        var lastRunData = createNewChangelog ? new LastRunData() : LastRunData.Load(dataDirectory, outputFilePath, logger);
        var scribanTemplate = new ChangelogTemplateReader(logger).Load(dataDirectory);

        var conventionalCommitsVersionInfo = new ConventionalCommitsVersionInfo(versioning.Versions, versioning.Metadata.Contributing);
        var changelog = BuildChangelogContent(conventionalCommitsVersionInfo, scribanTemplate, releaseUrl, releaseAs, lastRunData, changelogToUpdate);

        if (outputFilePath.IsEmpty)
        {
            return changelog;
        }

        lastRunData.Update(conventionalCommitsVersionInfo);
        lastRunData.ForcedReleasedTitle = releaseAs;
        lastRunData.Save(dataDirectory, outputFilePath);

        outputFilePath.WriteAllText(changelog);

        return changelog;
    }

    private string BuildChangelogContent(ConventionalCommitsVersionInfo convCommits,
                                         string scribanTemplate,
                                         string releaseUrl,
                                         string releaseAs,
                                         LastRunData lastRunData,
                                         string changelogToUpdate)
    {
        var contributingReleases = convCommits.ContributingReleases.Select(x => SemVersion.Parse(x, SemVersionStyles.Strict)).ToArray();
        var addNewRelease = lastRunData.ContributingReleasesChanged(contributingReleases);
        if (addNewRelease)
        {
            logger.LogInfo("New release.");
            lastRunData = new LastRunData();
        }

        var messagesWithChanges = GetUnhandledChanges(convCommits.ConventionalCommits, lastRunData.HandledChanges);

        var issueMarkdownFormatter = new MarkdownLinkFormatter(settings.IssueLinkFormat);
        var orderedCategories = settings.Categories.OrderBy(x => x.Order);
        var changeCategories = orderedCategories.Select(category => ExtractChangeCategory(category, messagesWithChanges, issueMarkdownFormatter))
                                                .ToList();
        if (changelogToUpdate.Length > 0 && changeCategories.Count == 0)
        {
            return changelogToUpdate;
        }

        var newChangesContent = RenderContent(convCommits, scribanTemplate, releaseUrl, releaseAs, changeCategories);
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
            destinationDocument.AppendChanges(changeCategories, newChanges);
        }

        return destinationDocument.Content;
    }

    private static ChangeCategory ExtractChangeCategory(CategorySettings categorySettings,
                                                        List<ConventionalCommit> changeMessages,
                                                        ITextFormatter markdownIssueFormatter)
    {
        var changeCategory = new ChangeCategory(categorySettings, markdownIssueFormatter);
        changeCategory.ExtractChangeLogsFrom(changeMessages);
        return changeCategory;
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
    private static List<ConventionalCommit> GetUnhandledChanges(IReadOnlyList<ConventionalCommit> changeMessages,
                                                                List<HandledChange> handledChanges)
    {
        var unhandledMessages = new List<ConventionalCommit>(changeMessages);
        var handledChangesLookup = new ChangeLookup<HandledChange>(handledChanges, v => v);
        foreach (var changeMessage in unhandledMessages.ToArray())
        {
            if (handledChangesLookup.TryGet(changeMessage, out var handledChange))
            {
                if (!handledChange!.TryAddIssues(changeMessage.Issues))
                {
                    unhandledMessages.Remove(changeMessage);
                }
            }
            else
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

    private static string ToAbsolutePath(string path, string defaultPath, string workingDirectory)
    {
        if (path.Length == 0)
        {
            path = defaultPath;
        }

        if (defaultPath.Length > 0 && Path.IsPathRooted(path)) // todo - check if this is correct
        {
            return path;
        }

        return Path.Combine(workingDirectory, path);
    }
}