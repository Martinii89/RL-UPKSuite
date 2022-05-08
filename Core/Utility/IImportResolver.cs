using Core.Types;

namespace Core.Utility;

/// <summary>
///     A IImportResolver is able to find related packages. This is required when resolving import objects
/// </summary>
public interface IImportResolver
{
    /// <summary>
    ///     Returns a deserialized and initialized package with the given name. May return null if it fails to find the
    ///     requested package
    /// </summary>
    /// <param name="packageName"></param>
    /// <returns></returns>
    UnrealPackage? ResolveExportPackage(string packageName);
}