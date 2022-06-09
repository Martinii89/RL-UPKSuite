using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "StaticMeshComponent", typeof(UPrimitiveComponent))]
public class UStaticMeshComponent : UPrimitiveComponent
{
    public UStaticMeshComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }

    public List<FStaticMeshComponentLODInfo> FStaticMeshComponentLodInfos { get; set; }
}

public class FStaticMeshComponentLODInfo
{
    public List<UObject> ShadowMaps { get; set; } = new();
    public List<UObject> ShadowVertexBuffers { get; set; } = new();
    public FLightMap FLightMapRef { get; set; } = new();
    public byte BLoadVertexColorData { get; set; }
    public FColorVertexBuffer ColorVertexBuffer { get; set; } = new();
}

public class FColorVertexBuffer
{
    private uint NumVertices;

    private uint Stride;
    // TODO fill out the vertex data buffer when NumVertices > 0
}

public class FLightMap
{
    public enum LightMapType
    {
        None = 0,
        LightMap1D = 1,
        LightMap2D = 2
    }

    public List<FGuid> LightGuids { get; set; } = new();
    public LightMapType Type { get; set; } = LightMapType.None;
}

public class FLightMap1D : FLightMap
{
    public UObject? ActorOwner { get; set; }
    public FByteBulkData DirectionalSamples { get; set; } = new();
    public FVector[] ScaleVectors { get; set; } = new FVector[3]; //Actually a FVector4 - but only x,y,z is serialized
    public FByteBulkData SimpleSamples { get; set; } = new();
}

public class FLightMap2D : FLightMap
{
    public ULightMapTexture2D?[] Textures { get; set; } = new ULightMapTexture2D[3];
    public FVector[] ScaleVectors { get; set; } = new FVector[3]; //Actually a FVector4 - but only x,y,z is serialized
    public FVector2D CoordinateScale { get; set; }
    public FVector2D CoordinateBias { get; set; }
}