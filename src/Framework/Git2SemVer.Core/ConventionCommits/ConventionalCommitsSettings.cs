using System.Text.Json.Serialization;


namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

/// <summary>
///     Conventional Commits settings.
/// </summary>
public sealed class ConventionalCommitsSettings : JsonSettingsFileBase<ConventionalCommitsSettings>, IEquatable<ConventionalCommitsSettings>
{
    /// <summary>
    ///     Commit message footer tokens that may contain a comma-delimited list of issues.
    /// </summary>
    [JsonPropertyOrder(20)]
    public string[] FooterIssueTokens { get; set; } =
    [
        "issues",
        "issue",
        "ref",
        "refs"
    ];

    public bool Equals(ConventionalCommitsSettings? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return GetHashCode() == other.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is ConventionalCommitsSettings other && Equals(other));
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return FooterIssueTokens.Aggregate(17, (current, item) => current * 23 + item.GetHashCode());
    }
}