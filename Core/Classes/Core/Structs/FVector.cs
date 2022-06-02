namespace Core.Classes.Core.Structs;

public class FVector
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    //public void Deserialize(BinaryReader Reader)
    //{
    //    X = Reader.ReadSingle();
    //    Y = Reader.ReadSingle();
    //    Z = Reader.ReadSingle();
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write(X);
    //    writer.Write(Y);
    //    writer.Write(Z);
    //}
}