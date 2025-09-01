using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

internal sealed class HandledChangeLookup(IEnumerable<HandledChange> handledChanges) : ChangeLookup<HandledChange>(handledChanges)
{
    protected override IChangeTypeAndDescription ToChangeMetadata(HandledChange item)
    {
        return item;
    }
}