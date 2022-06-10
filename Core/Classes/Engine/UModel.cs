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
    public FGuid lightingGuid;
    public List<FLightmassPrimitiveSettings> LightmassPrimitiveSettings;
    public uint Linked;
    public int NumVertices;
    public uint RootOutside;
    public FModelVertexBuffer VertexBuffer;

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
    public int bShadowIndirectOnly;
    public int bUseEmissiveForStaticLighting;
    public int bUseTwoSidedLighting;
    public float DiffuseBoost;
    public float EmissiveBoost;
    public float EmissiveLightExplicitInfluenceRadius;
    public float EmissiveLightFalloffExponent;
    public float FullyOccludedSamplesFraction;
    public float SpecularBoost;
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
    public FVector2D BackfaceShadowTexCoord;
    public int iSide;
    public int pVertex;
    public FVector2D ShadowTexCoord;
}

public class FBspSurf
{
    public UObject? Actor;
    public int iBrushPoly;
    public int iLightmassIndex;
    public ulong LightingChannels;
    public UObject? Material;
    public int pBase;
    public FPlane plane;
    public ulong PolyFlags;
    public float ShadowMapScale;
    public int vNormal;
    public int vTextureU;
    public int vTextureV;
}

public class FBspNode
{
    public int ComponentElementIndex;
    public ushort ComponentIndex;
    public ushort ComponentNodeIndex;
    public int[] iChild = new int[3];
    public int iCollisionBound;
    public int[] iLeaf = new int[2];
    public int iSurf;
    public int iVertexIndex;
    public int iVertPool;
    public byte[] iZone = new byte[2];
    public byte NodeFlags;
    public byte NumVertices;
    public FPlane plane;
}