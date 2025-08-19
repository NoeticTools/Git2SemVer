using NoeticTools.Git2SemVer.Core.ConventionCommits;


// ReSharper disable UnusedMember.Global

namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

public sealed class ChangeLogEntry
{
    private readonly List<string> _issues = [];
    private readonly ITextFormatter _markdownIssueFormatter;

    public ChangeLogEntry(ConventionalCommit messageMetadata, ITextFormatter markdownIssueFormatter)
    {
        _markdownIssueFormatter = markdownIssueFormatter;
        MessageMetadata = messageMetadata;
        TryAddIssues(messageMetadata.Issues);
    }

    public string Description => MessageMetadata.Description;

    // ReSharper disable once MemberCanBePrivate.Global
    public IReadOnlyList<string> Issues => _issues;

    /// <summary>
    ///     Issues as Markdown links when issue url is provided.
    /// </summary>
    public IReadOnlyList<string> IssuesMarkdown => Issues.Select(x => _markdownIssueFormatter.Format(x)).ToList();

    public ConventionalCommit MessageMetadata { get; }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is ChangeLogEntry other && Equals(other));
    }

    public override int GetHashCode()
    {
        return MessageMetadata.GetHashCode();
    }

    public void TryAddIssues(IEnumerable<string> issueIds)
    {
        foreach (var issueId in issueIds)
        {
            TryAddIssue(issueId);
        }
    }

    private bool Equals(ChangeLogEntry other)
    {
        return GetHashCode() == other.GetHashCode();
    }

    private bool TryAddIssue(string issueId)
    {
        if (_issues.Contains(issueId))
        {
            return false;
        }

        _issues.Add(issueId);
        return true;
    }
}