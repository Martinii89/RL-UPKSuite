using Core.Types;

namespace Core.Utility;

/// <summary>
///     A IPackageCache is able to find related packages. This is required when resolving import objects
/// </summary>
public interface IPackageCache
{
    /// <summary>
    ///     Returns a deserialized and initialized package with the given name. May return null if it fails to find the
    ///     requested package
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    UnrealPackage? ResolveExportPackage(string packageName);

    /// <summary>
    ///     Returns a package ONLY if it's already cached
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown for a missing package</exception>
    UnrealPackage GetCachedPackage(string packageName);

    /// <summary>
    ///     Adds a package to the cache of loaded packages
    /// </summary>
    /// <param name="package"></param>
    void AddPackage(UnrealPackage package);

    /// <summary>
    ///     Retrieve the list of cached packages
    /// </summary>
    /// <returns></returns>
    List<string> GetCachedPackageNames();

    /// <summary>
    ///     Check if a package is already cached
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    bool IsPackageCached(string packageName);

    void RemoveCachedPackage(UnrealPackage package);
}