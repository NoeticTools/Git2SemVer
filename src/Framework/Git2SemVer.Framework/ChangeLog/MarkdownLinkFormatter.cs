namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

internal sealed class MarkdownLinkFormatter(string linkFormat) : ITextFormatter
{
    public string Format(string value)
    {
        if (value.Contains('/'))
        {
            return value;
        }

        var link = string.Format(linkFormat, value);
        return string.Equals(link, value, StringComparison.InvariantCulture) ? link : $"[{value}]({link})";
    }
}