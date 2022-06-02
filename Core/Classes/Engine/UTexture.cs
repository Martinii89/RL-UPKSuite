using Core.Classes.Core;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "Texture", typeof(USurface))]
public class UTexture : USurface
{
    public UTexture(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public FByteBulkData SourceArt { get; set; } = new();
}

[Flags]
public enum BulkDataFlags : uint
{
    None = 0,
    StoreInSeparateFile = 0x1,
    SerializeCompressedZLIB = 0x2,
    ForceSingleElementSerialization = 0x4,
    SingleUse = 0x8,
    SerializeCompressedLZO = 0x10,
    Unused = 0x20,
    StoreOnlyPayload = 0x40,
    SerializeCompressedLZX = 0x80,
    SerializeCompressed = SerializeCompressedZLIB | SerializeCompressedLZO | SerializeCompressedLZX
}