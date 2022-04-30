using Core.Serialization;
using Core.Types;

namespace Core;

public class PackageLoader
{
    private readonly Dictionary<string, UnrealPackage> _packages = new();
    private readonly IStreamSerializerFor<UnrealPackage> _packageSerializer;

    public PackageLoader(IStreamSerializerFor<UnrealPackage> packageSerializer)
    {
        _packageSerializer = packageSerializer;
    }

    public UnrealPackage LoadPackage(string packagePath, string packageName)
    {
        if (_packages.TryGetValue(packageName, out var package))
        {
            return package;
        }

        var packageStream = File.OpenRead(packagePath);
        var unrealPackage = _packageSerializer.Deserialize(packageStream);
        unrealPackage.RootLoader = this;
        _packages.Add(packageName, unrealPackage);
        return unrealPackage;
    }
}