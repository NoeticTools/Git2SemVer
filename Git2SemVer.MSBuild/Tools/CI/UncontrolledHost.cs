using NoeticTools.Common.Exceptions;
using NoeticTools.Common.Logging;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;
using NoeticTools.Git2SemVer.MSBuild.Framework.Config;
using NoeticTools.Git2SemVer.MSBuild.Framework.Semver;


namespace NoeticTools.Git2SemVer.MSBuild.Tools.CI;

internal class UncontrolledHost : BuildHostBase, IDetectableBuildHost
{
    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    public UncontrolledHost(IConfiguration config, ILogger logger)
    {
        _config = config;
        _logger = logger;
        BuildContext = Environment.MachineName.ToNormalisedSemVerIdentifier();
        BuildNumber = _config.BuildNumber.ToString();
        DefaultBuildNumberFunc = () => [BuildContext, BuildNumber];

        if (string.IsNullOrWhiteSpace(BuildContext))
        {
            throw new Git2SemVerConfigurationException("UncontrolledHost: Host.BuildContext is required.");
        }

        logger.LogDebug($"Uncontrolled host. Build ID = {BuildId[0]}.{BuildId[1]}");
    }

    public HostTypeIds HostTypeId => HostTypeIds.Uncontrolled;

    public string Name => "Uncontrolled";

    public string BumpBuildNumber()
    {
        _config.BuildNumber++;
        _config.Save();
        BuildNumber = _config.BuildNumber.ToString();
        return BuildNumber;
    }

    public bool MatchesHostSignature()
    {
        return true;
    }

    public void ReportBuildStatistic(string key, int value)
    {
        _logger.LogTrace($"Build statistic {key} = {value}");
    }

    public void ReportBuildStatistic(string key, double value)
    {
        _logger.LogTrace($"Build statistic {key} = {value:G13}");
    }

    public void SetBuildLabel(string label)
    {
        _logger.LogDebug($"Build label: '{label}'");
    }
}