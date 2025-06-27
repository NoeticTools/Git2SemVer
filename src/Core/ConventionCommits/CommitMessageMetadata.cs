﻿using System.Text.Json.Serialization;


namespace NoeticTools.Git2SemVer.Core.ConventionCommits;

[JsonDerivedType(typeof(ICommitMessageMetadata))]
public sealed class CommitMessageMetadata : ICommitMessageMetadata
{
    private static readonly Dictionary<string, CommitChangeTypeId> ChangeTypeIdLookup = new()
    {
        { "feat", CommitChangeTypeId.Feature },
        { "fix", CommitChangeTypeId.Fix }
    };

    public CommitMessageMetadata(string changeType, bool breakingChangeFlagged, string changeDescription, string body,
                                 List<(string key, string value)> footerKeyValues)
    {
        ChangeType = ToChangeTypeId(changeType.ToLower());
        ChangeTypeText = changeType;
        ChangeDescription = changeDescription;
        Body = body;
        FooterKeyValues = footerKeyValues.ToLookup(k => k.key, v => v.value);

        var functionalityChange = ChangeType == CommitChangeTypeId.Feature;
        var fix = ChangeType == CommitChangeTypeId.Fix;
        var breakingChange = breakingChangeFlagged ||
                             FooterKeyValues.Contains("BREAKING-CHANGE") ||
                             FooterKeyValues.Contains("BREAKING CHANGE");

        ApiChangeFlags = new ApiChangeFlags(breakingChange, functionalityChange, fix);
    }

    public CommitMessageMetadata() : this("", false, "", "", [])
    {
    }

    public ApiChangeFlags ApiChangeFlags { get; }

    public string Body { get; }

    public string ChangeDescription { get; }

    public CommitChangeTypeId ChangeType { get; }

    public string ChangeTypeText { get; }

    public ILookup<string, string> FooterKeyValues { get; }

    private static CommitChangeTypeId ToChangeTypeId(string value)
    {
        // ReSharper disable once CanSimplifyDictionaryTryGetValueWithGetValueOrDefault
        if (ChangeTypeIdLookup.TryGetValue(value, out var changeTypeId))
        {
            return changeTypeId;
        }

        return string.IsNullOrWhiteSpace(value) ? CommitChangeTypeId.None : CommitChangeTypeId.Custom;
    }
}