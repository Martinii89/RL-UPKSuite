using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class VertexStream
{
    public int VertexSize { get; set; }
    public int VertexCount { get; set; }
    public TArray<FVector> VertexStreamArray { get; set; } = new();
}