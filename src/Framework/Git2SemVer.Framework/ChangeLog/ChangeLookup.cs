using NoeticTools.Git2SemVer.Core.ConventionCommits;


namespace NoeticTools.Git2SemVer.Framework.ChangeLog;

internal abstract class ChangeLookup<T>
{
    private readonly Dictionary<string, Dictionary<string, T>> _inner = new();

    protected ChangeLookup() : this([])
    {
    }

    protected ChangeLookup(IEnumerable<T> handledChanges)
    {
        AddRange(handledChanges);
    }

    public void Add(T value)
    {
        Add(GetKeys(ToChangeMetadata(value)), value);
    }

    public IReadOnlyList<T> ToList()
    {
        return _inner.SelectMany(x => x.Value.Select(y => y.Value)).ToList();
    }

    public bool TryGet(IChangeTypeAndDescription changeMetadata, out T? value)
    {
        return TryGet(GetKeys(changeMetadata), out value);
    }

    private T this[(string changeType, string description) key] => _inner[key.changeType][key.description];

    private void Add((string changeType, string description) key, T value)
    {
        Dictionary<string, T> itemsDictionary;
        // ReSharper disable once CanSimplifyDictionaryLookupWithTryGetValue
        if (!_inner.ContainsKey(key.changeType))
        {
            itemsDictionary = new Dictionary<string, T>();
            _inner.Add(key.changeType, itemsDictionary);
        }
        else
        {
            itemsDictionary = _inner[key.changeType];
        }

        itemsDictionary.Add(key.description, value);
    }

    private void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    private bool Contains((string changeType, string description) key)
    {
        // ReSharper disable once CanSimplifyDictionaryLookupWithTryGetValue
        return _inner.ContainsKey(key.changeType) && _inner[key.changeType].ContainsKey(key.description);
    }

    private static (string changeType, string description) GetKeys(IChangeTypeAndDescription value)
    {
        return (value.ChangeType, value.Description);
    }

    private bool TryGet((string changeType, string description) key, out T? value)
    {
        if (Contains(key))
        {
            value = this[key];
            return true;
        }

        value = default;
        return false;
    }

    protected abstract IChangeTypeAndDescription ToChangeMetadata(T item);
}