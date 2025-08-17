using NoeticTools.Git2SemVer.Core.Logging;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Framework.Framework;
using NoeticTools.Git2SemVer.Framework.Framework.BuildHosting;


namespace NoeticTools.Git2SemVer.Framework.Versioning.Builders.Scripting;

public sealed class ScriptVersionBuilder(ILogger logger) : IVersionBuilder
{
    public void Build(IBuildHost host, IGitTool gitTool, IVersionGeneratorInputs inputs, IVersionOutputs outputs,
                      IMSBuildGlobalProperties msBuildGlobalProperties)
    {
        if (inputs == null)
        {
            throw new ArgumentException("Script version builder requires non-null inputs.", nameof(inputs));
        }

        if (inputs.RunScript == false)
        {
            logger.LogDebug("User C# script versioning skipped as option not enabled.");
            return;
        }

        if (!File.Exists(inputs.BuildScriptPath))
        {
            if (inputs.RunScript == null)
            {
                logger.LogDebug($"User C# script '{inputs.BuildScriptPath}' was not found. Ignoring as run script options is not set.");
                return;
            }

            if (inputs.RunScript == true)
            {
                logger.LogError($"C# script '{inputs.BuildScriptPath}' was not found and run script options is enabled.");
                return;
            }
        }

        if (!inputs.Validate(logger))
        {
            return;
        }

        logger.LogDebug("Running user C# script version builder.");
        using (logger.EnterLogScope())
        {
            var context = new VersioningContext(inputs, outputs, host, gitTool, msBuildGlobalProperties, logger);
            var scriptRunner = new Git2SemVerScriptRunner(new CSharpScriptRunner(logger), logger);

            // ReSharper disable once UnusedVariable
            var task = scriptRunner.RunScript(context, inputs.BuildScriptPath);
            if (logger.IsLogging(LoggingLevel.Trace))
            {
                logger.LogTrace(outputs.GetReport());
            }
        }
    }
}