using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Versioning;
using Semver;


namespace NoeticTools.Git2SemVer.Framework.Tests.Versioning;

[TestFixture]
internal class VersionOutputsTests
{
    [SetUp]
    public void SetUp()
    {
        //CommitObfuscator.Clear();
    }

    [Test]
    public void CanDeserialiseRev2()
    {
        const string content = """
                               {
                                 "Rev": 2,
                                 "Git2SemVerVersionInfo": {
                                   "AssemblyVersion": "10.11.12",
                                   "BuildContext": "CONTEXT",
                                   "BuildNumber": "777",
                                   "BuildSystemVersion": "5.6.7-TEST",
                                   "FileVersion": null,
                                   "Git": {
                                     "$type": "GitOutputs",
                                     "BranchName": "",
                                     "HasLocalChanges": false,
                                     "HeadCommit": {
                                       "$type": "Commit",
                                       "CommitId": {
                                         "Sha": "00000000",
                                         "ShortSha": "0000000"
                                       },
                                       "ReleasedVersion": null,
                                       "Summary": "null commit",
                                       "MessageBody": "",
                                       "Parents": [],
                                       "Metadata": {
                                         "ApiChangeFlags": {
                                           "BreakingChange": false,
                                           "Fix": false,
                                           "FunctionalityChange": false
                                         },
                                         "Body": "",
                                         "ChangeDescription": "",
                                         "ChangeType": 1,
                                         "FooterKeyValues": []
                                       }
                                     },
                                     "LastReleaseCommit": null,
                                     "LastReleaseVersion": null
                                   },
                                   "InformationalVersion": null,
                                   "IsInInitialDevelopment": false,
                                   "Output1": "",
                                   "Output2": "",
                                   "PackageVersion": null,
                                   "PrereleaseLabel": "",
                                   "Version": null
                                 }
                               }
                               """;

        var result = VersioningOutputsJsonFileIO.FromJson(content);

        Assert.That(result.AssemblyVersion!.ToString(), Is.EqualTo("10.11.12"));
        Assert.That(result.BuildContext, Is.EqualTo("CONTEXT"));
        Assert.That(result.BuildNumber, Is.EqualTo("777"));
    }

    [Test]
    public void CanSerialise()
    {
        var target = new VersionOutputs
        {
            AssemblyVersion = new Version(10, 11, 12),
            BuildContext = "CONTEXT",
            BuildNumber = "777",
            BuildSystemVersion = new SemVersion(5, 6, 7).WithPrerelease("TEST")
        };

        var result = VersioningOutputsJsonFileIO.ToJson(target);

        Assert.That(result, Is.Not.Null);

        const string expected = """
                                {
                                  "rev": 3,
                                  "git2SemVerVersionInfo": {
                                    "version": "0.0.0",
                                    "informationalVersion": "0.0.0",
                                    "packageVersion": "0.0.0",
                                    "buildSystemVersion": "5.6.7-TEST",
                                    "assemblyVersion": "10.11.12",
                                    "fileVersion": "0.0.0",
                                    "buildNumber": "777",
                                    "prereleaseLabel": "",
                                    "buildContext": "CONTEXT",
                                    "isInInitialDevelopment": false,
                                    "git": {
                                      "$type": "GitOutputs",
                                      "branchName": "",
                                      "contributingReleases": [],
                                      "hasLocalChanges": false,
                                      "headCommit": {
                                        "$type": "Commit",
                                        "commitId": {
                                          "sha": "00000000"
                                        },
                                        "tagMetadata": {
                                          "releaseType": 1,
                                          "version": null,
                                          "changeFlags": {
                                            "breakingChange": false,
                                            "functionalityChange": false,
                                            "fix": false
                                          }
                                        },
                                        "summary": "null commit",
                                        "parents": [],
                                        "messageMetadata": {
                                          "apiChangeFlags": {
                                            "breakingChange": false,
                                            "functionalityChange": false,
                                            "fix": false
                                          },
                                          "body": "",
                                          "description": "",
                                          "changeType": "",
                                          "scope": "",
                                          "footerKeyValues": []
                                        }
                                      },
                                      "lastReleaseCommit": null,
                                      "lastReleaseVersion": null
                                    },
                                    "output1": "",
                                    "output2": ""
                                  }
                                }
                                """;
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void SetAllVersionPropertiesFrom()
    {
        var target = new VersionOutputs();
        var informationalVersion = new SemVersion(0, 5, 6).WithPrerelease("Beta-InitialDev", "77")
                                                          .WithMetadata("METADATA");

        target.SetAllVersionPropertiesFrom(informationalVersion, "BUILD_NUMBER", "BUILD_CONTEXT");

        Assert.That(target.InformationalVersion, Is.EqualTo(informationalVersion));
        Assert.That(target.PackageVersion, Is.EqualTo(informationalVersion.WithoutMetadata()));
    }
}