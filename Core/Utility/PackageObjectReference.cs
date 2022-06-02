using Core.Classes.Core;
using Core.Types.PackageTables;

namespace Core.Utility;

/// <summary>
///     A PackageObjectReference points to a unique object in a package, or a native only class for self imported
///     classes.
/// </summary>
public readonly struct PackageObjectReference : IEquatable<PackageObjectReference>
{
    /// <summary>
    ///     This references a native only class in a specific package
    /// </summary>
    public readonly UClass? NativeClass;

    /// <summary>
    ///     A index representing either a import or a export object from a package
    /// </summary>
    public readonly ObjectIndex ObjectIndex;

    /// <summary>
    ///     The name of the package that the ObjectIndex originates from
    /// </summary>
    public readonly string PackageName;

    /// <summary>
    ///     Creates a null reference
    /// </summary>
    public PackageObjectReference()
    {
        ObjectIndex = new ObjectIndex(0);
        PackageName = string.Empty;
        NativeClass = null;
    }

    /// <summary>
    ///     References a unique object in a package
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="objectIndex"></param>
    public PackageObjectReference(string packageName, ObjectIndex objectIndex)
    {
        ObjectIndex = objectIndex;
        PackageName = packageName;
        NativeClass = null;
    }

    /// <summary>
    ///     A package reference for a native only class object with no export\import table entry
    /// </summary>
    /// <param name="packageName"></param>
    /// <param name="nativeClass"></param>
    public PackageObjectReference(string packageName, UClass nativeClass)
    {
        ObjectIndex = new ObjectIndex();
        PackageName = packageName;
        NativeClass = nativeClass;
    }

    /// <summary>
    ///     Check for a null reference
    /// </summary>
    /// <returns></returns>
    public bool IsNull()
    {
        return string.IsNullOrEmpty(PackageName) && ObjectIndex.Index == 0;
    }

    /// <inheritdoc />
    public bool Equals(PackageObjectReference other)
    {
        return Equals(NativeClass, other.NativeClass) && ObjectIndex.Equals(other.ObjectIndex) && PackageName == other.PackageName;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is PackageObjectReference other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(NativeClass, ObjectIndex, PackageName);
    }

    /// <summary>
    ///     Uses the Equals method to overload the == operator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(PackageObjectReference left, PackageObjectReference right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Uses the == operator to overload the != operator
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(PackageObjectReference left, PackageObjectReference right)
    {
        return !(left == right);
    }
}