using NoeticTools.Git2SemVer.Core.Tools;


namespace NoeticTools.Git2SemVer.Core.Tests.Tools;

public class ContentEditorTests
{
    private const string Content = """
                                   <Project>
                                     
                                     <PropertyGroup>
                                       <Git2SemVerSolutionDirectory>$(MSBuildThisFileDirectory)../../</Git2SemVerSolutionDirectory>
                                       <Git2SemVer_NuGetLoad_Disable >true</Git2SemVer_NuGetLoad_Disable>
                                     </PropertyGroup>

                                     <Import Project="$(Git2SemVerSolutionDirectory)Directory.Build.props" />
                                     <Import Project="$(Git2SemVerSolutionDirectory)MSBuild/Build/NoeticTools.Git2SemVer.MSBuild.props" />

                                   </Project>

                                   """;

    private ContentEditor _target;

    [TestCase("true")]
    [TestCase("Project")]
    [TestCase("x")]
    [TestCase("MSBuildThisFileDirectory")]
    public void RemoveLineWhenSignatureNotPresentDoesNothing(string signature)
    {
        var result = _target.RemoveLinesWith(signature, Content);

        Assert.That(result, Is.EqualTo(Content));
    }

    [TestCase("<PropertyGroup>",
                 """
                 <Project>
                   
                     <Git2SemVerSolutionDirectory>$(MSBuildThisFileDirectory)../../</Git2SemVerSolutionDirectory>
                     <Git2SemVer_NuGetLoad_Disable >true</Git2SemVer_NuGetLoad_Disable>
                   </PropertyGroup>

                   <Import Project="$(Git2SemVerSolutionDirectory)Directory.Build.props" />
                   <Import Project="$(Git2SemVerSolutionDirectory)MSBuild/Build/NoeticTools.Git2SemVer.MSBuild.props" />

                 </Project>

                 """)]
    [TestCase("<Git2SemVerSolutionDirectory>$(MSBuildThisFileDirectory)../../</Git2SemVerSolutionDirectory>",
                 """
                 <Project>
                   
                   <PropertyGroup>
                     <Git2SemVer_NuGetLoad_Disable >true</Git2SemVer_NuGetLoad_Disable>
                   </PropertyGroup>

                   <Import Project="$(Git2SemVerSolutionDirectory)Directory.Build.props" />
                   <Import Project="$(Git2SemVerSolutionDirectory)MSBuild/Build/NoeticTools.Git2SemVer.MSBuild.props" />

                 </Project>

                 """)]
    public void RemoveLineWhenSignaturePresentRemovesLine(string signature, string expected)
    {
        var result = _target.RemoveLinesWith(signature, Content);

        Assert.That(expected, Is.EqualTo(result));
    }

    [SetUp]
    public void Setup()
    {
        _target = new ContentEditor();
    }
}