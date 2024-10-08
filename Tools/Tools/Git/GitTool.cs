using System.Text.RegularExpressions;
using Injectio.Attributes;
using NoeticTools.Common.Exceptions;
using NoeticTools.Common.Logging;


namespace NoeticTools.Common.Tools.Git;

#pragma warning disable CS1591
[RegisterTransient]
public class GitTool : IGitTool
{
    private readonly IGitProcessCli _inner;
    private readonly ILogger _logger;

    public GitTool(ILogger logger) : this(new GitProcessCli(logger), logger)
    {
    }

    private GitTool(IGitProcessCli inner, ILogger logger)
    {
        _logger = logger;
        _inner = inner;
        BranchName = GetBranchName();
        HasLocalChanges = GetHasLocalChanges();
    }

    public string BranchName { get; }

    public bool HasLocalChanges { get; }

    public string WorkingDirectory
    {
        get => _inner.WorkingDirectory;
        set => _inner.WorkingDirectory = value;
    }

    public IReadOnlyList<Commit> GetCommits(int skipCount, int takeCount)
    {
        var commits = new List<Commit>();

        var result = Run($"log --skip={skipCount} --max-count={takeCount} --pretty=\"format:%H|%P|%s|%d|\"");
        if (result.returnCode != 0)
        {
            _logger.LogError($"'Git log' command returned non-zero return code {result.returnCode}.");
        }

        foreach (var line in result.stdOutput.Split('\n'))
        {
            if (line.Length == 0)
            {
                continue;
            }

            var regex = new Regex(@"^(?<shortSha>[^\|]*)?\|(?<parents>[^\|]*)?\|(?<summary>[^\|]*)?\|(( \(tag: (?<tags>[^\|]+)*\))|([^\|]*))\|$",
                                  RegexOptions.Multiline);
            var match = regex.Match(line.Trim());
            if (!match.Success)
            {
                _logger.LogWarning($"Unexpected git log line: {line}.");
            }

            var tags = GetGroupValue(match, "tags");
            var shortSha = GetGroupValue(match, "shortSha");
            var parents = GetGroupValue(match, "parents").Split(' ');
            var summary = GetGroupValue(match, "summary");

            _logger.LogTrace($"Parsed git log line '{line}': sha {shortSha}, tags: {tags}");

            commits.Add(new Commit(shortSha, parents, summary, tags));
        }

        _logger.LogTrace($"Read {commits.Count} commits from git history. Skipped {skipCount}.");

        return commits;
    }

    public (int returnCode, string stdOutput) Run(string arguments)
    {
        var outWriter = new StringWriter();
        var errorWriter = new StringWriter();

        var returnCode = _inner.Run(arguments, outWriter, errorWriter);

        if (returnCode != 0)
        {
            throw new Git2SemVerGitOperationException($"Git command '{arguments}' returned non-zero return code: {returnCode}");
        }

        var errorOutput = errorWriter.ToString();
        if (!string.IsNullOrWhiteSpace(errorOutput))
        {
            _logger.LogError($"Git command '{arguments}' returned error: {errorOutput}");
        }

        return (returnCode, outWriter.ToString());
    }

    private string GetBranchName()
    {
        var result = Run("status -b -s --porcelain");
        if (result.returnCode != 0) // todo - not required as Run checks return code
        {
            _logger.LogInfo(result.stdOutput);
            _logger.LogError("Unable to run 'git status'. Appears git is not executable.");
            return "";
        }

        var regex = new Regex(@"^## (?<branchName>\S+)(\.\.\.)?");
        var match = regex.Match(result.stdOutput);

        if (!match.Success)
        {
            _logger.LogError($"Unable to read branch name from Git status. Received: '{result.stdOutput}'");
        }

        return match.Groups["branchName"].Value;
    }

    private string GetGitVersion()
    {
        var process = new ProcessCli(_logger);
        var result = process.Run("git", "--version");
        if (result.returnCode != 0)
        {
            _logger.LogError($"Unable to read git version. Return code was '{result.returnCode}'.");
        }

        return result.stdOutput;
    }

    private static string GetGroupValue(Match match, string groupName)
    {
        var group = match.Groups[groupName];
        return group.Success ? group.Value : "";
    }

    private bool GetHasLocalChanges()
    {
        var result = Run("status -u -s --porcelain");
        return result.stdOutput.Length > 0;
    }
}