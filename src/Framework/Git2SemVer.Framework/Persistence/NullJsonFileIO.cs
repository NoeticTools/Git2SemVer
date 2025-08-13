using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Framework.Versioning;


namespace NoeticTools.Git2SemVer.Framework.Persistence;

[ExcludeFromCodeCoverage]
public sealed class NullJsonFileIO : IOutputsJsonIO
{
    public IVersionOutputs Load(string directory)
    {
        throw new NotImplementedException("NullJsonFileIO");
    }

    public void Save(string directory, IVersionOutputs outputs)
    {
        throw new NotImplementedException("NullJsonFileIO");
    }
}