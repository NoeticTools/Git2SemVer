namespace NoeticTools.Git2SemVer.Core.Tools.DotnetCli;

public sealed class SolutionCommands : ISolutionCommands
{
    private readonly DotNetTool _inner;

    internal SolutionCommands(DotNetTool inner)
    {
        _inner = inner;
    }

    public void AddProject(string projectName)
    {
        _inner.Run($"sln add {projectName}");
    }

    public void AddProject(string solutionName, string projectName)
    {
        _inner.Run($"sln {solutionName} add {projectName}");
    }

    public (int returnCode, IReadOnlyList<string> project) GetProjects()
    {
        var returnCode = _inner.RunReturningStdOut("sln list", out var standardOutput);
        // ReSharper disable once UseCollectionExpression
        // FxCop CS0121 fails the build build without the "new char[]".
        return (returnCode, standardOutput.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries));
    }

    public (int returnCode, IReadOnlyList<string> projects) GetProjects(string solutionName)
    {
        var returnCode = _inner.RunReturningStdOut($"sln {solutionName} list", out var standardOutput);
        // ReSharper disable once UseCollectionExpression
        // FxCop CS0121 fails the build build without the "new char[]".
        return (returnCode, standardOutput.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries));
    }

    public void New(string solutionName)
    {
        _inner.Run($"new sln --name \"{solutionName}\" --format sln");
    }

    public void New()
    {
        _inner.Run("new sln --format sln");
    }

    public (int returnCode, string stdOutput) RemoveProject(string projectName)
    {
        var returnCode = _inner.RunReturningStdOut($"sln remove {projectName}", out var standardOutput);
        return (returnCode, standardOutput);
    }

    public (int returnCode, string stdOutput) RemoveProject(string solutionName, string projectName)
    {
        var returnCode = _inner.RunReturningStdOut($"sln {solutionName} remove {projectName}", out var standardOutput);
        return (returnCode, standardOutput);
    }
}