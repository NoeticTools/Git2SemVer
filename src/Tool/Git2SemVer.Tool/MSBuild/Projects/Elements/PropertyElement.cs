using System.Xml.Linq;


namespace NoeticTools.Git2SemVer.Tool.MSBuild.Projects.Elements;

internal sealed class PropertyElement
{
    private readonly XElement _element;

    public PropertyElement(XElement element)
    {
        _element = element;
    }

    public bool BoolValue
    {
        get => bool.TryParse(Value, out var result) && result;
        set => _element.Value = value.ToString();
    }

    public string Value
    {
        get => _element.Value;
        set => _element.Value = value;
    }
}