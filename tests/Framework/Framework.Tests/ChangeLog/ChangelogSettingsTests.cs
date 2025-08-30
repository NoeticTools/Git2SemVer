using NoeticTools.Git2SemVer.Framework.ChangeLog;


namespace NoeticTools.Git2SemVer.Framework.Tests.ChangeLog;

[TestFixture]
public class ChangelogSettingsTests
{
    [Test]
    public void JsonRoundTripTest()
    {
        var original = new ChangelogSettings
        {
            DataDirectory = "data_directory/",
            OutputFilePath = "output/path",
            ArtifactLinkPattern = "http://some_url.domain/%VERSION%",
            ConvCommits =
            {
                FooterIssueTokens = ["token1", "token2"]
            },
            Categories =
            [
                new CategorySettings(1, "Section", "tagName")
            ]
        };
        var json = original.ToJson();

        var copy = ChangelogSettings.FromJson(json);

        Assert.That(copy, Is.EqualTo(original));
        Assert.That(copy.ConvCommits.GetHashCode(), Is.EqualTo(original.ConvCommits.GetHashCode()));
        Assert.That(copy.GetHashCode(), Is.EqualTo(original.GetHashCode()));
        Assert.That(copy, Is.Not.SameAs(original));
    }

    [Test]
    public void ToJsonWithDefaultsTest()
    {
        var settings = new ChangelogSettings();

        var json = settings.ToJson();

        Console.WriteLine(json);

        Assert.That(json, Is.EqualTo("""
                                     {
                                       "rev": "1",
                                       "enabled": true,
                                       "outputFilePath": "CHANGELOG.md",
                                       "dataDirectory": ".git2semver/",
                                       "artifactLinkPattern": "",
                                       "issueLinkFormat": "{0}",
                                       "convCommits": {
                                         "footerIssueTokens": [
                                           "issues",
                                           "issue",
                                           "ref",
                                           "refs"
                                         ]
                                       },
                                       "categories": [
                                         {
                                           "changeTypePattern": "feat",
                                           "name": "Added",
                                           "order": 1
                                         },
                                         {
                                           "changeTypePattern": "change",
                                           "name": "Changed",
                                           "order": 2
                                         },
                                         {
                                           "changeTypePattern": "deprecate",
                                           "name": "Depreciated",
                                           "order": 3
                                         },
                                         {
                                           "changeTypePattern": "remove",
                                           "name": "Removed",
                                           "order": 4
                                         },
                                         {
                                           "changeTypePattern": "fix",
                                           "name": "Fixed",
                                           "order": 5
                                         },
                                         {
                                           "changeTypePattern": "security",
                                           "name": "Security",
                                           "order": 6
                                         },
                                         {
                                           "changeTypePattern": "^(?!dev|Dev|refactor).*$",
                                           "name": "Other",
                                           "order": 7
                                         }
                                       ]
                                     }
                                     """));
    }
}