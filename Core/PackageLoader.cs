using Core.Serialization;
using Core.Types;
using Core.Utility;

namespace Core;

/// <summary>
///     Use this instead of loading <see cref="UnrealPackage" /> directly. This can resolve cross package dependencies
///     semi-automatically
/// </summary>
public class PackageLoader
{
    private readonly IImportResolver _packageResolver;
    private readonly Dictionary<string, UnrealPackage> _packages = new();
    private readonly IStreamSerializerFor<UnrealPackage> _packageSerializer;

    /// <summary>
    ///     Constructs a package loader with a given package serializer. Only do this if you already know what type of
    ///     serializer you require
    /// </summary>
    /// <param name="packageSerializer"></param>
    /// <param name="packageResolver"></param>
    public PackageLoader(IStreamSerializerFor<UnrealPackage> packageSerializer, IImportResolver packageResolver)
    {
        _packageSerializer = packageSerializer;
        _packageResolver = packageResolver;
    }


    /// <summary>
    ///     Loads a package from a given path and gives it a specific name. The packageName is required because the filename
    ///     may not always represent the real package name.
    /// </summary>
    /// <param name="packagePath"></param>
    /// <param name="packageName"></param>
    /// <returns></returns>
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