using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "NavigationMeshBase", typeof(UObject))]
public class UNavigationMeshBase : UObject
{
    public UNavigationMeshBase(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }

    public int NavMeshVersionNum { get; set; }
    public int VersionAtGenerationTime { get; set; }

    public List<FMeshVertex> Verts { get; set; } = new();
    public List<FEdgeStorageDatum> EdgeStorageData { get; set; } = new();
    public List<FNavMeshPolyBase> Polys { get; set; } = new();
    public List<FBorderEdgeInfo> BorderEdgeSegments { get; set; } = new();
    public List<FNavMeshEdgeBase> Edges { get; set; } = new();

    public FMatrix LocalToWorld { get; set; } = new();
    public FMatrix WorldToLocal { get; set; } = new();

    public FBox BoxBounds { get; set; } = new();


    public APylon? GetPylon()
    {
        return null;
    }
}

public class FBorderEdgeInfo
{
    public ushort Vert0 { get; set; }
    public ushort Vert1 { get; set; }
    public ushort Poly { get; set; }
}

public class FNavMeshEdgeBase
{
    public byte[] data = Array.Empty<byte>();
    //public ushort Vert0 { get; set; }
    //public ushort Vert1 { get; set; }
    //public ushort Poly0 { get; set; }
    //public ushort Poly1 { get; set; }
    //public float EdgeLEngth { get; set; }
    //public float EffectiveEdgeLength { get; set; }
    //public FVector EdgeCenter { get; set; }
    //public byte EdgeType { get; set; }
}

public class FNavMeshPolyBase
{
    public List<ushort> PolyVerts { get; set; } = new();
    public List<ushort> PolyEdges { get; set; } = new();
    public FVector PolyCenter { get; set; } = new();
    public FVector PolyNormal { get; set; } = new();
    public FBox BoxBounds { get; set; } = new();
    public List<FCoverReference> PolyCover { get; set; } = new();
    public float PolyHeight { get; set; }
}

public class FCoverReference : FActorReference
{
    public int SlotIDx { get; set; }
}

public class FActorReference
{
    public AActor? Actor { get; set; }
    public FGuid Guid { get; set; } = new();
}

public class FEdgeStorageDatum
{
    public int DataPtrOffset { get; set; }
    public short DataSize { get; set; }
    public string ClassName { get; set; }
}

public class FMeshVertex
{
    public FVector vector { get; set; }
    public List<ushort> PolyIndices { get; set; } // may need to clear this out serialization if these are object references?
}