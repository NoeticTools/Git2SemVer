using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core.Extensions;


namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

public sealed class ConventionalCommitsParser(ConventionalCommitsSettings convCommitsSettings) : IConventionalCommitsParser
{
    private readonly Regex _bodyRegex = new("""
                                            \A
                                            (
                                              (?<body>\S ((\n|\r\n)|.)*? )
                                              (\Z | (?: (\n|\r\n) ) )
                                            )?
                                            (

                                              (?<footer>
                                                (
                                                  (?: (\n|\r\n)+ )
                                                  (?<token> (BREAKING\sCHANGE) | ( \w[\w-]+ ) )
                                                  ( \( (?<scope>\w[\w-]+) \) )?
                                                  ( \:\s|\s\# )
                                                  (?<value>
                                                    .*
                                                    (?:
                                                      (\n|\r\n).*
                                                    )*?
                                                  )
                                                )+
                                              )?
                                              \Z
                                            )
                                            """,
                                            RegexOptions.IgnorePatternWhitespace |
                                            RegexOptions.Multiline);

    private readonly Regex _summaryRegex = new("""
                                               \A
                                                 (?<ChangeType>\w[\w\-]*)
                                                   (\((?<scope>[\w\-\.]+)\))?(?<breakFlag>!)?: \s+(?<desc>\S.*?)
                                               \Z
                                               """,
                                               RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

    public CommitMessageMetadata Parse(string commitSummary, string commitMessageBody)
    {
        var summaryMatch = _summaryRegex.Match(commitSummary);
        if (!summaryMatch.Success)
        {
            return new CommitMessageMetadata(convCommitsSettings);
        }

        var changeType = summaryMatch.GetGroupValue("ChangeType");
        var scope = summaryMatch.GetGroupValue("scope");
        var breakingChangeFlagged = summaryMatch.GetGroupValue("breakFlag").Length > 0;
        var changeDescription = summaryMatch.GetGroupValue("desc");

        var bodyMatch = _bodyRegex.Match(commitMessageBody);
        var body = bodyMatch.GetGroupValue("body");

        var keyValuePairs = GetFooterKeyValues(bodyMatch);

        return new CommitMessageMetadata(changeType,
                                         scope,
                                         changeDescription,
                                         body,
                                         breakingChangeFlagged,
                                         keyValuePairs,
                                         convCommitsSettings);
    }

    private static FooterKeyValues GetFooterKeyValues(Match match)
    {
        var footerKeyValues = new FooterKeyValues();

        var tokensGroup = match.Groups["token"];
        var valuesGroup = match.Groups["value"];
        if (!tokensGroup.Success || !valuesGroup.Success)
        {
            return footerKeyValues;
        }

        var keywords = tokensGroup.Captures;
        var values = valuesGroup.Captures;

        for (var captureIndex = 0; captureIndex < keywords.Count; captureIndex++)
        {
            var keyword = keywords[captureIndex].Value;
            if (string.IsNullOrEmpty(keyword))
            {
                continue;
            }

            footerKeyValues.Add(keyword, values[captureIndex].Value.TrimEnd());
        }

        return footerKeyValues;
    }
}