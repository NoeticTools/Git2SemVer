using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;
using NoeticTools.Git2SemVer.Framework.Generation.Builders;
using NoeticTools.Git2SemVer.Framework.Generation.GitHistoryWalking;
using NoeticTools.Git2SemVer.Framework.Persistence;


namespace NoeticTools.Git2SemVer.Framework.Generation;

[ExcludeFromCodeCoverage]
public sealed class VersioningEngineFactory(ILogger logger)
{
    public IVersioningEngine Create(IVersionGeneratorInputs inputs,
                                    IOutputsJsonIO outputsJsonIO,
                                    IBuildHost host,
                                    ConventionalCommitsSettings convCommitsSettings)
    {
        var gitTool = new GitTool(new TagParser(inputs.ReleaseTagFormat),
                                  new ConventionalCommitsParser(convCommitsSettings))
        {
            RepositoryDirectory = inputs.WorkingDirectory
        };
        var gitPathsFinder = new GitHistoryWalker(gitTool, logger);

        var defaultBuilderFactory = new DefaultVersionBuilderFactory(logger);
        var versionGenerator = new VersioningEngine(inputs,
                                                    host,
                                                    outputsJsonIO,
                                                    gitTool,
                                                    gitPathsFinder,
                                                    defaultBuilderFactory,
                                                    logger);
        return versionGenerator;
    }
}