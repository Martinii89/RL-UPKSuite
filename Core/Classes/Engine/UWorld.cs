using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "World", typeof(UObject))]
public class UWorld : UObject
{
    public UWorld(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public UObject? SaveGameSummary_DEPRECATED { get; set; }

    public UObject? PersistentFaceFXAnimSet { get; set; }

    public ULevel? PersistentLevel { get; set; }
    public FLevelViewportInfo[] FLevelViewportInfos { get; set; } = new FLevelViewportInfo[4];
    public List<UObject?> ExtraReferencedObjects { get; set; }
}

public class FLevelViewportInfo
{
    public FVector CamPosition { get; set; } = new();
    public FRotator CamRotation { get; set; } = new();
    public float CamOrthoZoom { get; set; }
}