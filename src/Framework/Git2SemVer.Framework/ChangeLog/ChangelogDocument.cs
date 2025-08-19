using NoeticTools.Git2SemVer.Core.Logging;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

internal sealed class ChangelogDocument(string name, string content, ILogger logger)
{
    public string Content { get; set; } = content;

    /// <summary>
    ///     The document's name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    ///     Inserts new release section above existing release section. Existing release content is moved to be below the next
    ///     release section.
    /// </summary>
    /// <param name="sourceDocument"></param>
    public void AddNewRelease(ChangelogDocument sourceDocument)
    {
        if (this[SectionNameConstants.Version].Content.Contains("Unreleased"))
        {
            logger.LogWarning("Inserting new release into changelog when current release is `Unreleased`.");
        }

        var nextReleaseSection = this[SectionNameConstants.NextRelease];
        var cleanedPriorContent = ChangelogSection.RemoveSectionMarkers(nextReleaseSection.Content);
        nextReleaseSection.AppendAfter(cleanedPriorContent);
        nextReleaseSection.Content = sourceDocument[SectionNameConstants.NextRelease].Content;
        this[SectionNameConstants.Version].Content = sourceDocument[SectionNameConstants.Version].Content;
    }

    public void AppendChanges(IEnumerable<ChangeCategory> changes,
                              ChangelogDocument sourceDocument)
    {
        foreach (var category in changes)
        {
            var sourceContent = sourceDocument[category.Settings.Name + SectionNameConstants.UngroomedChangesSuffix].Content;
            if (sourceContent.Trim().Length <= 0)
            {
                continue;
            }

            var destinationSection = this[category.Settings.Name + SectionNameConstants.UngroomedChangesSuffix];
            if (destinationSection.Exists)
            {
                destinationSection.Content += sourceContent;
            }
        }

        this[SectionNameConstants.Version].Content = sourceDocument[SectionNameConstants.Version].Content;
    }

    /// <summary>
    ///     Get section from document. Sections are marked by metadata within the document.
    /// </summary>
    /// <param name="name">Section name</param>
    /// <returns>Section object</returns>
    private ChangelogSection this[string name] => new(name, this);
}