using NoeticTools.Git2SemVer.Core.Extensions;
using Semver;


// ReSharper disable StringLiteralTypo

namespace NoeticTools.Git2SemVer.Core.Tests;

[TestFixture]
internal class SemVersionExtensionsTests
{
    [TestCase("1.2.3+abcdef1234567890abcdef1234567890abcdef12")]
    [TestCase("1.2.3-beta+abcdef1234567890abcdef1234567890abcdef12")]
    [TestCase("1.2.3+1234.mypc.abcdef1234567890abcdef1234567890abcdef12")]
    public void GetGitFullShaTest(string version)
    {
        var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);

        var gitSha = semVersion.GetGitFullSha();

        Assert.That(gitSha, Is.EqualTo("abcdef1234567890abcdef1234567890abcdef12"));
    }

    [TestCase("1.2.3+abcdef1234567890abcdef1234567890abcdef12")]
    [TestCase("1.2.3-beta+abcdef1234567890abcdef1234567890abcdef12")]
    [TestCase("1.2.3+1234.mypc.abcdef1234567890abcdef1234567890abcdef12")]
    public void WithAbbreviatedGitShaTest(string version)
    {
        var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);

        var gitSha = semVersion.WithAbbreviatedGitSha();

        Assert.That(gitSha, Is.Not.Null);
        Assert.That(gitSha.ToString(), Does.EndWith("abcdef1"));
    }

    [TestCase("1.2.3+abcdef1234567890abcdef1234567890abcdef1")]
    [TestCase("1.2.3-beta.abcdef1234567890abcdef1234567890abcdef12+abcdef1")]
    [TestCase("1.2.3+abcdef1")]
    [TestCase("1.2.3")]
    public void WithAbbreviatedGitShaWhenNoShaTest(string version)
    {
        var semVersion = SemVersion.Parse(version, SemVersionStyles.Strict);

        var gitSha = semVersion.WithAbbreviatedGitSha();

        Assert.That(gitSha, Is.Not.Null);
        Assert.That(gitSha.ToString(), Is.EqualTo(version));
    }
}