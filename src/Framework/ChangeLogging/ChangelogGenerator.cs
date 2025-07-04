﻿using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.ChangeLogging.Exceptions;
using NoeticTools.Git2SemVer.Framework.Generation;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using Scriban;
using Semver;
using System.Text.RegularExpressions;


namespace NoeticTools.Git2SemVer.Framework.ChangeLogging;

public class ChangelogGenerator(ChangelogSettings config)
{
    /// <summary>
    ///     Generate a new changelog document.
    /// </summary>
    /// <param name="releaseUrl"></param>
    /// <param name="versioning"></param>
    /// <param name="contributing"></param>
    /// <param name="scribanTemplate"></param>
    /// <param name="incremental"></param>
    /// <returns></returns>
    public string Create(string releaseUrl,
                         IVersionOutputs versioning,
                         ContributingCommits contributing,
                         string scribanTemplate,
                         bool incremental)
    {
        Git2SemVerArgumentException.ThrowIfNull(releaseUrl, nameof(releaseUrl));
        Git2SemVerArgumentException.ThrowIfNullOrEmpty(scribanTemplate, nameof(scribanTemplate));

        var version = versioning.Version!;
        var changes = GetChanges(version, contributing);
        return Create(releaseUrl, contributing, scribanTemplate, incremental, version, changes);
    }

    private static string Create(string releaseUrl, ContributingCommits contributing, string scribanTemplate, bool incremental, SemVersion version,
                                 IReadOnlyList<CategoryChanges> changes)
    {
        var model = new ChangelogModel(version,
                                       contributing,
                                       changes,
                                       releaseUrl,
                                       incremental,
                                       createNewDocument: true);
        try
        {
            var template = Template.Parse(scribanTemplate);
            return template.Render(model, member => member.Name);
        }
        catch (Exception exception)
        {
            throw new Git2SemVerScribanFileParsingException("There was a problem parsing or rendering a Scriban template file.", exception);
        }
    }

    public string Update(string releaseUrl,
                         IVersionOutputs versioning,
                         ContributingCommits contributing,
                         string scribanTemplate,
                         string changelogToUpdate,
                         bool forceUpdate = false)
    {
        Git2SemVerArgumentException.ThrowIfNull(releaseUrl, nameof(releaseUrl));
        Git2SemVerArgumentException.ThrowIfNullOrEmpty(scribanTemplate, nameof(scribanTemplate));
        Git2SemVerArgumentException.ThrowIfNullOrEmpty(scribanTemplate, nameof(changelogToUpdate));

        var version = versioning.Version!;
        var changes = GetChanges(version, contributing); // todo - trim changes to new changes only
        if (!forceUpdate && changes.Count == 0)
        {
            return changelogToUpdate;
        }

        var createdContent = Create(releaseUrl, contributing, scribanTemplate, true, version, changes);
        var sourceDocument = new ChangelogDocument("generated", createdContent);
        var destinationDocument = new ChangelogDocument("existing", changelogToUpdate);

        foreach (var category in changes)
        {
            // todo - what if category not found in file?
            var sourceContent = sourceDocument[category.Settings.ChangeType + " changes"].Content;
            destinationDocument[category.Settings.ChangeType + " changes to review"].Content += sourceContent;
        }

        destinationDocument["version"].Content = sourceDocument["version"].Content;

        return destinationDocument.Content;
    }

    private static List<Commit> Extract(List<Commit> remainingCommits, string changeType)
    {
        var regex = new Regex(changeType);
        var extracted = remainingCommits.Where(x => regex.IsMatch(x.MessageMetadata.ChangeTypeText)).ToList();
        extracted.ForEach(x => remainingCommits.Remove(x));
        return extracted;
    }

    private static CategoryChanges? GetCategoryChanges(ChangelogCategorySettings categorySettings, List<Commit> remainingCommits, bool isRelease)
    {
        var commits = Extract(remainingCommits, categorySettings.ChangeType);
        if ((categorySettings.SkipIfNone && commits.Count <= 0) || (isRelease && !categorySettings.SkipIfRelease))
        {
            return null;
        }

        var categoryChanges = new CategoryChanges(categorySettings);
        categoryChanges.AddRange(GetUniqueChangelogEntries(commits));
        return categoryChanges;
    }

    private IReadOnlyList<CategoryChanges> GetChanges(SemVersion version, ContributingCommits contributing)
    {
        var remainingCommits = new List<Commit>(contributing.Commits.Where(x => x.MessageMetadata.ApiChangeFlags.Any));
        var orderedCategories = config.Categories.OrderBy(x => x.Order);
        return orderedCategories.Select(category => GetCategoryChanges(category, remainingCommits, version!.IsRelease)).OfType<CategoryChanges>()
                                .ToList();
    }

    private static IReadOnlyList<ChangeLogEntry> GetUniqueChangelogEntries(IReadOnlyList<Commit> commits)
    {
        var changeEntries = new List<ChangeLogEntry>();
        foreach (var commit in commits)
        {
            // >>> todo - incremental change. reject change if commitId is recorded in LastRun. Add commits to ChangeLogEntry

            var entry = changeEntries.SingleOrDefault(x => x.Equals(commit.MessageMetadata));
            if (entry == null)
            {
                entry = new ChangeLogEntry(commit.MessageMetadata);
                changeEntries.Add(entry);
            }
            else
            {
                // todo - incremental update will need to check issues count
                entry.AddIssues(commit.MessageMetadata.FooterKeyValues["issues"]);
            }

            entry.AddCommitId(commit.CommitId.ShortSha);
        }

        return changeEntries;
    }
}