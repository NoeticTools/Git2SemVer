using System.Diagnostics.CodeAnalysis;
using NoeticTools.Git2SemVer.Core.Exceptions;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog.Exceptions;

[ExcludeFromCodeCoverage]
public class Git2SemVerChangelogGenerationException : Git2SemverExceptionBase
{
    public Git2SemVerChangelogGenerationException(string message) : base(message)
    {
    }

    // ReSharper disable once UnusedMember.Global
    public Git2SemVerChangelogGenerationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}