using Moq;
using NoeticTools.Git2SemVer.Core.ConventionCommits;
using NoeticTools.Git2SemVer.Core.Tools.Git;
using NoeticTools.Git2SemVer.Core.Tools.Git.Parsers;
using NoeticTools.Git2SemVer.Testing.Core.ConventionalCommits;
using Semver;


namespace NoeticTools.Git2SemVer.Core.Tests.Tools.Git;

[TestFixture]
internal class CommitTests
{
    private Mock<ITagParser> _tagParser;

    [Test]
    public void CommitTest()
    {
        var messageMetadata = CommitMessageMetadata.Null;

        var target = new Commit("SHA00002", ["SHA00001"], "summary", messageMetadata, _tagParser.Object, [], DateTimeOffset.Now);

        Assert.That(target.TagMetadata.ReleaseType, Is.EqualTo(ReleaseTypeId.NotReleased));
        Assert.That(target.TagMetadata.Version, Is.Null);
        Assert.That(target.TagMetadata.ChangeFlags, Is.EqualTo(new ApiChangeFlags()));
    }

    [Test]
    public void CommitWithFeatureAddedTest()
    {
        var messageMetadata = new FeatureMessageMetadataStub();

        var target = new Commit("SHA00002", ["SHA00001"], "summary", messageMetadata, _tagParser.Object, [], DateTimeOffset.Now);

        Assert.That(target.TagMetadata.ReleaseType, Is.EqualTo(ReleaseTypeId.NotReleased));
        Assert.That(target.TagMetadata.Version, Is.Null);
        Assert.That(target.TagMetadata.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.TagMetadata.ChangeFlags.FunctionalityChange, Is.True);
        Assert.That(target.TagMetadata.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void CommitWithReleaseTagAndFeatureAddedTest()
    {
        var tag = new Mock<IGitTag>();
        tag.Setup(x => x.FriendlyName).Returns("my tag");
        _tagParser.Setup(x => x.ParseTagName("my tag")).Returns(new TagMetadata(ReleaseTypeId.Released, new SemVersion(1, 2, 3)));
        var messageMetadata = new FeatureMessageMetadataStub();

        var target = new Commit("SHA00002", ["SHA00001"], "summary", messageMetadata, _tagParser.Object, [tag.Object], DateTimeOffset.Now);

        Assert.That(target.TagMetadata.ReleaseType, Is.EqualTo(ReleaseTypeId.Released));
        Assert.That(target.TagMetadata.Version, Is.EqualTo(new SemVersion(1, 2, 3)));
        Assert.That(target.TagMetadata.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.TagMetadata.ChangeFlags.FunctionalityChange, Is.False);
        Assert.That(target.TagMetadata.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void RootCommitTest()
    {
        var messageMetadata = CommitMessageMetadata.Null;

        var target = new Commit("SHA00001", [], "summary", messageMetadata, _tagParser.Object, [], DateTimeOffset.Now);

        Assert.That(target.TagMetadata.ReleaseType, Is.EqualTo(ReleaseTypeId.RootCommit));
        Assert.That(target.TagMetadata.Version, Is.Null);
        Assert.That(target.TagMetadata.ChangeFlags, Is.EqualTo(new ApiChangeFlags()));
    }

    [Test]
    public void RootCommitWithFeatureAddedTest()
    {
        var messageMetadata = new FeatureMessageMetadataStub();

        var target = new Commit("SHA00001", [], "summary", messageMetadata, _tagParser.Object, [], DateTimeOffset.Now);

        Assert.That(target.TagMetadata.ReleaseType, Is.EqualTo(ReleaseTypeId.RootCommit));
        Assert.That(target.TagMetadata.Version, Is.Null);
        Assert.That(target.TagMetadata.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.TagMetadata.ChangeFlags.FunctionalityChange, Is.True);
        Assert.That(target.TagMetadata.ChangeFlags.Fix, Is.False);
    }

    [Test]
    public void RootCommitWithReleaseTagAndFeatureAddedTest()
    {
        var tag = new Mock<IGitTag>();
        tag.Setup(x => x.FriendlyName).Returns("my tag");
        _tagParser.Setup(x => x.ParseTagName("my tag")).Returns(new TagMetadata(ReleaseTypeId.Released, new SemVersion(1, 2, 3)));
        var messageMetadata = new FeatureMessageMetadataStub();

        var target = new Commit("SHA00001", [], "summary", messageMetadata, _tagParser.Object, [tag.Object], DateTimeOffset.Now);

        Assert.That(target.TagMetadata.ReleaseType, Is.EqualTo(ReleaseTypeId.Released));
        Assert.That(target.TagMetadata.Version, Is.EqualTo(new SemVersion(1, 2, 3)));
        Assert.That(target.TagMetadata.ChangeFlags.BreakingChange, Is.False);
        Assert.That(target.TagMetadata.ChangeFlags.FunctionalityChange, Is.False);
        Assert.That(target.TagMetadata.ChangeFlags.Fix, Is.False);
    }

    [SetUp]
    public void SetUp()
    {
        _tagParser = new Mock<ITagParser>();
    }
}