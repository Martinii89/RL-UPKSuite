using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class FStaticMeshLODModel3
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

    //public void Deserialize(BinaryReader Reader)
    //{
    //    FBulkData.Deserialize(Reader);
    //    FStaticMeshSections.Deserialize(Reader);
    //    VertexStream.Deserialize(Reader);
    //    UvStream.Deserialize(Reader);
    //    ColorStream.Deserialize(Reader);
    //    NumVerts = Reader.ReadInt32();
    //    Indicies.Deserialize(Reader);
    //    Indicies2.Deserialize(Reader);
    //    Indicies3.Deserialize(Reader);
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    FBulkData.Serialize(writer);
    //    FStaticMeshSections.Serialize(writer);
    //    VertexStream.Serialize(writer);
    //    UvStream.Serialize(writer);
    //    ColorStream.Serialize(writer);
    //    writer.Write(NumVerts);
    //    Indicies.Serialize(writer);
    //    Indicies2.Serialize(writer);
    //    Indicies3.Serialize(writer);
    //}
}