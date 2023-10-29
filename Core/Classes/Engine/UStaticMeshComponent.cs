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
    //UShadowMap2D
    public List<UObject?> ShadowMaps { get; set; } = new();

    //UShadowMap2D
    public List<UObject?> ShadowVertexBuffers { get; set; } = new();
    public FLightMap FLightMapRef { get; set; } = new();
    public byte BLoadVertexColorData { get; set; }
    public FColorVertexBuffer ColorVertexBuffer { get; set; } = new();
    public int UnkInt { get; set; }
}

public class FColorVertexBuffer
{
    public uint NumVertices { get; set; }
    public uint Stride { get; set; }

    public TArray<FColor> colorStream { get; set; } = new();
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
    public FVector[] ScaleVectors { get; set; } = new FVector[3]; //Actually a FVector4 - but only x,y,z is serialized
    public LightMapType Type { get; set; } = LightMapType.None;

    public FLightMap1D? FLightMap1D { get; set; }
    public FLightMap2D? FLightMap2D { get; set; }
}

public class FLightMap1D
{
    public UObject? ActorOwner { get; set; }
    public FByteBulkData DirectionalSamples { get; set; } = new();

    public FByteBulkData SimpleSamples { get; set; } = new();
}

public class FLightMap2D
{
    public ULightMapTexture2D?[] Textures { get; set; } = new ULightMapTexture2D?[3];
    public FVector2D CoordinateScale { get; set; }
    public FVector2D CoordinateBias { get; set; }
}