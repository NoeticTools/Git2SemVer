using NoeticTools.Git2SemVer.Framework.Persistence;
using NoeticTools.Git2SemVer.Framework.Versioning;


namespace NoeticTools.Git2SemVer.Tool.CommandLine.Versioning.Run;

internal class ReadOnlyOutputJsonIO : IOutputsJsonIO
{
    public IVersionOutputs Read(string directory)
    {
        return new VersioningOutputsJsonFileIO().Read(directory);
    }

    public void Write(string directory, IVersionOutputs outputs)
    {
        // do nothing
    }
}