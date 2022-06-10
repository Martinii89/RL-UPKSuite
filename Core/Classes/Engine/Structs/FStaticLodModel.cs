﻿namespace Core.Classes.Engine.Structs;

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