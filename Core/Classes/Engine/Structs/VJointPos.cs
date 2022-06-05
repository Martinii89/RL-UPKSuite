using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class VJointPos
{
    public FQuat Orientation { get; set; } = new();
    public FVector Position { get; set; } = new();
}

public class FStaticLodModel
{
    public List<FSkelMeshSection> Sections { get; set; } = new();
    public FSkelIndexBuffer IndexBuffer { get; set; } = new();
    public List<short> UsedBones { get; set; } = new();
    public List<FSkelMeshChunk> Chunks { get; set; } = new();
    public int Size { get; set; }
    public int NumVerts { get; set; }
    public List<byte> RequiredBones { get; set; } = new();
    public FByteBulkData FBulkData { get; set; } = new();
    public int NumUvSets { get; set; }
    public FSkeletalMeshVertexBuffer GpuSkin { get; set; } = new();
}

public class FSkeletalMeshVertexBuffer
{
    // if mesh->bHasVertexColors there would be a color stream here
    public FSkelIndexBuffer AdjacencyIndexBuffer { get; set; } = new();
    public int bUseFullPrecisionUVs { get; set; }
    public int bUsePackedPosition { get; set; }
    public List<FSkeletalMeshVertexInfluences> ExtraVertexInfluences { get; set; } = new();
    public FVector MeshExtension { get; set; } = new();
    public FVector MeshOrigin { get; set; } = new();
    public int NumUVSets { get; set; }
    public TArray<GpuVert> VertexBuffer { get; set; } = new();
}

public class GpuVert
{
    public byte[] BoneIndex { get; set; } = new byte[4];
    public byte[] BoneWeight { get; set; } = new byte[4];
    public uint N0 { get; set; }
    public uint N1 { get; set; }
    public FVector Pos { get; set; } = new();
    public UvHalf[] UV { get; set; } = Array.Empty<UvHalf>();
}

public class FSkeletalMeshVertexInfluences
{
    // TODO: fill out. throw if the array in the vertex buffer is not empty for now!
}

public class FSkelMeshChunk
{
    public List<short> Bones { get; set; } = new();
    public int FirstVertex { get; set; }
    public int MaxInfluences { get; set; }
    public int NumRigidVerts { get; set; }
    public int NumSoftVerts { get; set; }
    public List<FRigidVertex> RigidVerts { get; set; } = new();
    public List<FSoftVertex> SoftVerts { get; set; } = new();
}

public class FSoftVertex
{
    public byte[] BoneIndex { get; set; } = new byte[4];
    public byte[] BoneWeight { get; set; } = new byte[4];
    public FColor Color { get; set; } = new();
    public uint[] Normal { get; set; } = new uint[3];
    public FVector Pos { get; set; } = new();
    public FVector2D[] UV { get; set; } = new FVector2D[4];
}

public class FRigidVertex
{
    public byte BoneIndex { get; set; }
    public FColor Color { get; set; } = new();
    public uint[] Normal { get; set; } = new uint[3];
    public FVector Pos { get; set; } = new();
    public FVector2D[] UV { get; set; } = new FVector2D[4];
}

public class FSkelIndexBuffer
{
    public TArray<uint> Indices { get; set; } = new();
    public byte Size { get; set; }
    public int Unk { get; set; }
}

public class FSkelMeshSection
{
    public ushort MaterialIndex { get; set; }
    public ushort ChunkIndex { get; set; }
    public int FirstIndex { get; set; }
    public int NumTriangles { get; set; }
    public byte TriangleSorting { get; set; }
}