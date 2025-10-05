using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;


namespace NoeticTools.Git2SemVer.Core.Tests.Tools.Git;

[TestFixture]
[NonParallelizable]
internal class CommitObfuscatorTests
{
    private CommitObfuscator _target;

    [Test]
    public void FirstShaIs0001Test()
    {
        var result = _target.GetObfuscatedSha("1234567");

        Assert.That(result, Is.EqualTo("0001"));
    }

    [Test]
    public void ReturnsPriorObfuscatedShaTest()
    {
        const string sha1 = "fa340bd213c0001";
        const string sha2 = "fa340bd213c0002";
        const string sha3 = "fa340bd213c0003";

        var obfuscator = _target;

        var result11 = obfuscator.GetObfuscatedSha(sha1);
        var result12 = obfuscator.GetObfuscatedSha(sha1);
        var result21 = obfuscator.GetObfuscatedSha(sha2);
        var result31 = obfuscator.GetObfuscatedSha(sha3);
        var result22 = obfuscator.GetObfuscatedSha(sha2);

        Assert.That(result11, Is.EqualTo(result12));
        Assert.That(result21, Is.EqualTo(result22));
        Assert.That(result11, Is.Not.EqualTo(result21));
        Assert.That(result21, Is.Not.EqualTo(result31));
    }

    [TestCase("0099")]
    [TestCase("123456")]
    public void ShortShaIsReturnedSameTest(string sha)
    {
        var result = _target.GetObfuscatedSha(sha);

        Assert.That(result, Is.EqualTo(sha));
    }

    [SetUp]
    public void SetUp()
    {
        _target = new CommitObfuscator();
    }
}