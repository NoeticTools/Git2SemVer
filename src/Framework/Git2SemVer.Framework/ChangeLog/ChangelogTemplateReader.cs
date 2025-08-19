using NoeticTools.Git2SemVer.Core;
using NoeticTools.Git2SemVer.Core.Exceptions;
using NoeticTools.Git2SemVer.Core.FileSystem;
using NoeticTools.Git2SemVer.Core.Logging;
using Directory = NoeticTools.Git2SemVer.Core.FileSystem.Directory;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

public sealed class ChangelogTemplateReader(ILogger logger)
{
    public string Load(Directory directory)
    {
        var templatePath = directory + ChangelogConstants.DefaultMarkdownTemplateFilename;
        if (templatePath.Exists())
        {
            return templatePath.ReadAllText();
        }

        logger.LogDebug($"Creating default template file: {templatePath}");
        var defaultTemplate = GetDefaultContent();
        templatePath.WriteAllText(defaultTemplate);
        return defaultTemplate;
    }

    private static string GetDefaultContent()
    {
        var assembly = typeof(ChangelogGenerator).Assembly;
        var resourcePath = assembly.GetManifestResourceNames()
                                   .SingleOrDefault(path => path.EndsWith(ChangelogConstants.DefaultMarkdownTemplateFilename))!;
        if (resourcePath == null)
        {
            throw new Git2SemVerOperationException($"The code resource file '{ChangelogConstants.DefaultMarkdownTemplateFilename}' is required but not found.");
        }

        return assembly.GetResourceFileContent(resourcePath!)!;
    }
}