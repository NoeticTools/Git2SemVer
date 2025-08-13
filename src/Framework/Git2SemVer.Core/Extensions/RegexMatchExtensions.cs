using System.Text.RegularExpressions;


namespace NoeticTools.Git2SemVer.Core.Extensions;

public static class RegexMatchExtensions
{
    public static string GetGroupValue(this Match match, string groupName)
    {
        if (!match.Success)
        {
            return "";
        }

        var group = match.Groups[groupName];
        return group.Success ? group.Value : "";
    }
}