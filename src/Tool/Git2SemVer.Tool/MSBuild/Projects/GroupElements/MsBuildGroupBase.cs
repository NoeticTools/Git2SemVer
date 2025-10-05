using System.Xml.Linq;
using System.Xml.XPath;
using NoeticTools.Git2SemVer.Core.Exceptions;


#pragma warning disable CA1826

namespace NoeticTools.Git2SemVer.Tool.MSBuild.Projects.GroupElements;

public abstract class MsBuildGroupBase<T>
{
    private readonly Dictionary<string, T> _cache = [];
    private readonly string _groupElementName;
    private readonly IReadOnlyList<XElement> _groupElements;
    private readonly XDocument _xmlDocument;

    protected MsBuildGroupBase(XDocument xmlDocument, string groupElementName)
    {
        _xmlDocument = xmlDocument;
        _groupElementName = groupElementName;
        _groupElements = GetItemGroups();
    }

    public T this[string name] => GetItem(name);

    protected abstract T CreateItem(XElement element);

    protected T GetItem(string name)
    {
        if (_cache.TryGetValue(name, out var property))
        {
            return property;
        }

        var element = _groupElements.Elements().FirstOrDefault(x => x.Name == name);
        return element == null ? Add(name, "") : Add(name, element);
    }

    private T Add(string name, XElement element)
    {
        var property = CreateItem(element);
        _cache.Add(name, property);
        return property;
    }

    private T Add(string name, string value)
    {
        var element = new XElement(name, value);
        _groupElements.Last().Add(element);
        var property = CreateItem(element);
        _cache.Add(name, property);
        return property;
    }

#pragma warning disable CA1859
    private IReadOnlyList<XElement> GetItemGroups()
#pragma warning restore CA1859
    {
        var groups = _xmlDocument.XPathSelectElements($"//{_groupElementName}")
                                 .Where(x => !x.Attributes("Condition").Any())
                                 .ToList();
        if (groups.Count == 0)
        {
            throw new Git2SemVerInvalidOperationException($"No {_groupElementName} group element found.");
        }

        return groups;
    }
}