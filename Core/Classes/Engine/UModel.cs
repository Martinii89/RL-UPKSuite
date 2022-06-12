using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Classes.Engine;
/*
    UModel 010 template

    FBoxSphereBounds bounds;
    TArray_FVector vectors;
    TArray_FVector points;
    TArray_FBspNode Nodes;
    OIndex owner;
    TArray_FBspSurf Surfs;
    TArray_FVert Verts;
    int NumSharedSides;
    int NumZones;
    //FZoneProperty Zones[NumZones];
    OIndex polys;
    int elementSize;
    FIntArray leafHulls;
    int elementSize;
    FIntArray Leaves;
    uint RootOutside;
    uint Linked;
    int elementSize;
    FIntArray PortalNodes;
    int NumVertices;
    int elementSize;
    int VerticesCount;
    GUID LightingGuid;
    TArray_FLightmassPrimitiveSettings a;
 */

[NativeOnlyClass("Engine", "Model", typeof(UObject))]
public class UModel : UObject
{
    public FGuid lightingGuid { get; set; } = new();
    public List<FLightmassPrimitiveSettings> LightmassPrimitiveSettings { get; set; } = new();
    public uint Linked {get; set;}
    public int NumVertices {get; set;}
    public uint RootOutside {get; set;}
    public FModelVertexBuffer VertexBuffer { get; set; } = new();

    public UModel(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public FBoxSphereBounds bounds { get; set; } = new();
    public TArray<FVector> Vectors { get; set; } = new();
    public TArray<FVector> Points { get; set; } = new();
    public TArray<FBspNode> Nodes { get; set; } = new();

    public TransientArray<FBspSurf> Surfs { get; set; }
    public TArray<FVert> Verts { get; set; } = new();

    public int NumSharedSides { get; set; }
    public int NumZones { get; set; }
    public FZoneProperties[] Zones { get; set; } = new FZoneProperties[64];

    public UObject? Polys { get; set; }

    public TArray<int> LeafHulls { get; set; } = new();
    public TArray<int> Leaves { get; set; } = new();
    public TArray<int> PortalNodes { get; set; } = new();
}

public class FLightmassPrimitiveSettings
{
    public int bShadowIndirectOnly {get; set; }
    public int bUseEmissiveForStaticLighting {get; set;}
    public int bUseTwoSidedLighting {get; set;}
    public float DiffuseBoost {get; set;}
    public float EmissiveBoost {get; set;}
    public float EmissiveLightExplicitInfluenceRadius {get; set;}
    public float EmissiveLightFalloffExponent {get; set;}
    public float FullyOccludedSamplesFraction {get; set;}
    public float SpecularBoost {get; set;}
}

public class FModelVertexBuffer
{
    public TArray<FModelVertex> Vertices { get; set; } = new();
}

public class FModelVertex
{
}

public class FZoneProperties
{
}

public class FVert
{
    public FVector2D BackfaceShadowTexCoord {get; set; }
    public int iSide {get; set;}
    public int pVertex {get; set;}
    public FVector2D ShadowTexCoord {get; set;}
}

public class FBspSurf
{
    public UObject? Actor {get; set;}
    public int iBrushPoly {get; set;}
    public int iLightmassIndex {get; set;}
    public ulong LightingChannels {get; set;}
    public UObject? Material {get; set;}
    public int pBase {get; set;}
    public FPlane plane {get; set;}
    public ulong PolyFlags {get; set;}
    public float ShadowMapScale {get; set;}
    public int vNormal {get; set;}
    public int vTextureU {get; set;}
    public int vTextureV {get; set;}
}

public class FBspNode
{
    public int ComponentElementIndex {get; set; }
    public ushort ComponentIndex {get; set;}
    public ushort ComponentNodeIndex {get; set;}
    public int[] iChild {get; set;}= new int[3];
    public int iCollisionBound {get; set;}
    public int[] iLeaf {get; set;}= new int[2];
    public int iSurf {get; set;}
    public int iVertexIndex {get; set;}
    public int iVertPool {get; set;}
    public byte[] iZone {get; set;}= new byte[2];
    public byte NodeFlags {get; set;}
    public byte NumVertices {get; set;}
    public FPlane plane {get; set;}
}