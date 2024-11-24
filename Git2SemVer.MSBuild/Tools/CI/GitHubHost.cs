﻿using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.MSBuild.Framework.BuildHosting;


namespace NoeticTools.Git2SemVer.MSBuild.Tools.CI;

internal class GitHubHost : BuildHostBase, IBuildHost
{
    private readonly ILogger _logger;

    public GitHubHost(ILogger logger) : base(logger)
    {
        _logger = logger;
        Name = "GitHub";
        DefaultBuildNumberFunc = () => [BuildNumber, BuildContext];
    }

    public HostTypeIds HostTypeId => HostTypeIds.GitHub;
}