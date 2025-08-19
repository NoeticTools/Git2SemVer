using System.Text.Json.Serialization;
using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

public sealed class HandledChange : IChangeTypeAndDescription
{
    [JsonPropertyOrder(10)]
    public string ChangeType { get; set; } = string.Empty;

    [JsonPropertyOrder(20)]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyOrder(30)]
    public List<string> Issues { get; set; } = [];

    [JsonPropertyOrder(15)]
    public string Scope { get; set; } = string.Empty;

    public bool TryAddIssues(IEnumerable<string> issues)
    {
        var newIssues = issues.Where(x => !Issues.Contains(x)).ToList();
        Issues.AddRange(newIssues);
        return newIssues.Count > 0;
    }
}