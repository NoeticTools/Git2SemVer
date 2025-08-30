using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Framework.Versioning;


namespace NoeticTools.Git2SemVer.Framework.Persistence;

[ExcludeFromCodeCoverage]
public sealed class NullJsonFileIO : IOutputsJsonIO
{
    public IVersionOutputs Read(string directory)
    {
        throw new NotImplementedException("NullJsonFileIO");
    }

    public void Write(string directory, IVersionOutputs outputs)
    {
        throw new NotImplementedException("NullJsonFileIO");
    }
}