using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class ColorStream
{
    public int ItemSize { get; set; }
    public int NumVerts { get; set; }
    public TArray<FColor> Colors { get; set; } = new();


    //public void Deserialize(BinaryReader Reader)
    //{
    //    ItemSize = Reader.ReadInt32();
    //    NumVerts = Reader.ReadInt32();

    //    if (NumVerts > 0)
    //    {
    //        Colors.Deserialize(Reader);
    //    }
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write(ItemSize);
    //    writer.Write(NumVerts);
    //    if (NumVerts > 0)
    //    {
    //        Colors.Serialize(writer);
    //    }
    //}
}