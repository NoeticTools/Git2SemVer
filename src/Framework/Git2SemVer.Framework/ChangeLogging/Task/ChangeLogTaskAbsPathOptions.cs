namespace NoeticTools.Git2SemVer.Framework.ChangeLogging.Task;

public class ChangeLogTaskAbsPathOptions(IChangeLogGeneratorTaskOptions taskOptions) : IChangeLogGeneratorTaskOptions
{
    /// <inheritdoc />
    public string ChangelogArtifactLinkPattern { get; set; } = taskOptions.ChangelogArtifactLinkPattern;

    /// <inheritdoc />
    public string ChangelogDataDirectory { get; set; } =
        ToAbsolutePath(taskOptions.ChangelogDataDirectory, ChangelogConstants.DefaultDataDirectory, taskOptions.WorkingDirectory);

    /// <inheritdoc />
    public bool ChangelogEnable { get; set; } = taskOptions.ChangelogEnable;

    /// <inheritdoc />
    public string ChangelogOutputFilePath { get; set; } =
        ToAbsolutePath(taskOptions.ChangelogOutputFilePath, ChangelogConstants.DefaultFilename, taskOptions.WorkingDirectory);

    /// <inheritdoc />
    public string ChangelogReleaseAs { get; set; } = taskOptions.ChangelogReleaseAs;

    /// <inheritdoc />
    public string WorkingDirectory { get; } = taskOptions.WorkingDirectory;

    private static string ToAbsolutePath(string path, string defaultPath, string workingDirectory)
    {
        if (path.Length == 0)
        {
            path = defaultPath;
        }

        if (defaultPath.Length > 0 && Path.IsPathRooted(path))
        {
            return path;
        }

        return Path.Combine(workingDirectory, path);
    }
}