using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class FStaticMeshLODModel
{
    public FByteBulkData FBulkData { get; set; } = new();
    public List<FStaticMeshSection> FStaticMeshSections { get; set; } = new();
    public VertexStream VertexStream { get; set; } = new();
    public UvStream UvStream { get; set; } = new();
    public ColorStream ColorStream { get; set; } = new();
    public int NumVerts { get; set; }
    public TArray<ushort> Indicies { get; set; } = new();
    public TArray<ushort> Indicies2 { get; set; } = new();
    public TArray<ushort> Indicies3 { get; set; } = new();
}