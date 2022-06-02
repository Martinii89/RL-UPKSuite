namespace Core.Classes.Engine.Structs;

public class FColor
{
    public byte R { get; init; }
    public byte G { get; init; }
    public byte B { get; init; }
    public byte A { get; init; }

    //public void Deserialize(BinaryReader Reader)
    //{
    //    R = Reader.ReadByte();
    //    G = Reader.ReadByte();
    //    B = Reader.ReadByte();
    //    A = Reader.ReadByte();
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write(R);
    //    writer.Write(G);
    //    writer.Write(B);
    //    writer.Write(A);
    //}
}