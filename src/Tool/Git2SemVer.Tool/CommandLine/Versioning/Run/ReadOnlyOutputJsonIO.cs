using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Versioning;


namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;

internal class ReadOnlyOutputJsonIO : IOutputsJsonIO
{
    public IVersionOutputs Load(string directory)
    {
        return new VersioningOutputsJsonFileIO().Load(directory);
    }

    public void Save(string directory, IVersionOutputs outputs)
    {
        // do nothing
    }
}