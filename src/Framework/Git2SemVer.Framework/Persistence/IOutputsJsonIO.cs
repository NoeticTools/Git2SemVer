using NoeticTools.Git2SemVer.Framework.Versioning;


namespace NoeticTools.Git2SemVer.Framework.Persistence;

public interface IOutputsJsonIO
{
    IVersionOutputs Load(string directory);
    void Save(string directory, IVersionOutputs outputs);
}