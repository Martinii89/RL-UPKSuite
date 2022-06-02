using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class VertexStream
{
    public int VertexSize { get; set; }
    public int VertexCount { get; set; }
    public TArray<FVector> VertexStreamArray { get; set; } = new();

    //public void Deserialize(BinaryReader Reader)
    //{
    //    VertexSize = Reader.ReadInt32();
    //    VertexCount = Reader.ReadInt32();
    //    VertexStreamArray.Deserialize(Reader);
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write(VertexSize);
    //    writer.Write(VertexCount);
    //    VertexStreamArray.Serialize(writer);
    //}
}