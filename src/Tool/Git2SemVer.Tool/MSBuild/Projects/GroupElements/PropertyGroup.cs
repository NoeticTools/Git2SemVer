using System.Xml.Linq;
using NoeticTools.Git2SemVer.Tool.MSBuild.Projects.Elements;


namespace NoeticTools.Git2SemVer.Tool.MSBuild.Projects.GroupElements;

internal sealed class PropertyGroup : MsBuildGroupBase<PropertyElement>
{
    public PropertyGroup(XDocument xmlDocument)
        : base(xmlDocument, "PropertyGroup")
    {
    }

    public void Append(string name, string value)
    {
        var property = GetItem(name);
        if (!string.IsNullOrWhiteSpace(property.Value))
        {
            property.Value = property.Value + ";" + value;
        }
        else
        {
            property.Value = $"$({name});{value}";
        }
    }

    protected override PropertyElement CreateItem(XElement element)
    {
        return new PropertyElement(element);
    }
}