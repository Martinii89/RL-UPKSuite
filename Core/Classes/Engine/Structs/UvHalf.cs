namespace Core.Classes.Engine.Structs;

public class UvHalf
{
    public ushort A { get; set; }
    public ushort B { get; set; }

    //public void Deserialize(BinaryReader Reader)
    //{
    //    A = Reader.ReadUInt16();
    //    B = Reader.ReadUInt16();
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write(A);
    //    writer.Write(B);
    //}
}