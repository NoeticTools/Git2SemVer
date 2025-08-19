// ReSharper disable UnusedMember.Global

using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

/// <summary>
///     Change category (like 'Added' or 'Fixed') that appears in the changelog.
/// </summary>
/// <param name="settings"></param>
/// <param name="markdownIssueFormatter"></param>
public sealed class ChangeCategory(CategorySettings settings, ITextFormatter markdownIssueFormatter)
{
    private readonly List<ChangeLogEntry> _changes = [];
    private readonly Regex _changeTypeRegex = new(settings.ChangeTypePattern);

    public IReadOnlyList<ChangeLogEntry> Changes => _changes;

    public CategorySettings Settings { get; } = settings;

    public void ExtractChangeLogsFrom(List<ConventionalCommit> metatdata)
    {
        var matchingMetadata = metatdata.Where(Matches).ToList();
        matchingMetadata.ForEach(x => metatdata.Remove(x));
        AddRange(GetUniqueChangelogEntries(matchingMetadata));
    }

    private void AddRange(IReadOnlyList<ChangeLogEntry> changes)
    {
        _changes.AddRange(changes);
    }

    private IReadOnlyList<ChangeLogEntry> GetUniqueChangelogEntries(List<ConventionalCommit> metadata)
    {
        var changeLogEntries =
            new ChangeLookup<ChangeLogEntry>(logEntry => logEntry.MessageMetadata);
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

    private bool Matches(IChangeTypeAndDescription messageMetadata)
    {
        return _changeTypeRegex.IsMatch(messageMetadata.ChangeType);
    }
}