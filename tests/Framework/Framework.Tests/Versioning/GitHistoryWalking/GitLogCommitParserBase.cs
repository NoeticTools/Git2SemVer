using System.Text.RegularExpressions;
using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Extensions;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;


namespace NoeticTools.Git2SemVer.Framework.Tests.Versioning.GitHistoryWalking;

public abstract class GitLogCommitParserBase(
    ICommitsCache cache,
    TagParser tagParser,
    IConventionalCommitsParser? conventionalCommitParser = null)
{
    private const string GitLogParsingPattern =
        """
        ^(?<graph>[^\x1f$]*) 
          (\x1f\.\|
            (?<sha>[^\|]+) \|
            (?<parents>[^\|]*)? \|
            \x02(?<summary>[^\x03]*)?\x03 \|
            \x02(?<body>[^\x03]*)?\x03 \|
            (\s\((?<refs>.*?)\))?
           \|$)?
        """;

    private readonly IConventionalCommitsParser _conventionalCommitParser =
        conventionalCommitParser ?? new ConventionalCommitsParser(new ConventionalCommitsSettings());

    // todo - document this, needed to capture git graph for tests
    public string FormatArgs { get; } = "--graph --pretty=\"format:%x1f.|%H|%P|%x02%s%x03|%x02%b%x03|%d|%x1e\"";

    protected (Commit? commit, string graph) ParseCommitAndGraph(string line)
    {
        line = line.Trim();
        var regex = new Regex(GitLogParsingPattern, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
        var match = regex.Match(line);
        if (!match.Success)
        {
            throw new Git2SemVerGitLogParsingException($"Unable to parse Git log line {line}.");
        }

        var graph = match.GetGroupValue("graph");
        var sha = match.GetGroupValue("sha");
        var refs = match.GetGroupValue("refs");
        var parents = match.GetGroupValue("parents").Split(' ');
        if (parents.All(x => x.Length == 0))
        {
            parents = [];
        }

        var summary = match.GetGroupValue("summary");
        var body = match.GetGroupValue("body").Replace($"{CharacterConstants.GS}", "\n");

        if (cache.TryGet(sha, out var commit))
        {
            return (commit, graph);
        }

        var hasCommitMetadata = line.Contains($"{CharacterConstants.US}.|");
        if (hasCommitMetadata)
        {
            if (sha.Length == 0)
            {
                throw new Git2SemVerGitLogParsingException($"Unable to read SHA from line: '{line}'");
            }
        }

        var commitMetadata = _conventionalCommitParser.Parse(summary, body);

        commit = hasCommitMetadata
            ? new Commit(sha, parents, summary, refs, commitMetadata, tagParser)
            : null;

        return (commit, graph);
    }
}