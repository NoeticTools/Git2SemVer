using System.Text.Json.Serialization;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

/// <summary>
///     Configuration that defines changelog categories that are included in the changelog.
/// </summary>
public sealed class CategorySettings
{
    [JsonConstructor]
    public CategorySettings()
    {
    }

    public CategorySettings(int order, string name, string changeTypePattern)
    {
        ChangeTypePattern = changeTypePattern;
        Name = name;
        Order = order;
    }

    /// <summary>
    ///     Regular expression pattern matched against Git summary Conventional Commits change type like "feat" or "fix".
    /// </summary>
    public string ChangeTypePattern { get; set; } = "";

    /// <summary>
    ///     The category name to show in the changelog. e.g: "Added".
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    ///     The relative order in which the category will appear in the changelog.
    ///     Lower number appears before higher numbers.
    /// </summary>
    public int Order { get; set; }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        return HashCode.Combine(ChangeTypePattern, Name, Order);
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }
}