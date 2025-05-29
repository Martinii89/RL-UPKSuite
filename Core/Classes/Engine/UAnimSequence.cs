using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "AnimSequence", typeof(UObject))]
public class UAnimSequence : UObject
{
    public UAnimSequence(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public int NumBytes { get; set; }
    public byte[] SerializedData { get; set; } = Array.Empty<byte>();
    public int RawAnimationDataCount { get; set; }
}