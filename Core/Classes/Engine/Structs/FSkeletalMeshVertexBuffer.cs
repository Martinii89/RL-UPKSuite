using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class FSkeletalMeshVertexBuffer
{
    // if mesh->bHasVertexColors there would be a color stream here

    public int bUseFullPrecisionUVs { get; set; }
    public int bUsePackedPosition { get; set; }
    public FVector MeshExtension { get; set; } = new();
    public FVector MeshOrigin { get; set; } = new();
    public int NumUVSets { get; set; }
    public TArray<GpuVert> VertexBuffer { get; set; } = new();
}