using Core.Types;

namespace Core.Classes.Core.Properties;

public class FProperty
{
    internal UnrealPackage Package;
    public FName FName { get; set; }
    public string Name => Package.GetName(FName);
    public FName TypeFName { get; set; }
    public string TypeName => Package.GetName(TypeFName);

    public int Size { get; set; }

    public int ArrayIndex { get; set; }

    public override string ToString()
    {
        return Name;
    }
}