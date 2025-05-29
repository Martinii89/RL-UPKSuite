namespace RlUpk.Core.Classes.Engine.Structs;

public class FSkelMeshSection
{
    public ushort MaterialIndex { get; set; }
    public ushort ChunkIndex { get; set; }
    public int FirstIndex { get; set; }
    public int NumTriangles { get; set; }
    public byte TriangleSorting { get; set; }
}