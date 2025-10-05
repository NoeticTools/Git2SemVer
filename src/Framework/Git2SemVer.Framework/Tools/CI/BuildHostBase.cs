using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.Logging;


namespace NoeticTools.Git2SemVer.Framework.Tools.CI;

internal abstract class BuildHostBase
{
    private readonly ILogger _logger;
    private string _buildIdFormat = "";
    private Func<IReadOnlyList<string>> _buildNumberFunc;
    private Func<IReadOnlyList<string>> _defaultBuildNumberFunc;

    protected BuildHostBase(ILogger logger)
    {
        _logger = logger;
        _defaultBuildNumberFunc = () => [BuildNumber];
        _buildNumberFunc = _defaultBuildNumberFunc;
    }

    public string BuildContext { get; set; } = "UNKNOWN";

    public IReadOnlyList<string> BuildId => _buildNumberFunc();

    public string BuildIdFormat
    {
        get => _buildIdFormat;
        set
        {
            if (_buildIdFormat == value)
            {
                return;
            }

            _buildIdFormat = value;
            SetBuildIdFunc();
        }
    }

    public string BuildNumber { get; set; } = "UNKNOWN";

    public string Name { get; protected set; } = "UNKNOWN";

    public virtual string BumpBuildNumber()
    {
        // Default implementation: Not supported
        return BuildNumber;
    }

    public virtual void ReportBuildStatistic(string key, int value)
    {
        _logger.LogTrace("{0} host does not support build statistics. Ignored.", Name);
    }

    public virtual void ReportBuildStatistic(string key, double value)
    {
        _logger.LogTrace("{0} host does not support build statistics. Ignored.", Name);
    }

    public virtual void SetBuildLabel(string label)
    {
        _logger.LogTrace("{0} host does not support setting a build label. Ignored.", Name);
    }

    protected Func<IReadOnlyList<string>> DefaultBuildNumberFunc
    {
        get => _defaultBuildNumberFunc;
        set
        {
            _defaultBuildNumberFunc = value;
            SetBuildIdFunc();
        }
    }

    private string[] CustomBuildIdFormat()
    {
        if (string.IsNullOrWhiteSpace(BuildIdFormat))
        {
            throw new Git2SemVerConfigurationException("Host `BuildIdFormat` is required to generate a custom build ID.");
        }

        var buildId = BuildIdFormat.Replace("BUILD_NUMBER", BuildNumber)
                                   .Replace("BUILD_CONTEXT", BuildContext);
        return buildId.Split('.');
    }

    private void SetBuildIdFunc()
    {
        _buildNumberFunc = BuildIdFormat.Length > 0 ? CustomBuildIdFormat : DefaultBuildNumberFunc;
    }
}