using NoeticTools.Git2SemVer.Tool.MSBuild.Projects.GroupElements;


namespace NoeticTools.Git2SemVer.Tool.MSBuild.Projects;

internal interface IProjectDocument
{
    PropertyGroup Properties { get; }

    void Save();
}