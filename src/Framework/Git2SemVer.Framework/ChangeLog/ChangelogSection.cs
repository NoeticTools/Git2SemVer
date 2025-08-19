using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Exceptions;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

internal sealed class ChangelogSection(string name, ChangelogDocument document)
{
    private const RegexOptions Options = RegexOptions.Multiline | RegexOptions.Singleline;
    private const string SectionContentPattern = @"^(?<=.*?\<\!-- Section start: {0} -->.*?).*(?=^\<\!-- Section end: {0} -->.*?)";
    private const string SectionEndMarkerPattern = @"(^\<\!-- Section end: {0} -->.*?$)";
    private const string SectionStartMarkerPattern = @"(^\<\!-- Section start: {0} -->.*?$)";

    private readonly Regex _regex = new(string.Format(SectionContentPattern, name), Options);

    public string Content
    {
        get
        {
            var sourceMatch = _regex.Match(document.Content);
            if (!sourceMatch.Success)
            {
                throw new
                    Git2SemVerInvalidFormatException($"The {document.Name} changelog is missing missing a start or end of a '{name}' section marker marker like '<!-- Section start: {name} -->'.");
            }

            return sourceMatch.Value;
        }
        set
        {
            var destMatch = _regex.Match(document.Content);
            if (!destMatch.Success)
            {
                throw new
                    Git2SemVerInvalidFormatException($"The {document.Name} changelog is missing missing a start or end of a '{name}' section marker marker like '<!-- Start start: {name} -->'.");
            }

            var newContent = _regex.Replace(document.Content, value, 1);
            document.Content = newContent;
        }
    }

    public bool Exists => _regex.Match(document.Content).Success;

    /// <summary>
    ///     Append text immediately after this section.
    /// </summary>
    /// <param name="text"></param>
    public void AppendAfter(string text)
    {
        var regex = new Regex(string.Format(SectionEndMarkerPattern, name), Options);
        var textBlocks = regex.Split(document.Content);
        if (textBlocks.Length != 3)
        {
            throw new Git2SemVerConfigurationException("Malformed changelog."); // todo
        }

        document.Content = textBlocks[0] + textBlocks[1] + "\n" + text + "\n" + textBlocks[2];
    }

    public static string RemoveSectionMarkers(string content)
    {
        var pattern = @$"({string.Format(SectionStartMarkerPattern, ".*?")}|{string.Format(SectionEndMarkerPattern, ".*?")}\n?)";
        return new Regex(pattern, Options).Replace(content, "");
    }
}