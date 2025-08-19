using NoeticTools.Git2SemVer.Framework.Versioning;


namespace NoeticTools.Git2SemVer.MSBuild.Tasks;

public interface IChangeLogGeneratorTaskOptions : ICommonTaskOptions
{
    /// <summary>
    ///     Path to changelog generator's data and configuration files directory. It may be a relative or absolute path.
    /// </summary>
    string ChangelogDataDirectory { get; } // todo - constant
}