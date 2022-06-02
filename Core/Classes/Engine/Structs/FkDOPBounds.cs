using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class FkDOPBounds
{
    public FVector V1 { get; set; } = new();
    public FVector V2 { get; set; } = new();

    //public void Deserialize(BinaryReader Reader)
    //{
    //    V1.Deserialize(Reader);
    //    V2.Deserialize(Reader);
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    V1.Serialize(writer);
    //    V2.Serialize(writer);
    //}
}