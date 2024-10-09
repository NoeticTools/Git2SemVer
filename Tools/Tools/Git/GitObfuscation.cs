using System.Text.RegularExpressions;
using NoeticTools.Common.Exceptions;
#pragma warning disable SYSLIB1045


namespace NoeticTools.Common.Tools.Git;

#pragma warning disable CS1591
internal static class GitObfuscation
{
    private static readonly Dictionary<string, string> ObfuscatedCommitShaLookup = new Dictionary<string, string>();

    public static void Reset()
    {
        ObfuscatedCommitShaLookup.Clear();
    }

    /// <summary>
    /// Get partially obfuscated version of the log line (--pretty="format:|%H|%P|%s|%d|") suitable for logging.
    /// </summary>
    public static string ObfuscateLogLine(string line)
    {
        if (line.Length == 0)
        {
            return line;
        }

        var regex = new Regex(@"^\|(?<sha>[^\|]*)?\|(?<parents>[^\|]*)?\|(?<summary>[^\|]*)?\|(?<refs>[^\|]*)?\|$", RegexOptions.Multiline);
        var match = regex.Match(line.Trim());
        if (!match.Success)
        {
            throw new Git2SemVerGitOperationException($"Unable to obtain obfuscated log line from line: {line}.");
        }

        var sha = GetGroupValue(match, "sha");
        var parents = GetGroupValue(match, "parents").Split(' ');
        var tags = GetGroupValue(match, "refs")!;
        var redactedRefs = new Regex(@"\(HEAD -> .*?\)").Replace(tags, "(HEAD -> REDACTED_BRANCH, origin/REDACTED_BRANCH)");
        redactedRefs = new Regex(@"\(origin/.*?\)").Replace(redactedRefs, "(origin/REDACTED_BRANCH, REDACTED_BRANCH)");

        return $"  |{GetObfuscatedSha(sha)}|{string.Join(" ", parents.Select(GetObfuscatedSha))}|REDACTED|{redactedRefs}|";
    }

    private static string GetGroupValue(Match match, string groupName)
    {
        var group = match.Groups[groupName];
        return group.Success ? group.Value : "";
    }

    public static string GetObfuscatedSha(string sha)
    {
        if (ObfuscatedCommitShaLookup.TryGetValue(sha, out var value))
        {
            return value;
        }

        var newValue = (ObfuscatedCommitShaLookup.Count + 1).ToString("D7");
        ObfuscatedCommitShaLookup.Add(sha, newValue);
        return newValue;
    }
}