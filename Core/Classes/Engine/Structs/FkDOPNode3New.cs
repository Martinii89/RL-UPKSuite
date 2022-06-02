namespace Core.Classes.Engine.Structs;

public class FkDOPNode3New
{
    public byte[] Mins { get; init; } = new byte[3];
    public byte[] Maxs { get; init; } = new byte[3];

    //public void Deserialize(BinaryReader Reader)
    //{
    //    Reader.Read(Mins, 0, 3);
    //    Reader.Read(Maxs, 0, 3);
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write(Mins, 0, 3);
    //    writer.Write(Maxs, 0, 3);
    //}
}