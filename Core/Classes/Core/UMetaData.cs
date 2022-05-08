using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     Holds a TMap of key\value pairs
/// </summary>
[NativeOnlyClass("Core", "MetaData", "Object")]
public class UMetaData : UObject
{
    /// <inheritdoc />
    public UMetaData(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}