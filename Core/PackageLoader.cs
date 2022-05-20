using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;
using Core.Utility;

namespace Core;

/// <summary>
///     Use this instead of loading <see cref="UnrealPackage" /> directly. This can resolve cross package dependencies
///     semi-automatically
/// </summary>
public class PackageLoader
{
    private readonly IObjectSerializerFactory? _ObjectSerializerFactory;
    private readonly IPackageCache _packageCache;
    private readonly IStreamSerializerFor<UnrealPackage> _packageSerializer;
    private readonly IPackageUnpacker _packageUnpacker;

    /// <summary>
    ///     Constructs a package loader with a given package serializer. Only do this if you already know what type of
    ///     serializer you require
    /// </summary>
    /// <param name="packageSerializer"></param>
    /// <param name="packageCache"></param>
    /// <param name="packageUnpacker"></param>
    /// <param name="objectSerializerFactory"></param>
    public PackageLoader(IStreamSerializerFor<UnrealPackage> packageSerializer, IPackageCache packageCache, IPackageUnpacker packageUnpacker,
        IObjectSerializerFactory? objectSerializerFactory = null)
    {
        _packageSerializer = packageSerializer;
        _packageCache = packageCache;
        _packageUnpacker = packageUnpacker;
        _ObjectSerializerFactory = objectSerializerFactory;
    }

    /// <summary>
    ///     Returns a loaded package
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    public UnrealPackage? GetPackage(string packageName)
    {
        return _packageCache.IsPackageCached(packageName) ? _packageCache.GetCachedPackage(packageName) : null;
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
        if (_packageCache.IsPackageCached(packageName))
        {
            return _packageCache.GetCachedPackage(packageName);
        }

        var packageStream = File.OpenRead(packagePath);
        var unrealPackage = DeserializePackage(packageName, packageStream);
        unrealPackage.RootLoader = this;
        _packageCache.AddPackage(unrealPackage);

        var dependencyGraph = new CrossPackageDependencyGraph(_packageCache);
        for (var index = 0; index < unrealPackage.ExportTable.Count; index++)
        {
            dependencyGraph.AddObjectDependencies(new PackageObjectReference(packageName, new ObjectIndex(ObjectIndex.FromExportIndex(index))));
        }

        for (var index = 0; index < unrealPackage.ImportTable.Count; index++)
        {
            dependencyGraph.AddObjectDependencies(new PackageObjectReference(packageName, new ObjectIndex(ObjectIndex.FromImportIndex(index))));
        }

        var loadOrder = dependencyGraph.TopologicalSort();
        foreach (var packageObjectReference in loadOrder)
        {
            var package = _packageCache.ResolveExportPackage(packageObjectReference.PackageName);
            ArgumentNullException.ThrowIfNull(package);
            var obj = package?.GetObjectReference(packageObjectReference.ObjectIndex);
            if (obj != null)
            {
                package!.CreateObject(obj);
            }
        }

        return unrealPackage;
    }

    private UnrealPackage DeserializePackage(string packageName, FileStream packageStream)
    {
        UnrealPackage unrealPackage;
        var loadOptions = new UnrealPackageOptions(_packageSerializer, packageName, _packageCache, _ObjectSerializerFactory);
        if (_packageUnpacker.IsPackagePacked(packageStream))
        {
            var unpackedStream = new MemoryStream();
            _packageUnpacker.Unpack(packageStream, unpackedStream);
            unpackedStream.Position = 0;
            unrealPackage = UnrealPackage.DeserializeAndInitialize(unpackedStream, loadOptions);
        }
        else
        {
            unrealPackage = UnrealPackage.DeserializeAndInitialize(packageStream, loadOptions);
        }

        return unrealPackage;
    }
}