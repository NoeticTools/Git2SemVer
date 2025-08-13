namespace NoeticTools.Git2SemVer.Core.Extensions;

public static class StringExtensions
{
    public static string ToSentenceCase(this string input)
    {
        return input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            // ReSharper disable once ReplaceSubstringWithRangeIndexer
            _ => input[0].ToString().ToUpper() + input.Substring(1)
        };
    }
}