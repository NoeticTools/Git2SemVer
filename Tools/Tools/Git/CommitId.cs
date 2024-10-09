// ReSharper disable ReplaceSubstringWithRangeIndexer

namespace NoeticTools.Common.Tools.Git;

public sealed class CommitId : IEquatable<CommitId>, IEquatable<string>
{
    private const int ShortShaLength = 7;

    internal CommitId(string sha)
    {
        Id = sha;
        ShortSha = ToShortSha(sha);
    }

    public static string ToShortSha(string sha)
    {
        return sha.Length < 7 ? sha : sha.Substring(0, ShortShaLength);
    }

    public string Id { get; }

    public string ObfuscatedSha => GitObfuscation.GetObfuscatedSha(Id);

    public string ShortSha { get; }

    public bool Equals(string? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        return Id.Equals(other);
    }

    public bool Equals(CommitId? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is CommitId other && Equals(other));
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}