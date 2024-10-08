using System.Xml.Linq;
using NoeticTools.Common;
using NoeticTools.Git2SemVer.Tool.MSBuild.Projects;
using NoeticTools.Testing.Common;


namespace NoeticTools.Git2SemVer.Tool.Tests.BuildPropsFile;

internal class BuildPropsDocumentTests
{
    private NUnitTaskLogger _logger;

    //[TestCase("Git2SemVer_Installed", false)]
    //[TestCase("PropertyB", true)]
    //public void GetBoolPropertyTest(string propertyName, bool expected)
    //{
    //    var target = Setup("Sample1.props");

    //    var result = target.GetBoolProperty(propertyName);

    //    Assert.That(result, Is.EqualTo(expected));
    //}

    //[TestCase("NonExistingProperty")]
    //public void GetBoolPropertyThrowsExceptionWhenPropertyDoesNotExistTest(string propertyName)
    //{
    //    var target = Setup("Sample1.props");

    //    Assert.That(() => target.GetBoolProperty(propertyName), Throws.Exception);
    //}

    //[TestCase("Git2SemVer_Installed", true)]
    //[TestCase("PropertyA", true)]
    //[TestCase("PropertyB", true)]
    //[TestCase("PropertyC", false)]
    //public void HasPropertyTest(string propertyName, bool expectedHas)
    //{
    //    var target = Setup("Sample1.props");

    //    var result = target.HasProperty(propertyName);

    //    Assert.That(result, Is.EqualTo(expectedHas));
    //}

    [SetUp]
    public void SetUp()
    {
        _logger = new NUnitTaskLogger(false);
    }

    [TearDown]
    public void TearDown()
    {
        _logger.Dispose();
    }

    private ProjectDocument Setup(string testPropsFileName)
    {
        var content = new EmbeddedResources<BuildPropsDocumentTests>().GetResourceFileContent(testPropsFileName);
        var xmlDocument = XDocument.Parse(content);
        var target = new ProjectDocument(xmlDocument, new FileInfo("file"));
        return target;
    }
}