// ReSharper disable UnusedMember.Global

using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

/// <summary>
///     Change category (like 'Added' or 'Fixed') that appears in the changelog.
/// </summary>
/// <param name="settings"></param>
public sealed class ChangeCategory(CategorySettings settings)
{
    private readonly List<ChangeLogEntry> _changes = [];
    private readonly Regex _changeTypeRegex = new(settings.ChangeTypePattern);

    public IReadOnlyList<ChangeLogEntry> Changes => _changes;

    public CategorySettings Settings { get; } = settings;

    public void AddRange(IReadOnlyList<ChangeLogEntry> changes)
    {
        _changes.AddRange(changes);
    }

    public bool Matches(IChangeTypeAndDescription messageMetadata)
    {
        return _changeTypeRegex.IsMatch(messageMetadata.ChangeType);
    }
}