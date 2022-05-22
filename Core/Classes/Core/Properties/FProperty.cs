using Core.Types;

namespace Core.Classes.Core.Properties;

public class FProperty
{
    internal UnrealPackage Package;
    public string Name { get; set; }

    public PropertyType Type { get; set; }

    public string StructName { get; set; }
    public string EnumName { get; set; }

    public int Size { get; set; }

    public int ArrayIndex { get; set; }

    public long ValueStart { get; set; }

    public override string ToString()
    {
        return Name;
    }
}