using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

internal sealed class ChangeLogEntryLookup : ChangeLookup<ChangeLogEntry>
{
    protected override IChangeTypeAndDescription ToChangeMetadata(ChangeLogEntry item)
    {
        return item.MessageMetadata;
    }
}